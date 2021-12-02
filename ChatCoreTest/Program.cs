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
        private static List<int> messageTypeList;

        public enum Type
        {
            isInt = 1,
            isFloat = 2,
            isString = 3,
        }

        public static void Main(string[] args)
        {
            getLengh = 0;
            m_PacketData = new byte[1024];
            m_Pos = 4;
            messageTypeList = new List<int>();
            Write(109);
            Write(109.99f);
            Write("Hello!");
            Write("Hello!");
            Write("WOOOOW!");
            Write(333.33f);
            Write(999);

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
            // convert int to byte array
            var bytes = BitConverter.GetBytes(i);
            messageTypeList.Add((int)Type.isInt);
            _Write(bytes);
            return true;
        }

        // write a float into a byte array
        private static bool Write(float f)
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(f);
            messageTypeList.Add((int)Type.isFloat);
            _Write(bytes);
            return true;
        }

        // write a string into a byte array
        private static bool Write(string s)
        {
            // convert string to byte array
            var bytes = Encoding.Unicode.GetBytes(s);

            // write byte array length to packet's byte array
            if (Write(bytes.Length) == false)
            {
                return false;
            }
            messageTypeList.RemoveAt(messageTypeList.Count - 1);

            _Write(bytes);
            messageTypeList.Add((int)Type.isString);
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

            m_Pos = 0;
            for (int i = 0; i < messageTypeList.Count; i++)
            {
                string request = null;
                byte[] tmpBytes = null;
                int messageLengh = 0;
                if (messageTypeList[i] == (int)Type.isInt)
                {
                    messageLengh = 4;
                    m_Pos += (uint)messageLengh ;
                    tmpBytes = new byte[messageLengh];
                    Array.Copy(byteData, m_Pos, tmpBytes, 0, 4);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(tmpBytes);
                    }

                    request = BitConverter.ToInt32(tmpBytes, 0).ToString();



                    Console.Write($"{request}\n");
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(tmpBytes);
                    }
                }
                else if (messageTypeList[i] == (int)Type.isFloat)
                {
                    messageLengh = 4;
                    m_Pos += (uint)messageLengh;
                    tmpBytes = new byte[messageLengh];
                    Array.Copy(byteData, m_Pos, tmpBytes, 0, 4);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(tmpBytes);
                    }

                    request = BitConverter.ToSingle(tmpBytes, 0).ToString();

                    Console.Write($"{request}\n");
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(tmpBytes);
                    }
                }
                else if (messageTypeList[i] == (int)Type.isString)
                {

                    byte[] stringLengh = new byte[4] ;
                    m_Pos += (uint)stringLengh.Length;
                    Array.Copy(byteData, m_Pos, stringLengh, 0, 4);
                    Array.Reverse(stringLengh);
                    int tmpLengh = BitConverter.ToInt32(stringLengh, 0);
                    Array.Reverse(stringLengh);

                    m_Pos += 4;
                    tmpBytes = new byte[tmpLengh];
                    Array.Copy(byteData, m_Pos, tmpBytes, 0, tmpLengh);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(tmpBytes);
                    }

                    request = Encoding.Unicode.GetString(tmpBytes, 0, tmpLengh);

                    Console.Write($"{request}\n");
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(tmpBytes);
                    }
                    m_Pos += (uint)tmpLengh-4;
                }

            }
        }

    }
}
