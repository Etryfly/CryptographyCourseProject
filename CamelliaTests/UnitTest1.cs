using System.IO;
using System.Text;
using Xunit;

namespace CamelliaTests
{
    public class UnitTest1
    {
        [Fact]
        public void EncryptTest()
        {
            byte[] key = ASCIIEncoding.ASCII.GetBytes("1234567890123456");
            CryptographyCourseProject.Camellia camellia = new CryptographyCourseProject.Camellia(key);


            byte[] message = ASCIIEncoding.ASCII.GetBytes("jghfuerygiop1234");
            byte[] message2 = ASCIIEncoding.ASCII.GetBytes("jghfuerygiop1qwe");



            byte[] encrypted = camellia.Encrypt(message);
            byte[] actual = camellia.Decrypt(encrypted);

            Assert.Equal(message, actual);
           
        }

        [Fact]
        public void EncryptDecryptJpgTest()
        {
            byte[] key = ASCIIEncoding.ASCII.GetBytes("1234567890123456");
           
            string input = @"C:\Users\Etryfly\source\repos\CryptographyCourseProject\CamelliaTests\2.jpg";
            string output = @"C:\Users\Etryfly\source\repos\CryptographyCourseProject\CamelliaTests\d.jpg";
            if (File.Exists(output))
                File.Delete(output);

            string tmp = Path.GetTempFileName();

            Ciphers.FileEncrypter.Encrypt(input, tmp, key, Ciphers.FileEncrypter.MODE.ECB);
            Ciphers.FileEncrypter.Decrypt(tmp, output, key, Ciphers.FileEncrypter.MODE.ECB);
            
            //Assert.Equal(message, actual);
        }
        [Fact]
        public void EncryptDecryptTextTest()
        {
            byte[] key = ASCIIEncoding.ASCII.GetBytes("1234567890123456");

            string input = @"C:\Users\Etryfly\source\repos\CryptographyCourseProject\CamelliaTests\Text.txt";
            string output = @"C:\Users\Etryfly\source\repos\CryptographyCourseProject\CamelliaTests\t.txt";
            if (File.Exists(output))
                File.Delete(output);

            //string tmp = Path.GetTempFileName();
            string tmp = @"C:\Users\Etryfly\source\repos\CryptographyCourseProject\CamelliaTests\e";

            Ciphers.FileEncrypter.Encrypt(input, tmp, key, Ciphers.FileEncrypter.MODE.ECB);
            Ciphers.FileEncrypter.Decrypt(tmp, output, key, Ciphers.FileEncrypter.MODE.ECB);

            //Assert.Equal(message, actual);
        }

    }
}
