using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    class Program
    {
        static HashSet<TcpClient> clients = new HashSet<TcpClient>();

        static void Main(string[] args)
        {
            const int port = 4099;

            Console.WriteLine("-----");

            TcpListener listener = new TcpListener(IPAddress.Any, port);

            try
            {
                Console.WriteLine($"伺服器 開始於 port{port}");
                listener.Start();

                Console.WriteLine($"等待連線......");
                TcpClient client = listener.AcceptTcpClient();

                string address = client.Client.RemoteEndPoint.ToString();
                Console.WriteLine($"客戶端 已經連限於{address}端");

                client.Close();
                Console.WriteLine($"取消連線 客戶端{address}");

            }
            catch (SocketException e)
            {
                Console.WriteLine($" 當機 發生於{e}");
            }
            finally
            {
                //停止監聽新客戶端連線
                listener.Stop();
                Console.WriteLine($"伺服器端關閉");
            }
        }

        private static void HandleMessage()
        {
            while (true)
            {
                lock (clients)
                {
                    foreach (var client in clients)
                    {
                        try
                        {
                            if (client.Available > 0)
                                Recevie(client);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error : {e}");
                        }
                    }
                }
            }
        }

        private static void Recevie(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            string address = client.Client.RemoteEndPoint.ToString();

            int numBytes = client.Available;
            if (numBytes == 0)
                return;

            byte[] buffer = new byte[numBytes];
            int bytesRead = stream.Read(buffer, 0, numBytes);

            string request = Encoding.ASCII.GetString(buffer).Substring(0, bytesRead);
            Console.WriteLine($"Text:{request} from {address}");

        }
    }
}
