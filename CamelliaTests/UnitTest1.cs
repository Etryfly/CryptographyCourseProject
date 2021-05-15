using System.Text;
using Xunit;

namespace CamelliaTests
{
    public class UnitTest1
    {
        [Fact]
        public void EncryptTest()
        {
            byte[] key = ASCIIEncoding.ASCII.GetBytes("01234567890123456");
            CryptographyCourseProject.Camellia camellia = new CryptographyCourseProject.Camellia(key);


            byte[] message = ASCIIEncoding.ASCII.GetBytes("jghfuerygiop1234");

           

            byte[] encrypted = camellia.Encrypt(message);
            byte[] actual = camellia.Decrypt(encrypted);

            Assert.Equal(message, actual);
        }

       
    }
}
