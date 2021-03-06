using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Server
{
    public static class NetworkStreamUtils
    {
        public static byte[] ReadBytesFromStream(NetworkStream stream)
        {
            List<byte> result = new List<byte>();
            BinaryFormatter formatter = new BinaryFormatter();
            while (true)
            {

                Package package = (Package)formatter.Deserialize(stream);
                if (package.message == Message.END) break;
                result.AddRange(package.data);

            }
            return result.ToArray();
        }

        public static void WriteDataIntoStream(Message message, byte[] data, NetworkStream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            foreach (byte[] packageBlock in SplitByteArray(data, Package.PackageDataLength))
            {
                Package package = new Package();
                package.message = message;
                package.data = packageBlock;

                formatter.Serialize(stream, package);
            }
            Package end = new Package();
            end.message = Message.END;


            formatter.Serialize(stream, end);
            stream.Flush();
        }

        public static void WriteFileIntoStream(string fileName, NetworkStream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (var input = File.OpenRead(fileName)) {
                int count = 0;
                int size = 0;
                byte[] block = new byte[Package.PackageDataLength];
                while ((count = input.Read(block)) != 0)
                {
                    size += count;
                    Package package = new Package();
                    package.message = Message.FILE;
                    package.data = block;

                    formatter.Serialize(stream, package);

                    stream.Flush();
                }
            
            Package end = new Package();
            end.message = Message.END;


            formatter.Serialize(stream, end);
            stream.Flush();
            }
        }

        public static void ReadFileFromStream(string outputFileName, NetworkStream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (var output = File.OpenWrite(outputFileName))
            {
                
                while (true)
                {

                    Package package = (Package)formatter.Deserialize(stream);

                    if (package.message == Message.END) break;
                   
                    output.Write(package.data);

                }
            }
            
        }

        public static void SendDecryptedPackage(NetworkStream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Package package = new Package();
            package.message = Message.DECRYPTED;
            formatter.Serialize(stream, package);

            stream.Flush();
        }

        public static bool RecieveDecryptedPackage(NetworkStream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
          
            Package package = (Package)formatter.Deserialize(stream);
            if (package.message == Message.DECRYPTED) return true;
            return false;
        }

        private static List<byte[]> SplitByteArray(byte[] array, int size)
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
    }
}
