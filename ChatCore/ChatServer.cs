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
    public class ChatServer
    {
        private int port;
        private TcpListener listener;
        private Thread handleThread;

        private readonly Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
        private readonly Dictionary<string, string> userNames = new Dictionary<string, string>();
        private readonly Dictionary<string, string> accounts = new Dictionary<string, string>();

        public ChatServer()
        {
            accounts.Add("arthur", "1111");//建立測試用
            accounts.Add("jojo", "1111");//建立測試用
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
                TcpClient client = listener.AcceptTcpClient();

                string clientID = client.Client.RemoteEndPoint.ToString();
                Console.WriteLine($"客戶端{clientID}已連線");

                lock (clients)
                {
                    clients.Add(clientID, client);
                }
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

        private void SendData(TcpClient client,string data)
        {
            byte[] requestBuffer = System.Text.Encoding.Unicode.GetBytes(data);
            client.GetStream().Write(requestBuffer, 0, requestBuffer.Length);
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
                if(tokens.Length != 3)
                {
                    Console.WriteLine($"客戶{clientID} 登入失敗");
                    SendData(client, "LOGIN:0");
                    return;
                }

                string username = tokens[1];
                string password = tokens[2];

                if(accounts[username] != password)
                {
                    Console.WriteLine($"客戶{clientID},{username} 登入失敗 / 密碼錯誤");
                    SendData(client, "LOGIN:0");
                    return;
                }

                userNames[clientID] = username;
                Console.WriteLine($"Client{clientID} 登入  從{username}");

                SendData(client, "LOGIN:1");
                return;
            }

            if (request.StartsWith("MESSAGE:", StringComparison.OrdinalIgnoreCase))
            {
                String[] tokens = request.Split(':');
                string message = tokens[1];

                if (!userNames.ContainsKey(clientID))
                {
                    Console.WriteLine($"TEXT:{message} from 未知對象");
                }
                else
                {
                    Console.WriteLine($"TEXT:{message} form{clientID}");
                }
            }
        }
    }
}
