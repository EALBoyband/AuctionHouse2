using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServerDesign
{
    class ChatService
    {
        public delegate void Broadcaster(string message);
        public event Broadcaster Subscribers;

        public void Broadcast(string message)
        {
            Subscribers(message);
        }
    }
}
