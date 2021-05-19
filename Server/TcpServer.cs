using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

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



                        BigInteger c = new BigInteger(NetworkStreamUtils.ReadBytesFromStream(stream));
                        BigInteger symKey = CryptographyCourseProject.RSA.Decrypt(c, d, n);

                        byte[] IV = NetworkStreamUtils.ReadBytesFromStream(stream);

                        string workingDirectory = Environment.CurrentDirectory;
                        string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

                        string fileName = Encoding.ASCII.GetString(NetworkStreamUtils.ReadBytesFromStream(stream));

                        string Mode = Encoding.ASCII.GetString(NetworkStreamUtils.ReadBytesFromStream(stream));
                        string outputFilePath = projectDirectory + "/Files/" + fileName;
                        NetworkStreamUtils.ReadFileFromStream(outputFilePath + "e", stream);

                        Ciphers.FileEncrypter.Decrypt(outputFilePath + "e", outputFilePath, symKey.ToByteArray(),
                            (Ciphers.FileEncrypter.MODE)Enum.Parse(typeof(Ciphers.FileEncrypter.MODE), Mode), IV);

                        Console.WriteLine(outputFilePath + "Decrypted");
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
