using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    class TcpServer
    {
        TcpListener Listener = new TcpListener(IPAddress.Any, 8888);

        public TcpServer()
        {
            Listener.Start();

            while (true)
            {

                using (TcpClient tcpClient = Listener.AcceptTcpClient())
                {
                    byte[] block = new byte[tcpClient.ReceiveBufferSize];
                    using (NetworkStream stream = tcpClient.GetStream())
                    {
                        BigInteger e;
                        BigInteger d;
                        BigInteger n;

                        CryptographyCourseProject.RSA.GenerateKeys(1024, out e, out n, out d);

                        Console.WriteLine(e.ToString());

                        
                            BinaryFormatter formatter = new BinaryFormatter();
                            foreach(byte[] packageBlock in SplitByteArray(e.ToByteArray(), Package.PackageDataLength))
                            {
                                Package package = new Package();
                                package.message = Message.KEY;
                                package.data = packageBlock;

                                formatter.Serialize(stream, package);
                            }
                        Package end = new Package();
                        end.message = Message.END;
                      

                        formatter.Serialize(stream, end);
                        stream.Flush();
                    }
                }
            }
        }

        private List<byte[]> SplitByteArray(byte[] array, int size)
        {
            List<byte[]> result = new List<byte[]>();
            
           
                for (int i = 0; i < array.Length; i += size)
                {
                    byte[] block = new byte[size];
                    block = array.Take(size).ToArray();
                    array.Skip(size);
                    result.Add(block);
                }
            return result;
            
        }

        ~TcpServer()
        {
            if (Listener != null) Listener.Stop();
        }

        static void Main(string[] args)
        {
            new TcpServer();
           
        }
    }
}
