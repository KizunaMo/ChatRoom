using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatCore
{
    public class ChatClient
    {
        private TcpClient client;

        public ChatClient()
        {

        }

        public bool Connect(string address,int port)
        {
            client = new TcpClient();

            try
            {
                Console.WriteLine($"連線到聊天伺服器 {address}:{port}");
                client.Connect(address, port);

                Console.WriteLine($"已連線到聊天伺服器");
                return client.Connected;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"ArgumentException : {e}");
                return false;
            }
            catch (SocketException e)
            {
                Console.WriteLine($"SocketException : {e}");
                return false;
            }
        }
        
        public void Disconnect()
        {
            client.Close();
            Console.WriteLine($"已中斷連線");
        }

        public void SendData(string message)
        {
            byte[] requestBuffer = Encoding.ASCII.GetBytes(message);
            client.GetStream().Write(requestBuffer, 0, requestBuffer.Length);
        }

    }
}
