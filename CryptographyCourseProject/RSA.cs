using System;
using System.Numerics;
using System.Security.Cryptography;

namespace CryptographyCourseProject
{
    public static class RSA
    {


        public static BigInteger GetPrime(int sizeInBytes)
        {
            Random random = new Random();
            byte[] bytes = new byte[sizeInBytes];

            while (true)
            {
                random.NextBytes(bytes);
                BigInteger q = new BigInteger(bytes);

                if (MillerRabin(q, 7)) return q;


            }
        }

        public static bool MillerRabin(this BigInteger source, int certainty)
        {
            if (source == 2 || source == 3)
                return true;
            if (source < 2 || source % 2 == 0)
                return false;

            BigInteger d = source - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }


            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[source.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < certainty; i++)
            {
                do
                {
                    rng.GetBytes(bytes);
                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= source - 2);

                BigInteger x = BigInteger.ModPow(a, d, source);
                if (x == 1 || x == source - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, source);
                    if (x == 1)
                        return false;
                    if (x == source - 1)
                        break;
                }

                if (x != source - 1)
                    return false;
            }

            return true;
        }

        public static BigInteger Inverse(BigInteger b, BigInteger n)
        {
            if (EuclideanAlgorithm(n, b) % n != 1) throw new ArgumentException();
            BigInteger s;
            BigInteger t;
            ExtendedEuclideanAlgorithm(n, b, out s, out t);
            while (t < 0)
            {
                t += n;
            }
            return t;
        }

        public static BigInteger EuclideanAlgorithm(BigInteger left, BigInteger right)
        {
            while (true)
            {
                if (left < right)
                {
                    BigInteger tmp = left;
                    left = right;
                    right = tmp;
                }

                BigInteger remainder = left % right;

                if (remainder == 0) return right;

                left = remainder;
            }
        }

        public static BigInteger ExtendedEuclideanAlgorithm(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y)
        {
            if (b == 0)
            {
                x = 1;
                y = 0;
                return a;
            }
            BigInteger tmpX;
            BigInteger tmpY;
            BigInteger gcd = ExtendedEuclideanAlgorithm(b, a % b, out tmpX, out tmpY);

            y = tmpX - tmpY * (a / b);
            x = tmpY;
            return gcd;
        }



        public static void GenerateKeys(int size, out BigInteger e, out BigInteger n, out BigInteger d)
        {
            BigInteger q = GetPrime(size / (8 * 2));
            BigInteger p = GetPrime(size / (8 * 2));

            n = p * q;

            BigInteger eiler = (p - 1) * (q - 1);

          

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[size / 32];
            while (true)
            {
                rng.GetBytes(bytes);

                d = new BigInteger(bytes);

                if (d > 0 && EuclideanAlgorithm(eiler, d) == 1 && !(36 * BigInteger.Pow(d, 4) < n)) break;
            }

            e = Inverse(d, eiler);
        }



        public static BigInteger Encrypt(BigInteger m, BigInteger e, BigInteger n)
        {
            return QuickPow(m, e, n);
        }

        public static BigInteger Decrypt(BigInteger c, BigInteger d, BigInteger n)
        {
            return QuickPow(c, d, n);
        }

        public static BigInteger QuickPow(BigInteger b, BigInteger degree, BigInteger mod)
        {
            BigInteger result = 1;
            BigInteger bitesForMask = degree;
            while (bitesForMask > 0)
            {
                if ((bitesForMask & 0b01) == 1)
                {
                    result = (result * b) % mod;
                }
                bitesForMask >>= 1;

                b *= b % mod;
            }
            return result;
        }
    }
}
