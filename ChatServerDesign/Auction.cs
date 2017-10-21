using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServerDesign
{
    class Auction
    {
        int auctionSeconds = 15;
        string name;
        string highestBidder;
        decimal startPrice;
        decimal currentPrice;
        decimal priceIncrease;
        ChatService cs;
        object padLock;

        public Auction(string name, decimal startPrice, decimal priceIncrease, ChatService cs)
        {
            this.name = name;
            this.startPrice = startPrice;
            currentPrice = this.startPrice;
            this.priceIncrease = priceIncrease;
            this.cs = cs;
            padLock = new object();
        }

        public void Run()
        {
            while (auctionSeconds > 0)
            {
                Thread.Sleep(1000);
                
                if (currentPrice > startPrice)
                {
                    auctionSeconds--;

                    switch (auctionSeconds)
                    {
                        case 4:
                            Console.WriteLine("Going first!");
                            cs.Broadcast("Going first!");
                            break;
                        case 2:
                            Console.WriteLine("Going twice!");
                            cs.Broadcast("Going twice!");
                            break;
                        case 0:
                            Console.WriteLine($"SOLD to {highestBidder}");
                            cs.Broadcast($"SOLD to {highestBidder}");
                            break;
                    }
                }                
            }
        }

        //Use a delegate instead of ClientHandler ch (ch.SendMessage)
        //comment
        public void Bid(ClientHandler ch)
        {
            lock (padLock)
            {
                if (auctionSeconds > 0)
                {
                    currentPrice += priceIncrease;
                    highestBidder = ch.Name;
                    Console.WriteLine($"Bid on {name} has increased to: {currentPrice} by {highestBidder}");
                    cs.Broadcast($"Bid on {name} has increased to: {currentPrice} by {highestBidder}");
                    if (auctionSeconds < 6)
                        auctionSeconds = 6;
                }
                else
                    ch.SendMessage("The auction has already ended");
            }
        }
    }
}
