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
        Auction auction;

        public bool CanRead
        {
            get { return stream.CanRead; }
        }

        public string Name { get; set; }

        public ClientHandler(Socket socket, ChatService chatService, Auction auction)
        {
            client = socket;
            this.chatService = chatService;
            stream = new NetworkStream(client);
            sr = new StreamReader(stream);
            sw = new StreamWriter(stream);
            this.auction = auction;
            Subscribe();
        }

        public void Run()
        {
            SendMessage($"You are {Name}");
            while (CanRead)
            {
                try
                {
                    sr.ReadLine();
                    auction.Bid(this);
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
            sw.WriteLine(message);
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
