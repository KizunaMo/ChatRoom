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
            handleThread.Start();

            while (true)
            {
                Console.WriteLine("等待連線");
            }





        }

        private void ClientsHandler()
        {
            while (true)
            {
                lock (clients)
                {
                    foreach(var clientID in clients.Keys)
                    {
                        TcpClient client = clients[clientID];

                        try
                        {
                            if (client.Available > 0)
                                ReceiveMessage(clientID);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Client {clientID} ,Error{e.Message}");
                        }
                    }
                }
            }
        }

        private void ReceiveMessage(string clientID)
        {
            TcpClient client = clients[clientID];
            NetworkStream stream = client.GetStream();

            int numBytes = client.Available;
            byte[] buffer = new byte[numBytes];
            int bytesRead = stream.Read(buffer, 0, numBytes);
            string request = Encoding.Unicode.GetString(buffer).Substring(0, bytesRead/2);

            if (request.StartsWith("LOGIN:", StringComparison.OrdinalIgnoreCase))
            {
                string[] tokens = request.Split(':');
                string message = tokens[1];
                Console.WriteLine($"TEXT:{message} form{clientID}");
            }
        }
    }
}
