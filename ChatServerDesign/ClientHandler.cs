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
        List<Channel> subscribedChannels;
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
            commandHandler = new CommandHandler();
            subscribedChannels = new List<Channel>();
        }

        public void Run()
        {
            Subscribe("main");
            string data;

            while (CanRead)
            {
                try
                {
                    data = sr.ReadLine();
                    Console.WriteLine("<{2}> {0} says: {1}", Name, data, currentChannel.Name);
                    HandleCommand(data);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                    EndClient();
                }
            }
        }

        public void SendMessage(string message)
        {
            SendServerMessage(string.Format("<{2}> {0} says: {1}", Name, message, currentChannel.Name));
        }

        private void SendServerMessage(string message)
        {
            try
            {
                sw.WriteLine(message);
                sw.Flush();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                EndClient();
            }
        }

        private void EndClient()
        {
            stream.Close();
            client.Close();
            UnsubscribeAll();
        }

        private void Subscribe(string channelName)
        {
            Channel channel = subscribedChannels.Find(x => x.Name == channelName);

            if (channel == null)
            {
                channel = chatService.JoinChannel(channelName);
                currentChannel = channel;
                currentChannel.Subscribers += SendMessage;
                subscribedChannels.Add(currentChannel);
                Console.WriteLine("{0} joined channel <{1}>", Name, channelName);
                SendServerMessage(string.Format("You joined channel <{0}>", channelName));
            }
            else
            {
                currentChannel = channel;
                SendServerMessage(string.Format("You are now typing in <{0}>", channel.Name));
            }
        }

        private void Unsubscribe(string channelName)
        {
            Channel channel = subscribedChannels.Find(x => x.Name == channelName);

            if (channel != null && channel.Name != "main")
            {
                currentChannel = subscribedChannels.FirstOrDefault();
                subscribedChannels.Remove(channel);
                channel.Subscribers -= SendMessage;
                SendServerMessage(string.Format("You left channel <{0}>", channel.Name));
            }
        }

        private void UnsubscribeAll()
        {
            for (int i = 0; i < subscribedChannels.Count; i++)
            {
                subscribedChannels[i].Subscribers -= SendMessage;
                subscribedChannels.RemoveAt(i);
            }
        }

        private void HandleCommand(string data)
        {
            string[] tokens = data.Split('/');

            if (tokens.Length > 1 && tokens[0] == "")
            {
                string[] parametres = tokens[1].Split(' ');

                switch (parametres[0].ToLower())
                {
                    case "join":
                        Subscribe(GetParametre(parametres, 1));
                        break;
                    case "leave":
                        Unsubscribe(GetParametre(parametres, 1));
                        break;
                    default:
                        SendServerMessage("Invalid command!");
                        break;
                }
            }
            else if (currentChannel != null)
                chatService.InitiateBroadcast(currentChannel.Name, data);
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
