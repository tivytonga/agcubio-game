using AgCubio;
using System;
using Newtonsoft.Json;
using System.Text;

public class Program
{
    static PreservedState state;
    public static void Main(string[] args)
    {
        Console.WriteLine("Begin Connect");
        state = Network.Connect_to_Server(() => rightAfterConnected(), "localhost");
        Console.Read();
    }

    private static void rightAfterConnected()
    {
        Console.WriteLine("Connected.");
        state.callback = () => wantMore();
        Network.Send(state, "My_Name\n");
        wantMore();
    }

    static int i = 0;
    private static void wantMore()
    {
        if (i < 2)
        {
            Console.WriteLine(i);
            int k = 0;
            foreach (string s in state.getLines())
            {
                Console.WriteLine("Line: " + k);
                Console.WriteLine("Raw text: " + s);
                Cube cube = JsonConvert.DeserializeObject<Cube>(s);
                Console.WriteLine("Cube name and id: " + cube.ToString());
                k++;
            }
            i++;
            Network.i_want_more_data(state);
        }
    }
}