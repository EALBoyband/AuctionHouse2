using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServerDesign
{
    class ChatService
    {
        List<Channel> channels;

        public ChatService()
        {
            channels = new List<Channel>();
            Channel main = new Channel();
            main.Name = "main";
            channels.Add(main);
        }

        public Channel JoinChannel(string name)
        {
            Channel channel = channels.Find(x => x.Name == name);

            if (channel == null)
            {
                channel = new Channel();
                channel.Name = name;
                channels.Add(channel);
            }

            return channel;
        }

        public void LeaveChannel(string name, ClientHandler clientHandler)
        {
            Channel channel = channels.Find(x => x.Name == name);

            if (channel != null)
                channel.Subscribers -= clientHandler.SendMessage;
        }

        public void InitiateBroadcast(string channelName, string data)
        {
            channels.Find(x => x.Name == channelName).Broadcast(data);
        }
    }
}
