using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Client
{
    class Client
    {
        public static WebSocket client;
        static void Main(string[] args)
        {
            client = new WebSocket("ws://localhost:8080");
            client.Connect();
            string s = Console.ReadLine();
            while(s!="0")
            {
                client.Send(s);
                s = Console.ReadLine();
            }
        }
    }
}
