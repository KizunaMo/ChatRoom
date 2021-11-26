using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatCore
{
    class ChatServer
    {
        private int port;
        private TcpListener listener;
        private Thread handleThread;

        private readonly Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
        private readonly Dictionary<string, string> userNames = new Dictionary<string, string>();

        public ChatServer()
        {

        }

        public void Bind(int port)
        {
            this.port = port;

            listener = new TcpListener(IPAddress.Any, port);
            Console.WriteLine($"伺服器開始於port{port}");
            listener.Start();
        }

        public void StartSever()
        {
            handleThread = new Thread(ClientsHandler);
        }

        private void ClientsHandler()
        {

        }
    }
}
