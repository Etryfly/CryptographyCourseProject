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
                    using (NetworkStream stream = tcpClient.GetStream())
                    {
                        BigInteger e;
                        BigInteger d;
                        BigInteger n;

                        CryptographyCourseProject.RSA.GenerateKeys(1024, out e, out n, out d);
                        NetworkStreamUtils.WriteDataIntoStream(Message.KEY, e.ToByteArray(), stream);
                        NetworkStreamUtils.WriteDataIntoStream(Message.KEY, n.ToByteArray(), stream);
                           
                    }
                }
            }
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
