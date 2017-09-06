using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace ChatServerDesign
{
    class Server
    {
        IPAddress ip;
        int port;
        TcpListener listener;
        Socket client;
        ChatService chatService;
        int clientNumber = 0;

        public Server()
        {
            Console.Title = "Server";
            ip = IPAddress.Parse("127.0.0.1");
            port = 11111;
            listener = new TcpListener(ip, port);
            listener.Start();
            chatService = new ChatService();
        }

        public void Run()
        {
            ClientHandler ch;

            while (true)
            {
                client = listener.AcceptSocket();
                Console.WriteLine("Client found!");
                ch = new ClientHandler(client, chatService);
                ch.Name = "Client_" + clientNumber;
                new Thread(() => ch.Run()).Start();
                clientNumber++;
            }
        }
    }
}
