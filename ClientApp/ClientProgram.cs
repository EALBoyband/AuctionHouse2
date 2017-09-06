using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace ClientApp
{
    class ClientProgram
    {
        TcpClient client;
        NetworkStream stream;
        StreamReader sr;
        StreamWriter sw;
        bool connected;

        public ClientProgram()
        {
            Console.Title = "Client";
            client = new TcpClient();
            client.Connect("127.0.0.1", 11111);
            stream = client.GetStream();
            sr = new StreamReader(stream);
            sw = new StreamWriter(stream);
            connected = true;

            Console.WriteLine("Connected!");
        }

        public void Run()
        {
            new Thread(() => RecieveLoop()).Start();

            while (connected)
            {
                Write(Console.ReadLine());
            }
        }

        private void Write(string message)
        {
            if (connected)
            {
                sw.WriteLine(message);
                sw.Flush();
            }
        }

        private void RecieveLoop()
        {
            try
            {
                string data = sr.ReadLine();
                while (data != null)
                {
                    Console.WriteLine(data);
                    data = sr.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                connected = false;
            }
        }
    }
}
