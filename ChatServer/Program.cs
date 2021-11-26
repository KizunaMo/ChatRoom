using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    internal class Program
    {
        static HashSet<TcpClient> clients = new HashSet<TcpClient>();

        static void Main(string[] args)
        {
            Console.WriteLine($"--------");
            ChatCore.ChatServer server = new ChatCore.ChatServer();
            server.Bind(4099);
            server.StartSever();
        }
    }
}
