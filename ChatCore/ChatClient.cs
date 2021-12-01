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
        private List<KeyValuePair<string, string>> messageList;

        public ChatClient()
        {

        }

        public bool Connect(string address, int port)
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

        public void SetName(string name, string password)
        {
            string data = "LOGIN:" + name + ":" + password;
            SendData(data);
        }

        public void SendMessage(string message)
        {
            string data = "MESSAGE:" + message;
            SendData(data);
        }

        public void SendData(string message)
        {
            byte[] requestBuffer = Encoding.Unicode.GetBytes(message);
            client.GetStream().Write(requestBuffer, 0, requestBuffer.Length);
        }

        public void Refresh()
        {
            if (client.Available > 0)
                HandleReceivedMessage(client);
        }
        public List<KeyValuePair<string, string>> GetMessages()
        {
            var messages = new List<KeyValuePair<string, string>>(messageList);
            messageList.Clear();

            return messages;
        }

        private void HandleReceivedMessage(TcpClient client)
        {
            NetworkStream steam = client.GetStream();

            int numBytes = client.Available;
            byte[] buffer = new byte[numBytes];
            int bytesRead = steam.Read(buffer, 0, numBytes);
            string request = System.Text.Encoding.Unicode.GetString(buffer).Substring(0, bytesRead);

            if (request.StartsWith("LOGIN:1", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("登入成功");
                return;
            }

            if (request.StartsWith("LOGIN:0", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("登入失敗");
                return;
            }

            if (request.StartsWith("MESSAGE:", StringComparison.OrdinalIgnoreCase))
            {
                string[] tokens = request.Split(':');
                string sender = tokens[1];
                string message = tokens[2];
                Console.WriteLine($"{sender}:{message}");
                messageList.Add(new KeyValuePair<string, string>(sender, message));
            }
        }
    }
}
