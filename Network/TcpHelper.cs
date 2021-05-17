using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Network
{
    public static class TcpHelper
    {
        public static void SendData(byte[] data)
        {
            using (TcpClient tcpClient = new TcpClient("localhost", 8888))
            {
                byte[] block = new byte[tcpClient.ReceiveBufferSize];
                using (NetworkStream stream = tcpClient.GetStream())
                {
                    for (int i = 0; i < data.Length; i += block.Length)
                    {
                        block = data.Take(block.Length).ToArray();
                        data.Skip(block.Length);
                        stream.Write(block);
                        block = new byte[tcpClient.ReceiveBufferSize];
                    }
                    stream.Flush();
                }
            }
        }

        public static List<byte[]> ReceiveData()
        {
            List<byte[]> result = new List<byte[]>();
            using (TcpClient tcpClient = new TcpClient("localhost", 8888))
            {
                byte[] block = new byte[tcpClient.ReceiveBufferSize];
                using (NetworkStream stream = tcpClient.GetStream())
                {
                    int count = 0;
                    while ((count = stream.Read(block)) != 0)
                    {
                        result.Add(block);
                    }
                }
            }
            return result;
        }
    }
}
