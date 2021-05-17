using CryptographyCourseProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ciphers
{
    public static class FileEncrypter
    {

        private static readonly int Nb = 16;
        public enum MODE
        {
            ECB,
            CBC,
            CFB,
            OFB
        }

        public static List<byte[]> SplitFile(string fileName, int batchSizeInBytes)
        {

            List<byte[]> result = new List<byte[]>();
            byte[] batch = new byte[batchSizeInBytes];
            using (var stream = File.OpenRead(fileName))
            {
                int count = 0;
                while ((count = stream.Read(batch)) != 0)
                {
                    result.Add(batch);
                    batch = new byte[batchSizeInBytes];
                }
            }
            return result;

        }

        public static void Encrypt(string inputFile, string outputFile,  string Key, MODE mode, byte[] IV = null)
        {
            if (Key.Length != 16 && Key.Length != 24 && Key.Length != 32) throw new ArgumentException();
            switch (mode)
            {
                case MODE.ECB:
                    {
                        EncryptECB(inputFile, outputFile, Key);
                    }
                    break;

                case MODE.CBC:
                    {
                        EncryptCBC(inputFile, outputFile,Key, IV);
                    }
                    break;

                case MODE.CFB:
                    {
                        EncryptCFB(inputFile, outputFile,  Key, IV);
                    }
                    break;

                case MODE.OFB:
                    {
                        EncryptOFB(inputFile, outputFile, Key, IV);
                    }
                    break;
            }
        }

        public static void Decrypt(string inputFile, string outputFile,  string Key, MODE mode, byte[] IV = null)
        {
            if (Key.Length != 16 && Key.Length != 24 && Key.Length != 32) throw new ArgumentException();
            switch (mode)
            {
                case MODE.ECB:
                    {
                        DecryptECB(inputFile, outputFile,  Key);
                    }
                    break;

                case MODE.CBC:
                    {
                        DecryptCBC(inputFile, outputFile,  Key, IV);
                    }
                    break;

                case MODE.CFB:
                    {
                        DecryptCFB(inputFile, outputFile, Key, IV );
                    }
                    break;

                case MODE.OFB:
                    {
                        DecryptOFB(inputFile, outputFile, Key, IV );
                    }
                    break;
            }
        }
        public static void EncryptECB(string inputFile, string outputFile,  string Key)
        {
            byte[] keyInBytes = Encoding.ASCII.GetBytes(Key);
            Camellia camellia = new Camellia(keyInBytes);
            File.Delete(outputFile);
            List<byte[]> chunks = SplitFile(inputFile, Nb).ToList();

            List<byte[]> result = chunks.AsParallel().AsOrdered().Select((block) =>
            {

                return camellia.Encrypt(block);
            }).ToList();
            //string tmp = Path.GetTempFileName();

            using (var outputStream = File.OpenWrite(outputFile))
            {
                foreach (var line in result)
                {
                    outputStream.Write(line);
                }
            }


        }

        public static void DecryptECB(string inputFile, string outputFile, string Key)
        {
            byte[] keyInBytes = Encoding.ASCII.GetBytes(Key);
            Camellia camellia = new Camellia(keyInBytes);
            File.Delete(outputFile);
            List<byte[]> chunks = SplitFile(inputFile, Nb).ToList();

            List<byte[]> result = chunks.AsParallel().AsOrdered().Select((block) =>
            {

                return camellia.Decrypt(block);
            }).ToList();
            string tmp = Path.GetTempFileName();

            using (var outputStream = File.OpenWrite(tmp))
            {
                foreach (var line in result)
                {
                    outputStream.Write(line);
                }
            }

            using (var input = File.OpenRead(tmp))
            {
                using (var output = File.OpenWrite(outputFile))
                {
                    byte[] buffer = new byte[Nb ];
                    int count = 0;

                    while ((count = input.Read(buffer)) != 0)
                    {
                        if (input.Position == input.Length)
                        {
                            int i = buffer.Length - 1;
                            while (buffer[i] == 0)
                            {
                                i--;
                            }
                            output.Write(buffer, 0, i + 1);
                            buffer = new byte[1024];
                        }
                        else
                        {
                            output.Write(buffer);
                        }
                    }

                }
            }

            File.Delete(tmp);

        }

        public static void EncryptCBC(string inputFile, string outputFile, string Key, byte[] IV)
        {
            byte[] keyInBytes = Encoding.ASCII.GetBytes(Key);
            Camellia camellia = new Camellia(keyInBytes);
            File.Delete(outputFile);
            using (var InputStream = File.OpenRead(inputFile))
            using (var OutputStream = File.OpenWrite(outputFile))
            {
                byte[] block = new byte[Nb ];
                int count;
                byte[] Encrypted = IV;
                while ((count = InputStream.Read(block)) != 0)
                {

                    for (int i = count; i < block.Length; i++)
                    {
                        block[i] = 0x0;
                    }
                    block = XOR(block, Encrypted);
                    Encrypted = camellia.Encrypt(block);
                    OutputStream.Write(Encrypted);
                }

            }
        }

        public static void DecryptCBC(string inputFile, string outputFile, string Key, byte[] IV)
        {
            byte[] keyInBytes = Encoding.ASCII.GetBytes(Key);
            Camellia camellia = new Camellia(keyInBytes);
            File.Delete(outputFile);
            using (var InputStream = File.OpenRead(inputFile))
            using (var OutputStream = File.OpenWrite(outputFile))
            {
                byte[] block = new byte[Nb ];
                int count;
                byte[] Encrypted = IV;
                while ((count = InputStream.Read(block)) != 0)
                {

                    byte[] Decrypted = XOR(camellia.Decrypt(block), Encrypted);
                    Encrypted = (byte[])block.Clone();

                    if (InputStream.Position == InputStream.Length)
                    {
                        int i = Decrypted.Length - 1;
                        while (Decrypted[i] == 0)
                        {
                            i--;
                        }

                        OutputStream.Write(Decrypted, 0, i + 1);
                    }
                    else
                    {
                        OutputStream.Write(Decrypted);
                    }
                }
            }
        }

        public static void EncryptCFB(string inputFile, string outputFile, string Key, byte[] IV)
        {

            byte[] keyInBytes = Encoding.ASCII.GetBytes(Key);
            Camellia camellia = new Camellia(keyInBytes);
            File.Delete(outputFile); 
            
            using (var InputStream = File.OpenRead(inputFile))
            using (var OutputStream = File.OpenWrite(outputFile))
            {
                byte[] block = new byte[Nb ];
                int count;
                byte[] tmp = IV;
                while ((count = InputStream.Read(block)) != 0)
                {

                    for (int i = count; i < block.Length; i++)
                    {
                        block[i] = 0x0;
                    }
                    tmp = XOR(block, camellia.Encrypt(tmp));

                    OutputStream.Write(tmp);
                }

            }
        }

        public static void DecryptCFB(string inputFile, string outputFile,  string Key, byte[] IV)
        {
            byte[] keyInBytes = Encoding.ASCII.GetBytes(Key);
            Camellia camellia = new Camellia(keyInBytes);
            File.Delete(outputFile);
            using (var InputStream = File.OpenRead(inputFile))
            using (var OutputStream = File.OpenWrite(outputFile))
            {
                byte[] block = new byte[Nb];
                int count;
                byte[] tmp = IV;
                while ((count = InputStream.Read(block)) != 0)
                {

                    byte[] Decrypted = XOR(camellia.Encrypt(tmp), block);
                    tmp = (byte[])block.Clone();

                    if (InputStream.Position == InputStream.Length)
                    {
                        int i = Decrypted.Length - 1;
                        while (Decrypted[i] == 0)
                        {
                            i--;
                        }

                        OutputStream.Write(Decrypted, 0, i + 1);
                    }
                    else
                    {
                        OutputStream.Write(Decrypted);
                    }
                }
            }
        }


        public static void EncryptOFB(string inputFile, string outputFile,  string Key, byte[] IV)
        {
            byte[] keyInBytes = Encoding.ASCII.GetBytes(Key);
            Camellia camellia = new Camellia(keyInBytes);
            File.Delete(outputFile);
            using (var InputStream = File.OpenRead(inputFile))
            using (var OutputStream = File.OpenWrite(outputFile))
            {
                byte[] block = new byte[Nb ];
                int count;
                byte[] tmp = IV;
                while ((count = InputStream.Read(block)) != 0)
                {

                    for (int i = count; i < block.Length; i++)
                    {
                        block[i] = 0x0;
                    }
                    tmp = camellia.Encrypt(tmp);
                    OutputStream.Write(XOR(block, tmp));
                }

            }
        }

        public static void DecryptOFB(string inputFile, string outputFile, string Key, byte[] IV)
        {
            byte[] keyInBytes = Encoding.ASCII.GetBytes(Key);
            Camellia camellia = new Camellia(keyInBytes);
            File.Delete(outputFile);
            using (var InputStream = File.OpenRead(inputFile))
            using (var OutputStream = File.OpenWrite(outputFile))
            {
                byte[] block = new byte[Nb ];
                int count;
                byte[] tmp = IV;
                while ((count = InputStream.Read(block)) != 0)
                {

                    tmp = camellia.Encrypt(tmp);
                    byte[] Decrypted = XOR(block, tmp);


                    if (InputStream.Position == InputStream.Length)
                    {
                        int i = Decrypted.Length - 1;
                        while (Decrypted[i] == 0)
                        {
                            i--;
                        }

                        OutputStream.Write(Decrypted, 0, i + 1);
                    }
                    else
                    {
                        OutputStream.Write(Decrypted);
                    }
                }
            }
        }

        public static byte[] XOR(byte[] left, byte[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException();
            byte[] result = left.Clone() as byte[];
            for (int i = 0; i < left.Length; i++)
            {
                result[i] ^= right[i];
            }

            return result;
        }
    }
}

