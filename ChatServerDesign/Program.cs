﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServerDesign
{
    class Program
    {
        static void Main(string[] args)
        {
            Server s = new Server();
            s.Run();
        }
    }
}
