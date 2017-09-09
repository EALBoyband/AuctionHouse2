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
        Channel currentChannel;
        CommandHandler commandHandler;

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
            Subscribe("main");
            commandHandler = new CommandHandler();
        }

        public void Run()
        {
            string data;

            while (CanRead)
            {
                try
                {
                    data = sr.ReadLine();
                    Console.WriteLine("<{2}> {0} says: {1}", Name, data, currentChannel.Name);
                    HandleCommand(data);
                    currentChannel.Broadcast(data);
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
            sw.WriteLine("<{2}> {0} says: {1}", Name, message, currentChannel.Name);
            sw.Flush();
        }

        private void EndClient()
        {
            Unsubscribe();
            stream.Close();
            client.Close();
        }

        private void Subscribe(string channelName)
        {
            if (currentChannel != null)
                Unsubscribe();

            Console.WriteLine("{0} joined channel <{1}>", Name, channelName);
            sw.WriteLine("You joined channel <{0}>", channelName);
            sw.Flush();
            currentChannel = chatService.JoinChannel(channelName);
            currentChannel.Subscribers += SendMessage;
        }

        private void Unsubscribe()
        {
            currentChannel.Subscribers -= SendMessage;
        }

        private void HandleCommand(string data)
        {
            string[] tokens = data.Split('/');

            if (tokens.Length > 1 && tokens[0] == "")
            {
                string[] parametres = tokens[1].Split(' ');
                                
                switch (parametres[0].ToLower())
                {
                    case "channel":
                        Subscribe(GetParametre(parametres, 1));
                        break;
                    default:
                        sw.WriteLine("Invalid command!");
                        break;
                }
            }
        }

        private string GetParametre(string[] parametres, int num)
        {
            string parametre = "";

            if (parametres.Length >= num + 1)
                parametre = parametres[num];

            return parametre;
        }
    }
}
