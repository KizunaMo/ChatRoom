using System;
using System.Collections.Generic;
using System.Text;

namespace ChatCoreTest
{
    internal class Program
    {
        private static byte[] m_PacketData;
        private static uint m_Pos;
        private static int getLengh;
        private static int index;


        public static void Main(string[] args)
        {
            getLengh = 0;
            m_PacketData = new byte[1024];
            m_Pos = 4;
            Write(109);
            Write(109.99f);
            Write("Hello!");

            Console.Write($"Output Byte array(length:{m_Pos}): ");
            for (var i = 0; i < m_Pos; i++)
            {
                Console.Write(m_PacketData[i] + ", ");
            }

            Console.WriteLine($"開始讀取資料");
            Read(m_PacketData);
            Console.ReadLine();

        }

        // write an integer into a byte array
        private static bool Write(int i)
        {
            index = 1;
            // convert int to byte array
            var bytes = BitConverter.GetBytes(i);
            var bytesIndex = BitConverter.GetBytes(index);

            _Write(bytesIndex);
            _Write(bytes);
            return true;
        }

        // write a float into a byte array
        private static bool Write(float f)
        {
            index = 2;
            // convert int to byte array
            var bytes = BitConverter.GetBytes(index + f);
            var bytesIndex = BitConverter.GetBytes(index);
            _Write(bytesIndex);
            _Write(bytes);
            return true;
        }

        // write a string into a byte array
        private static bool Write(string s)
        {
            index = 3;
            // convert string to byte array
            var bytes = Encoding.Unicode.GetBytes(index + s);
            var bytesIndex = BitConverter.GetBytes(index);

            // write byte array length to packet's byte array
            if (Write(bytes.Length) == false)
            {
                return false;
            }

            _Write(bytesIndex);
            _Write(bytes);
            return true;
        }

        // write a byte array into packet's byte array
        private static void _Write(byte[] byteData)
        {
            GetMessageTotalLengh(byteData);
            // converter little-endian to network's big-endian
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteData);
            }

            byteData.CopyTo(m_PacketData, m_Pos);
            m_Pos += (uint)byteData.Length;
        }


        private static void GetMessageTotalLengh(byte[] byteData)
        {
            getLengh += byteData.Length;
            var bytes = BitConverter.GetBytes(getLengh);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            bytes.CopyTo(m_PacketData, 0);
        }

        private static void Read(byte[] byteData)
        {
            Console.WriteLine($"封包大小為{m_Pos}，封包前四個bytes為紀錄實際數據總長度:{getLengh}");
            for (int i = 4; i < m_Pos; i++)
            {
                m_Pos = 4;
                //string message = Encoding.Unicode.GetString(byteData).Substring(0, (int)m_Pos);
                //Console.Write($"{message},");
                m_Pos += (uint)byteData.Length;
            }
        }

    }
}
