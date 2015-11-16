using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AgCubio
{
    /// <summary>
    /// 
    /// </summary>
    public class PreservedState
    {
        public Action callback;
        public Socket socket;

        public byte[] buffer;
        public const int BUFFER_SIZE = 1024;
        public StringBuilder sb;
        
        public void BeginReceive()
        {
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, Network.ReceiveCallback, this);
        }

        public void BeginSend(byte[] outgoing)
        {
            socket.BeginSend(outgoing, 0, outgoing.Length, SocketFlags.None, Network.SendCallback, this);
        }

        public void HandleData(Encoding enc, int read)
        {
            lock(sb)
            {
                sb.Append(enc.GetString(buffer, 0, read));
            }
        }

        /// <summary>
        /// Returns the up to the first "amount" characters of the currently received data, and removes
        /// it from the PreservedState. If amount is more than the number of characters avaible, returns
        /// as many as are available.
        /// </summary>
        // todo probably get rid of this
        private string getData(int amount)
        {
            lock (sb)
            {
                string ret = sb.ToString(0, amount);
                sb.Remove(0, (amount > sb.Length ? sb.Length : amount));
                return ret;
            }
        }

        /// <summary>
        /// Returns the next available line (i.e. ending in "\n") of data available and removes it
        /// from the PreservedState. If a full line is unavailable, returns an empty string. Returned string
        /// is always without the "\n".
        /// </summary>
        public string getLine()
        {
            lock (sb)
            {
                string ret = "";
                for (int i = 0; i < sb.Length; i++)
                {
                    char curChar = sb[i];
                    if (curChar == '\n')
                    {
                        sb.Remove(0, i + 1);
                        return ret;
                    }
                    ret += curChar;
                }
                return "";
            }
        }

        public PreservedState(Action callback, Socket socket)
        {
            this.callback = callback;
            this.socket = socket;
            buffer = new byte[BUFFER_SIZE];
            sb = new StringBuilder();
        }
    }

    public static class Network
    {
        public const int DEFAULT_PORT = 11000;

        /// <summary>
        /// Encoding used for incoming/outgoing data
        /// </summary>
        private static UTF8Encoding encoding = new UTF8Encoding();

        /// <summary>
        /// For synchronizing sends (does nothing)
        /// </summary>
        private static readonly object sendSync = new object();

        /// <summary>
        /// Attempts to connect to the server via the provided hostname. Calls the given callback function upon connection.
        /// </summary>
        /// <param name="callback">Function to be called (inside the View) when a connection is made.</param>
        /// <param name="hostname">The name of the server to connect to.</param>
        public static PreservedState Connect_to_Server(Action callback, string hostname)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            PreservedState state = new PreservedState(callback, socket);

            socket.BeginConnect(hostname, DEFAULT_PORT, Connected_to_Server, state);
            return state;
        }

        /// <summary>
        /// Called by OS when socket connects to server.
        /// After this, user should send any initial data, then call i_want_more_data
        /// </summary>
        private static void Connected_to_Server(IAsyncResult stateInResult)
        {
            PreservedState state = (PreservedState)stateInResult.AsyncState;
            state.callback();
            state.socket.EndConnect(stateInResult);
        }

        /// <summary>
        /// Called by OS when new data arrives.
        /// </summary>
        public static void ReceiveCallback(IAsyncResult stateInResult)
        {
            PreservedState state = (PreservedState)stateInResult.AsyncState;
            int read = state.socket.EndReceive(stateInResult);
            if (read == 0)
            {
                state.socket.Close();
            }
            else
            {
                state.HandleData(encoding, read);
                state.callback();
            }
        }

        // Useless? have user call state.BeginReceive() theirself
        /// <summary>
        /// Begins the receiving of data.
        /// </summary>
        public static void i_want_more_data(PreservedState state)
        {
            state.BeginReceive();
        }

        /// <summary>
        /// Sends the given data over the network through the given socket.
        /// This function (along with it's helper 'SendCallback') will allow a program to send data over a socket. 
        /// This function needs to convert the data into bytes and then send them using socket.BeginSend.
        /// </summary>
        public static void Send(PreservedState state, string data)
        {
            byte[] outgoing = encoding.GetBytes(data);
            state.BeginSend(outgoing);
            /*
            //Looking over
            // Get exclusive access to send mechanism
            lock (sendSync)
            {
                // Append the message to the unsent string
                outgoing += message;

                // If there's not a send ongoing, start one.
                if (!sendIsOngoing)
                {
                    sendIsOngoing = true;
                    SendBytes();
                }
            }
            */
        }

        /// <summary>
        /// Helper for Send function. 
        /// </summary>
        public static void SendCallback(IAsyncResult stateInResult)
        {
            PreservedState state = (PreservedState)stateInResult.AsyncState;
            state.socket.EndSend(stateInResult);
        }
    }
}
