using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ChatServerDesign
{
    class ClientHandler
    {
        Socket client;
        ChatService chatService;
        NetworkStream stream;
        StreamReader sr;
        StreamWriter sw;

        public bool CanRead
        {
            get { return stream.CanRead; }
        }

        public string Name { get; set; }

        public ClientHandler(Socket socket, ChatService chatService)
        {
            client = socket;
            this.chatService = chatService;
            stream = new NetworkStream(client);
            sr = new StreamReader(stream);
            sw = new StreamWriter(stream);
            Subscribe();
        }

        public void Run()
        {
            string data;

            while (CanRead)
            {
                try
                {
                    data = sr.ReadLine();
                    Console.WriteLine("{0}: {1}", Name, data);
                    chatService.Broadcast(data);
                }
                catch(IOException e)
                {
                    Console.WriteLine(e.Message);
                    EndClient();
                }
            }
        }

        public void SendMessage(string message)
        {
            sw.WriteLine("{0}: {1}", Name, message);
            sw.Flush();
        }

        private void EndClient()
        {
            Unsubscribe();
            stream.Close();
            client.Close();
        }

        private void Subscribe()
        {
            chatService.Subscribers += SendMessage;
        }

        private void Unsubscribe()
        {
            chatService.Subscribers -= SendMessage;
        }
    }
}
