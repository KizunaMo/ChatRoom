using System;
using System.Net.Sockets;
using System.Text;

namespace ChatClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--------");
            ChatCore.ChatClient client = new ChatCore.ChatClient();

            Console.WriteLine($"<請輸入名字>");
            string name = Console.ReadLine();

            Console.WriteLine("請輸入密碼");
            string password = Console.ReadLine();

            string IP = "127.0.0.1";
            int port = 4099;
            bool succeed = client.Connect(IP, port);
            if (!succeed)
                return;

            client.SetName(name,password);
            Console.WriteLine($"你現在可以輸入任何訊息......");

            while (true)
            {
                while(Console.KeyAvailable == false)
                {
                    client.Refresh();
                }


                string message = Console.ReadLine();

                if (message == "exit")
                {
                    Console.WriteLine($"離開連線");
                    client.Disconnect();
                    break;
                }

                client.SendMessage(message);
                Console.WriteLine($"訊息已傳輸；可以輸入任意資訊再次傳送");
            }
        }
    }
}
