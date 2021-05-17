using System;
using System.Collections.Generic;
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
