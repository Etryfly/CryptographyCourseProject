using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptographyCourseProject
{
    public class Camellia
    {
        private ulong[] _kw, _ke, _k;
        private readonly int keyLength;

        private readonly ulong MASK8 = 0xff;
        private readonly ulong MASK32 = 0xffffffff;
        private readonly ulong MASK64 = 0xffffffffffffffff;
        

        private readonly ulong C1 = 0xA09E667F3BCC908B;
        private readonly ulong C2 = 0xB67AE8584CAA73B2;
        private readonly ulong C3 = 0xC6EF372FE94F82BE;
        private readonly ulong C4 = 0x54FF53A5F1D36F1C;
        private readonly ulong C5 = 0x10E527FADE682D1D;
        private readonly ulong C6 = 0xB05688C2B3E6C1FD;

        private byte[] SBOX1 = {

            112, 130, 44, 236, 179, 39, 192, 229, 228, 133, 87, 53, 234, 12, 174, 65,

            35, 239, 107, 147, 69, 25, 165, 33, 237, 14, 79, 78, 29, 101, 146, 189,

            134, 184, 175, 143, 124, 235, 31, 206, 62, 48, 220, 95, 94, 197, 11, 26,

            166, 225, 57, 202, 213, 71, 93, 61, 217, 1, 90, 214, 81, 86, 108, 77,

            139, 13, 154, 102, 251, 204, 176, 45, 116, 18, 43, 32, 240, 177, 132, 153,

            223, 76, 203, 194, 52, 126, 118, 5, 109, 183, 169, 49, 209, 23, 4, 215,

            20, 88, 58, 97, 222, 27, 17, 28, 50, 15, 156, 22, 83, 24, 242, 34,

            254, 68, 207, 178, 195, 181, 122, 145, 36, 8, 232, 168, 96, 252, 105, 80,

            170, 208, 160, 125, 161, 137, 98, 151, 84, 91, 30, 149, 224, 255, 100, 210,

            16, 196, 0, 72, 163, 247, 117, 219, 138, 3, 230, 218, 9, 63, 221, 148,

            135, 92, 131, 2, 205, 74, 144, 51, 115, 103, 246, 243, 157, 127, 191, 226,

            82, 155, 216, 38, 200, 55, 198, 59, 129, 150, 111, 75, 19, 190, 99, 46,

            233, 121, 167, 140, 159, 110, 188, 142, 41, 245, 249, 182, 47, 253, 180, 89,

            120, 152, 6, 106, 231, 70, 113, 186, 212, 37, 171, 66, 136, 162, 141, 250,

            114, 7, 185, 85, 248, 238, 172, 10, 54, 73, 42, 104, 60, 56, 241, 164,

            64, 40, 211, 123, 187, 201, 67, 193, 21, 227, 173, 244, 119, 199, 128, 158

        };

        public Camellia(byte[] key)
        {
            ulong[] _key = new ulong[key.Length / 8];
            for (int i = 0; i < _key.Length; i++)
            {
                _key[i] = BitConverter.ToUInt64(key.Skip(8 * i).Take(8).ToArray());
            }

            if (_key.Length < 2 || _key.Length > 4) throw new ArgumentException();
            keyLength = _key.Length;
            GenerateKeys(_key);
        }

        private byte SBOX2(byte x)
        {
            return CircularShiftLeft(SBOX1[x], 1);
        }

        private byte SBOX3(byte x)
        {
            return CircularShiftLeft(SBOX1[x], 7);
        }

        private byte SBOX4(byte x)
        {
            return SBOX1[CircularShiftLeft(x, 1) & MASK8];
        }

        private void GenerateKeys(ulong[] key)
        {
            ulong[] splitedKey = SplitKey(key);
            ulong[] KL = new ulong[2];
            ulong[] KR = new ulong[2];
            ulong[] KA;
            ulong[] KB;
            KL[0] = splitedKey[0];
            KL[1] = splitedKey[1];
            KR[0] = splitedKey[2];
            KR[1] = splitedKey[3];



            GenerateKaKb(key, out KA, out KB);


            GenerateSubkeys(KA, KB, KL, KR, key.Length, out _kw, out _ke, out _k);
        }

        private ulong[] Rounds(ulong[] message, ulong[] kw, ulong[] ke, ulong[] k)
        {
            ulong D1 = message[0];
            ulong D2 = message[1];
            if (keyLength == 2)
            {
                D1 = D1 ^ kw[0];
                D2 = D2 ^ kw[1];
                D2 = D2 ^ F(D1, k[0]);
                D1 = D1 ^ F(D2, k[1]);
                D2 = D2 ^ F(D1, k[2]);
                D1 = D1 ^ F(D2, k[3]);
                D2 = D2 ^ F(D1, k[4]);
                D1 = D1 ^ F(D2, k[5]);
                D1 = FL(D1, ke[0]);
                D2 = FLINV(D2, ke[1]);
                D2 = D2 ^ F(D1, k[6]);
                D1 = D1 ^ F(D2, k[7]);
                D2 = D2 ^ F(D1, k[8]);
                D1 = D1 ^ F(D2, k[9]);
                D2 = D2 ^ F(D1, k[10]);
                D1 = D1 ^ F(D2, k[11]);
                D1 = FL(D1, ke[2]);
                D2 = FLINV(D2, ke[3]);
                D2 = D2 ^ F(D1, k[12]);
                D1 = D1 ^ F(D2, k[13]);
                D2 = D2 ^ F(D1, k[14]);
                D1 = D1 ^ F(D2, k[15]);
                D2 = D2 ^ F(D1, k[16]);
                D1 = D1 ^ F(D2, k[17]);
                D2 = D2 ^ kw[2];
                D1 = D1 ^ kw[3];
            }
            else
            {

                D1 = D1 ^ kw[0];
                D2 = D2 ^ kw[1];
                D2 = D2 ^ F(D1, k[0]);
                D1 = D1 ^ F(D2, k[1]);
                D2 = D2 ^ F(D1, k[2]);
                D1 = D1 ^ F(D2, k[3]);
                D2 = D2 ^ F(D1, k[4]);
                D1 = D1 ^ F(D2, k[5]);
                D1 = FL(D1, ke[0]);
                D2 = FLINV(D2, ke[1]);
                D2 = D2 ^ F(D1, k[6]);
                D1 = D1 ^ F(D2, k[7]);
                D2 = D2 ^ F(D1, k[8]);
                D1 = D1 ^ F(D2, k[9]);
                D2 = D2 ^ F(D1, k[10]);
                D1 = D1 ^ F(D2, k[11]);
                D1 = FL(D1, ke[2]);
                D2 = FLINV(D2, ke[3]);
                D2 = D2 ^ F(D1, k[12]);
                D1 = D1 ^ F(D2, k[13]);
                D2 = D2 ^ F(D1, k[14]);
                D1 = D1 ^ F(D2, k[15]);
                D2 = D2 ^ F(D1, k[16]);
                D1 = D1 ^ F(D2, k[17]);
                D1 = FL(D1, ke[4]);
                D2 = FLINV(D2, ke[5]);
                D2 = D2 ^ F(D1, k[18]);
                D1 = D1 ^ F(D2, k[19]);
                D2 = D2 ^ F(D1, k[20]);
                D1 = D1 ^ F(D2, k[21]);
                D2 = D2 ^ F(D1, k[22]);
                D1 = D1 ^ F(D2, k[23]);
                D2 = D2 ^ kw[2];
                D1 = D1 ^ kw[3];

            }
            return new ulong[] { D2, D1 };
        }

        public byte[] Encrypt(byte[] message)
        {
            if (message.Length != 16) throw new ArgumentException();
            ulong[] _message = new ulong[2];
            _message[0] = BitConverter.ToUInt64(message.Take(8).ToArray());
            _message[1] = BitConverter.ToUInt64(message.Skip(8).Take(8).ToArray());
          
           

            ulong[] kw = _kw;
            ulong[] ke = _ke;
            ulong[] k = _k;

            List<byte> result = new List<byte>();
            ulong[] roundsResult = Rounds(_message, kw, ke, k);
            result.AddRange(BitConverter.GetBytes(roundsResult[0]));
            result.AddRange(BitConverter.GetBytes(roundsResult[1]));

            return  result.ToArray();

        }

        public byte[] Decrypt(byte[] message)
        {
            if (message.Length != 16) throw new ArgumentException();
            ulong[] _message = new ulong[2];
            _message[0] = BitConverter.ToUInt64(message.Take(8).ToArray());
            _message[1] = BitConverter.ToUInt64(message.Skip(8).Take(8).ToArray());
            
           

            ulong[] kw = _kw;
            ulong[] ke = _ke;
            ulong[] k = _k;

            if (keyLength == 2)
            {
                Swap(ref kw[0], ref kw[2]);
                Swap(ref kw[1], ref kw[3]);
                Swap(ref k[0], ref k[17]);
                Swap(ref k[1], ref k[16]);
                Swap(ref k[2], ref k[15]);
                Swap(ref k[3], ref k[14]);
                Swap(ref k[4], ref k[13]);
                Swap(ref k[5], ref k[12]);
                Swap(ref k[6], ref k[11]);
                Swap(ref k[7], ref k[10]);
                Swap(ref k[8], ref k[9]);
                Swap(ref ke[0], ref ke[3]);
                Swap(ref ke[1], ref ke[2]);
            }
            else
            {
                Swap(ref kw[0], ref kw[2]);
                Swap(ref kw[1], ref kw[3]);
                Swap(ref k[0], ref k[23]);
                Swap(ref k[1], ref k[22]);
                Swap(ref k[2], ref k[21]);
                Swap(ref k[3], ref k[20]);
                Swap(ref k[4], ref k[19]);
                Swap(ref k[5], ref k[18]);
                Swap(ref k[6], ref k[17]);
                Swap(ref k[7], ref k[16]);
                Swap(ref k[8], ref k[15]);
                Swap(ref k[9], ref k[14]);
                Swap(ref k[10], ref k[13]);
                Swap(ref k[11], ref k[12]);
                Swap(ref ke[0], ref ke[5]);
                Swap(ref ke[1], ref ke[4]);
                Swap(ref ke[2], ref ke[3]);
            }

            List<byte> result = new List<byte>();
            ulong[] roundsResult = Rounds(_message, kw, ke, k);
            result.AddRange(BitConverter.GetBytes(roundsResult[0]));
            result.AddRange(BitConverter.GetBytes(roundsResult[1]));

            return result.ToArray();
        }

        #region Utils

        private void Swap(ref ulong left, ref ulong right)
        {
            ulong tmp = left;
            left = right;
            right = tmp;
        }
        private ulong CircularShiftLeft(ulong value, int count)
        {
            return value << count | value >> (64 - count);
        }

        private byte CircularShiftLeft(byte value, int count)
        {
            return (byte)(value << count | value >> (8 - count));
        }

        public ulong[] CircularShiftLeft(ulong[] value, int count)
        {
            if (value.Length != 2) throw new ArgumentException();
            if (count == 0) return value;
            ulong[] result = new ulong[2];
            if (count <= 64)
            {
                result[1] = value[0] >> (64 - count);
                result[1] |= value[1] << count;
                result[0] = value[0] << count;
                result[0] |= value[1] >> (64 - count);
            } else
            {
                result[1] = value[0] << ( count - 64);
                result[1] |= value[1] >> (128 - count);
                result[0] = value[0] >> (128 - count);
                result[0] |= value[1] << (count - 64);
            }

            return result;
        }
        #endregion



        private void GenerateSubkeys(ulong[] KA, ulong[] KB, ulong[] KL, ulong[] KR, int keySizeInUlongs, out ulong[] kw, out ulong[] ke, out ulong[] k)
        {
            kw = new ulong[4];
            ke = new ulong[6];
            k = new ulong[24];

            if (keySizeInUlongs == 2)
            {
                kw[0] = CircularShiftLeft(KL, 0)[0];
                kw[1] = CircularShiftLeft(KL, 0)[1];
                k[0] = CircularShiftLeft(KA, 0)[0];
                k[1] = CircularShiftLeft(KA, 0)[1];
                k[2] = CircularShiftLeft(KL, 15)[0];
                k[3] = CircularShiftLeft(KL, 15)[1];
                k[4] = CircularShiftLeft(KA, 15)[0];
                k[5] = CircularShiftLeft(KA, 15)[1];
                ke[0] = CircularShiftLeft(KA, 30)[0];
                ke[1] = CircularShiftLeft(KA, 30)[1];
                k[6] = CircularShiftLeft(KL, 45)[0];
                k[7] = CircularShiftLeft(KL, 45)[1];
                k[8] = CircularShiftLeft(KA, 45)[0];
                k[9] = CircularShiftLeft(KL, 60)[1];
                k[10] = CircularShiftLeft(KA, 60)[0];
                k[11] = CircularShiftLeft(KA, 60)[1];
                ke[2] = CircularShiftLeft(KL, 77)[0];
                ke[3] = CircularShiftLeft(KL, 77)[1];
                k[12] = CircularShiftLeft(KL, 94)[0];
                k[13] = CircularShiftLeft(KL, 94)[1];
                k[14] = CircularShiftLeft(KA, 94)[0];
                k[15] = CircularShiftLeft(KA, 94)[1];
                k[16] = CircularShiftLeft(KL, 111)[0];
                k[17] = CircularShiftLeft(KL, 111)[1];
                kw[2] = CircularShiftLeft(KA, 111)[0];
                kw[3] = CircularShiftLeft(KA, 111)[1];
            }
            else
            {
                kw[0] = CircularShiftLeft(KL, 0)[0];
                kw[1] = CircularShiftLeft(KL, 0)[1];
                k[0] = CircularShiftLeft(KB, 0)[0];
                k[1] = CircularShiftLeft(KB, 0)[1];
                k[2] = CircularShiftLeft(KR, 15)[0];
                k[3] = CircularShiftLeft(KR, 15)[1];
                k[4] = CircularShiftLeft(KA, 15)[0];
                k[5] = CircularShiftLeft(KA, 15)[1];
                ke[0] = CircularShiftLeft(KR, 30)[0];
                ke[1] = CircularShiftLeft(KR, 30)[1];
                k[6] = CircularShiftLeft(KB, 30)[0];
                k[7] = CircularShiftLeft(KB, 30)[1];
                k[8] = CircularShiftLeft(KL, 45)[0];
                k[9] = CircularShiftLeft(KL, 45)[1];
                k[10] = CircularShiftLeft(KA, 45)[0];
                k[11] = CircularShiftLeft(KA, 45)[1];
                ke[2] = CircularShiftLeft(KL, 60)[0];
                ke[3] = CircularShiftLeft(KL, 60)[1];
                k[12] = CircularShiftLeft(KR, 60)[0];
                k[13] = CircularShiftLeft(KR, 60)[1];
                k[14] = CircularShiftLeft(KB, 60)[0];
                k[15] = CircularShiftLeft(KB, 60)[1];
                k[16] = CircularShiftLeft(KL, 77)[0];
                k[17] = CircularShiftLeft(KL, 77)[1];
                ke[4] = CircularShiftLeft(KA, 77)[0];
                ke[5] = CircularShiftLeft(KA, 77)[1];
                k[18] = CircularShiftLeft(KR, 94)[0];
                k[19] = CircularShiftLeft(KR, 94)[1];
                k[20] = CircularShiftLeft(KA, 94)[0];
                k[21] = CircularShiftLeft(KA, 94)[1];
                k[22] = CircularShiftLeft(KL, 111)[0];
                k[23] = CircularShiftLeft(KL, 111)[1];
                kw[2] = CircularShiftLeft(KB, 111)[0];
                kw[3] = CircularShiftLeft(KB, 111)[1];
            }
        }
        private ulong[] SplitKey(ulong[] key)
        {
            if (key.Length != 2 && key.Length != 3 && key.Length != 4) throw new ArgumentException();

            ulong[] result = new ulong[4];
            result[0] = key[0];
            result[1] = key[1];


            if (key.Length == 3)
            {
                result[2] = key[2];
                result[3] = ~key[2];
            }
            if (key.Length == 4)
            {
                result[2] = key[2];
                result[3] = key[3];
            }

            return result;
        }

        private void GenerateKaKb(ulong[] key, out ulong[] KA, out ulong[] KB)
        {
            KA = new ulong[2];
            KB = new ulong[2];
            ulong[] KL = new ulong[2];
            ulong[] KR = new ulong[2];

            ulong[] Splited = SplitKey(key);
            KL[0] = Splited[0];
            KL[1] = Splited[1];
            KR[0] = Splited[2];
            KR[1] = Splited[3];

            ulong D1 = (KL[0] ^ KR[0]);
            ulong D2 = (KL[1] ^ KR[1]);
            D2 = D2 ^ F(D1, C1);
            D1 = D1 ^ F(D2, C2);
            D1 = D1 ^ (KL[0]);
            D2 = D2 ^ (KL[1]);
            D2 = D2 ^ F(D1, C3);
            D1 = D1 ^ F(D2, C4);
            KA[0] = D1;
            KA[1] = D2;
            D1 = (KA[0] ^ KR[0]);
            D2 = (KA[1] ^ KR[1]);
            D2 = D2 ^ F(D1, C5);
            D1 = D1 ^ F(D2, C6);
            KB[0] = D1;
            KB[1] = D2;
        }
        private ulong F(ulong F_IN, ulong KE)
        {
            ulong x = F_IN ^ KE;
            byte t1 =(byte) ( (x >> 56) & MASK8);
            byte t2 =(byte)((x >> 48) & MASK8);
            byte t3 =(byte)((x >> 40) & MASK8);
            byte t4 =(byte)((x >> 32) & MASK8);
            byte t5 =(byte)((x >> 24) & MASK8);
            byte t6 =(byte)((x >> 16) & MASK8);
            byte t7 =(byte)((x >> 8) & MASK8);
            byte t8 =(byte)(x & MASK8);
            t1 = SBOX1[t1];
            t2 = SBOX2(t2);
            t3 = SBOX3(t3);
            t4 = SBOX4(t4);
            t5 = SBOX2(t5);
            t6 = SBOX3(t6);
            t7 = SBOX4(t7);
            t8 = SBOX1[t8];
            ulong y1 =(byte)(t1 ^ t3 ^ t4 ^ t6 ^ t7 ^ t8);
            ulong y2 =(byte)(t1 ^ t2 ^ t4 ^ t5 ^ t7 ^ t8);
            ulong y3 =(byte)(t1 ^ t2 ^ t3 ^ t5 ^ t6 ^ t8);
            ulong y4 =(byte)(t2 ^ t3 ^ t4 ^ t5 ^ t6 ^ t7);
            ulong y5 =(byte)(t1 ^ t2 ^ t6 ^ t7 ^ t8);
            ulong y6 =(byte)(t2 ^ t3 ^ t5 ^ t7 ^ t8);
            ulong y7 =(byte)(t3 ^ t4 ^ t5 ^ t6 ^ t8);
            ulong y8 =(byte)(t1 ^ t4 ^ t5 ^ t6 ^ t7);
            return ((y1 << 56) | (y2 << 48) | (y3 << 40) | (y4 << 32) | (y5 << 24) | (y6 << 16) | (y7 << 8) | y8);
        }

        private ulong FL(ulong FL_IN, ulong KE)
        {
            uint x1 = (uint)(FL_IN >> 32);
            uint x2 = (uint)(FL_IN & MASK32);
            uint k1 = (uint)(KE >> 32);
            uint k2 = (uint)(KE & MASK32);
            x2 ^= (((x1 & k1) << 1) | ((x1 & k1) >> (32 - 1)));
            x1 ^= (x2 | k2);
            return ((((ulong)x1) << 32) | ((ulong)x2));
        }

        private ulong FLINV(ulong FLINV_IN, ulong KE)
        {
            uint y1 = (uint)(FLINV_IN >> 32);
            uint y2 = (uint)(FLINV_IN & MASK32);
            uint k1 = (uint)(KE >> 32);
            uint k2 = (uint)(KE & MASK32);
            y1 ^= (y2 | k2);
            y2 ^= (((y1 & k1) << 1) | ((y1 & k1) >> (32 - 1)));
            return ((((ulong)y1) << 32) | ((ulong)y2));

        }
    }
}
