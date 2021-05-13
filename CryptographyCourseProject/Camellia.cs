using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CryptographyCourseProject
{
    class Camellia
    {
        public readonly ulong MASK8 = 0xff;
        public readonly ulong MASK32 = 0xffffffff;
        public readonly ulong MASK64 = 0xffffffffffffffff;

        public readonly ulong c1 = 0xA09E667F3BCC908B;
        public readonly ulong c2 = 0xB67AE8584CAA73B2;
        public readonly ulong c3 = 0xC6EF372FE94F82BE;
        public readonly ulong c4 = 0x54FF53A5F1D36F1C;
        public readonly ulong c5 = 0x10E527FADE682D1D;
        public readonly ulong c6 = 0xB05688C2B3E6C1FD;

        private ulong[] SBOX1 = {

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

        public ulong SBOX2(ulong x)
        {
            return CircularShiftLeft(SBOX1[x], 1);
        }

        public ulong SBOX3(ulong x)
        {
            return CircularShiftLeft(SBOX1[x], 7);
        }

        public ulong SBOX4(ulong x)
        {
            return SBOX1[CircularShiftLeft(x, 1)];
        }

        public ulong CircularShiftLeft(ulong value, int count)
        {
            return value << count | value >> (64 - count);
        }

        private ulong[] SplitKey(ulong[] key)
        {
            if (key.Length != 2 && key.Length != 3 && key.Length != 4) throw new ArgumentException();

            ulong[] result = new ulong[4];
            result[0] = key[0];
            result[1] = key[1];

            if (key.Length == 2)
            {
                
            }
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

        private ulong F(ulong F_IN, ulong KE)
        {
           ulong x = F_IN ^ KE;
           ulong t1 = x >> 56;
           ulong t2 = (x >> 48) & MASK8;
           ulong t3 = (x >> 40) & MASK8;
           ulong t4 = (x >> 32) & MASK8;
           ulong t5 = (x >> 24) & MASK8;
           ulong t6 = (x >> 16) & MASK8;
           ulong t7 = (x >> 8) & MASK8;
           ulong t8 = x & MASK8;
            t1 = SBOX1[t1];
            t2 = SBOX2(t2);
            t3 = SBOX3(t3);
            t4 = SBOX4(t4);
            t5 = SBOX2(t5);
            t6 = SBOX3(t6);
            t7 = SBOX4(t7);
            t8 = SBOX1[t8];
            ulong y1 = t1 ^ t3 ^ t4 ^ t6 ^ t7 ^ t8;
            ulong y2 = t1 ^ t2 ^ t4 ^ t5 ^ t7 ^ t8;
            ulong y3 = t1 ^ t2 ^ t3 ^ t5 ^ t6 ^ t8;
            ulong y4 = t2 ^ t3 ^ t4 ^ t5 ^ t6 ^ t7;
            ulong y5 = t1 ^ t2 ^ t6 ^ t7 ^ t8;
            ulong y6 = t2 ^ t3 ^ t5 ^ t7 ^ t8;
            ulong y7 = t3 ^ t4 ^ t5 ^ t6 ^ t8;
            ulong y8 = t1 ^ t4 ^ t5 ^ t6 ^ t7;
            return (y1 << 56) | (y2 << 48) | (y3 << 40) | (y4 << 32) | (y5 << 24) | (y6 << 16) | (y7 << 8) | y8;
        }
    }
}
