using AgCubio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace View
{
    /// <summary>
    /// Handles the title screen, welcoming the user and prompting for input.
    /// </summary>
    public partial class TitleForm : Form
    {
        string PlayerName;
        PreservedState state;
        FormManager manager;
        
        public TitleForm(FormManager manager)
        {
            this.manager = manager;
            InitializeComponent();
        }

        /// <summary>
        /// Called when a key is entered in a textbox. If the Enter key, attempts to connect to server.
        /// </summary>
        private void checkKeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                /// Player entered data for Player Name and Server
                if (!PlayerNameTextBox.Text.Equals("") && !HostNameTextBox.Text.Equals(""))
                {
                    PlayerName = PlayerNameTextBox.Text;
                    state = Network.Connect_to_Server(() => InitialConnection(), HostNameTextBox.Text);
                }
            }
        }

        /// <summary>
        /// Called when first connected to the server.
        /// </summary>
        private void InitialConnection()
        {
            if (!state.socket.Connected)
            {
                MessageBox.Show("Error: not connected to server.");
                return;
            }
            BeginInvoke((Action)(() => manager.startGameForm(state, PlayerName)));
        }
    }
}
