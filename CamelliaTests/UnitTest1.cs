using Xunit;

namespace CamelliaTests
{
    public class UnitTest1
    {
        [Fact]
        public void EncryptTest()
        {
            ulong[] key = new ulong[]
          {
                0x0123456789abcdef, 0xfedcba9876543210
          };
            CryptographyCourseProject.Camellia camellia = new CryptographyCourseProject.Camellia(key);


            ulong[] message = new ulong[]
            {
                 0x0123456789abcdef, 0xfedcba9876543210
            };

            ulong[] expected = new ulong[]
            {
                0x6767313854966973,0x0857065648eabe43
            };


            ulong[] actual = camellia.Encrypt(message, key);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EncryptDecryptTest()
        {

            ulong[] key = new ulong[]
            {
                0x0123456789abcdef, 0xfedcba9876543210
            };

            CryptographyCourseProject.Camellia camellia = new CryptographyCourseProject.Camellia(key);

            ulong[] message = new ulong[]
            {
                 0x0123456789abcdef, 0xfedcba9876543210
            };




            ulong[] encrypted = camellia.Encrypt(message, key);
            ulong[] actual = camellia.Decrypt(encrypted, key);

            Assert.Equal(message, actual);
        }
    }
}
