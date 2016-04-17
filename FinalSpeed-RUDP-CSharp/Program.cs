using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FinalSpeed_RUDP_CSharp.RUDP;

namespace FinalSpeed_RUDP_CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Route client = new Route(1, 150);
            Route server = new Route(2, 150);
        }
    }
}
