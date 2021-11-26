using System;
using System.Net.Sockets;
using System.Text;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            const string hostIP = "127.0.0.1";
            const int port = 4099;

            Console.WriteLine("--------");
            TcpClient client = new TcpClient();

            try
            {
                Console.WriteLine($"連線到聊天伺服器 {hostIP}:{port}");
                client.Connect(hostIP, port);

                if (!client.Connected)
                {
                    Console.WriteLine($"無法連線到聊天伺服器");
                    return;
                }
                Console.WriteLine($"已連線到聊天伺服器");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"ArgumentException : {e}");
            }
            catch (SocketException e)
            {
                Console.WriteLine($"SocketException : {e}");
            }
            finally
            {
                client.Close();
                Console.WriteLine($"連線中斷");
            }
        }
        private static void Send(TcpClient client , string message)
        {
            byte[] requestBuffer = Encoding.ASCII.GetBytes(message);//轉換ASCII成byte
            client.GetStream().Write(requestBuffer, 0, requestBuffer.Length);
        }


    }
}
