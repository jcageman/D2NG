﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Serilog;

namespace D2NG.BNCS.Login
{
    public class CdKeySha1 : CdKey
    {
        private static readonly byte[] Translate =
        {
             0x09, 0x04, 0x07, 0x0F, 0x0D, 0x0A, 0x03, 0x0B, 0x01, 0x02, 0x0C, 0x08, 0x06, 0x0E, 0x05, 0x00,
             0x09, 0x0B, 0x05, 0x04, 0x08, 0x0F, 0x01, 0x0E, 0x07, 0x00, 0x03, 0x02, 0x0A, 0x06, 0x0D, 0x0C,
             0x0C, 0x0E, 0x01, 0x04, 0x09, 0x0F, 0x0A, 0x0B, 0x0D, 0x06, 0x00, 0x08, 0x07, 0x02, 0x05, 0x03,
             0x0B, 0x02, 0x05, 0x0E, 0x0D, 0x03, 0x09, 0x00, 0x01, 0x0F, 0x07, 0x0C, 0x0A, 0x06, 0x04, 0x08,
             0x06, 0x02, 0x04, 0x05, 0x0B, 0x08, 0x0C, 0x0E, 0x0D, 0x0F, 0x07, 0x01, 0x0A, 0x00, 0x03, 0x09,
             0x05, 0x04, 0x0E, 0x0C, 0x07, 0x06, 0x0D, 0x0A, 0x0F, 0x02, 0x09, 0x01, 0x00, 0x0B, 0x08, 0x03,
             0x0C, 0x07, 0x08, 0x0F, 0x0B, 0x00, 0x05, 0x09, 0x0D, 0x0A, 0x06, 0x0E, 0x02, 0x04, 0x03, 0x01,
             0x03, 0x0A, 0x0E, 0x08, 0x01, 0x0B, 0x05, 0x04, 0x02, 0x0F, 0x0D, 0x0C, 0x06, 0x07, 0x09, 0x00,
             0x0C, 0x0D, 0x01, 0x0F, 0x08, 0x0E, 0x05, 0x0B, 0x03, 0x0A, 0x09, 0x00, 0x07, 0x02, 0x04, 0x06,
             0x0D, 0x0A, 0x07, 0x0E, 0x01, 0x06, 0x0B, 0x08, 0x0F, 0x0C, 0x05, 0x02, 0x03, 0x00, 0x04, 0x09,
             0x03, 0x0E, 0x07, 0x05, 0x0B, 0x0F, 0x08, 0x0C, 0x01, 0x0A, 0x04, 0x0D, 0x00, 0x06, 0x09, 0x02,
             0x0B, 0x06, 0x09, 0x04, 0x01, 0x08, 0x0A, 0x0D, 0x07, 0x0E, 0x00, 0x0C, 0x0F, 0x02, 0x03, 0x05,
             0x0C, 0x07, 0x08, 0x0D, 0x03, 0x0B, 0x00, 0x0E, 0x06, 0x0F, 0x09, 0x04, 0x0A, 0x01, 0x05, 0x02,
             0x0C, 0x06, 0x0D, 0x09, 0x0B, 0x00, 0x01, 0x02, 0x0F, 0x07, 0x03, 0x04, 0x0A, 0x0E, 0x08, 0x05,
             0x03, 0x06, 0x01, 0x05, 0x0B, 0x0C, 0x08, 0x00, 0x0F, 0x0E, 0x09, 0x04, 0x07, 0x0A, 0x0D, 0x02,
             0x0A, 0x07, 0x0B, 0x0F, 0x02, 0x08, 0x00, 0x0D, 0x0E, 0x0C, 0x01, 0x06, 0x09, 0x03, 0x05, 0x04,
             0x0A, 0x0B, 0x0D, 0x04, 0x03, 0x08, 0x05, 0x09, 0x01, 0x00, 0x0F, 0x0C, 0x07, 0x0E, 0x02, 0x06,
             0x0B, 0x04, 0x0D, 0x0F, 0x01, 0x06, 0x03, 0x0E, 0x07, 0x0A, 0x0C, 0x08, 0x09, 0x02, 0x05, 0x00,
             0x09, 0x06, 0x07, 0x00, 0x01, 0x0A, 0x0D, 0x02, 0x03, 0x0E, 0x0F, 0x0C, 0x05, 0x0B, 0x04, 0x08,
             0x0D, 0x0E, 0x05, 0x06, 0x01, 0x09, 0x08, 0x0C, 0x02, 0x0F, 0x03, 0x07, 0x0B, 0x04, 0x00, 0x0A,
             0x09, 0x0F, 0x04, 0x00, 0x01, 0x06, 0x0A, 0x0E, 0x02, 0x03, 0x07, 0x0D, 0x05, 0x0B, 0x08, 0x0C,
             0x03, 0x0E, 0x01, 0x0A, 0x02, 0x0C, 0x08, 0x04, 0x0B, 0x07, 0x0D, 0x00, 0x0F, 0x06, 0x09, 0x05,
             0x07, 0x02, 0x0C, 0x06, 0x0A, 0x08, 0x0B, 0x00, 0x0F, 0x04, 0x03, 0x0E, 0x09, 0x01, 0x0D, 0x05,
             0x0C, 0x04, 0x05, 0x09, 0x0A, 0x02, 0x08, 0x0D, 0x03, 0x0F, 0x01, 0x0E, 0x06, 0x07, 0x0B, 0x00,
             0x0A, 0x08, 0x0E, 0x0D, 0x09, 0x0F, 0x03, 0x00, 0x04, 0x06, 0x01, 0x0C, 0x07, 0x0B, 0x02, 0x05,
             0x03, 0x0C, 0x04, 0x0A, 0x02, 0x0F, 0x0D, 0x0E, 0x07, 0x00, 0x05, 0x08, 0x01, 0x06, 0x0B, 0x09,
             0x0A, 0x0C, 0x01, 0x00, 0x09, 0x0E, 0x0D, 0x0B, 0x03, 0x07, 0x0F, 0x08, 0x05, 0x02, 0x04, 0x06,
             0x0E, 0x0A, 0x01, 0x08, 0x07, 0x06, 0x05, 0x0C, 0x02, 0x0F, 0x00, 0x0D, 0x03, 0x0B, 0x04, 0x09,
             0x03, 0x08, 0x0E, 0x00, 0x07, 0x09, 0x0F, 0x0C, 0x01, 0x06, 0x0D, 0x02, 0x05, 0x0A, 0x0B, 0x04,
             0x03, 0x0A, 0x0C, 0x04, 0x0D, 0x0B, 0x09, 0x0E, 0x0F, 0x06, 0x01, 0x07, 0x02, 0x00, 0x05, 0x08,
        };

        private static readonly byte[] KeyTable =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0x00, 0xFF, 0x01, 0xFF, 0x02, 0x03,
            0x04, 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B,
            0x0C, 0xFF, 0x0D, 0x0E, 0xFF, 0x0F, 0x10, 0xFF,
            0x11, 0xFF, 0x12, 0xFF, 0x13, 0xFF, 0x14, 0x15,
            0x16, 0x17, 0x18, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B,
            0x0C, 0xFF, 0x0D, 0x0E, 0xFF, 0x0F, 0x10, 0xFF,
            0x11, 0xFF, 0x12, 0xFF, 0x13, 0xFF, 0x14, 0x15,
            0x16, 0x17, 0x18, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        };

        public CdKeySha1(string key) : base(key)
        {
            KeyLength = key.Length;
            Decode();
        }
        protected static int[] BuildTableFromKey(String cdKey)
        {
            var table = new int[52];

            var b = 0x21;
            for (int i = 0; i < 26; i++)
            {
                var a = (b + 0x07B5) % 52;
                b = (a + 0x07B5) % 52;

                var key = KeyTable[cdKey[i]];
                table[a] = key / 5;
                table[b] = key % 5;
            }

            return table;
        }

        protected static int[] GenerateValues(int[] table)
        {
            var values = new long[4];

            var rounds = 4;
            var mulx = 5L;

            for (int i = 52; i > 0; i--)
            {
                var posA = rounds - 1;
                var posB = posA;
                long byt = table[i - 1];

                for (int j = 0; j < rounds; j++)
                {
                    var p1 = values[posA] & 0xFFFFFFFFL;
                    posA -= 1;

                    var p2 = mulx & 0xFFFFFFFFL;
                    var edxeax = p1 * p2;

                    values[posB] = (int)byt + (int)edxeax;
                    byt = edxeax >> 32;
                    posB -= 1;
                }
            }

            var var8 = 29;
            for (int i = 464; i > -1; i -= 16)
            {
                var esi = (var8 & 7) << 2;
                var var4 = var8 >> 3;
                var varC = (values[3 - var4] & (0x0FL << esi)) >> esi;

                if (i < 464)
                {
                    for (int j = 29; j > var8; j--)
                    {
                        varC = RecalcVarC(varC, values, i, j);
                    }
                }

                var8 -= 1;

                for (int j = var8; j > -1; j--)
                {
                    varC = RecalcVarC(varC, values, i, j);
                }

                var index = 3 - var4;
                var ebx = (Translate[varC + i] & 0x0FL) << esi;
                values[index] = (ebx | ~(0x0FL << esi) & values[index]);

            }

            return values.Select(v => (int)v).ToArray();
        }

        protected static long RecalcVarC(long varC, long[] values, int i, int j)
        {
            var ecx = ((j & 7) << 2);
            var idx = CalcIdx(j);
            var ebp = CalculateEbp(values[idx], ecx);
            return Translate[ebp ^ (Translate[varC + i] + i)];
        }

        protected static int CalcIdx(int j) => 0x03 - (j >> 3);

        protected static long CalculateEbp(long value, int ecx) => (value & (0x0FL << ecx)) >> ecx;
        
        private void Decode()
        {
            var table = BuildTableFromKey(this.Key);
            var values = GenerateValues(table);

            var valuesAsBytes = values
                .SelectMany(BitConverter.GetBytes)
                .ToArray();

            var esi = 0;
            for (byte edi = 0; edi < 120; edi++)
            {
                var eax = edi & 0x1F;
                var ecx = esi & 0x1F;
                var edx = 3 - (edi >> 5);

                var loc = 12 - ((esi >> 5) << 2);
                var ebp = BitConverter.ToInt32(valuesAsBytes, loc);
                ebp = (ebp & (1 << ecx)) >> ecx;

                values[edx] = ((ebp & 1) << eax) | (~(1 << eax) & values[edx]);

                esi += 0x0B;
                if (esi > 120)
                {
                    esi -= 120;
                }
            }

            this.Product = values[0] >> 0X0A;
            Log.Debug("[{0}] Product: 0x{1:X}", GetType(), Product);
            this.Public = BitConverter.GetBytes(((values[0] & 0x03FF) << 0x10) | (int)((uint)values[1] >> 0x10));

            var priv = new List<byte>();
            priv.Add((byte) ((values[1] & 0x00FF) >> 0));
            priv.Add((byte) ((values[1] & 0xFF00) >> 8));
            priv.AddRange(BitConverter.GetBytes(values[2]));
            priv.AddRange(BitConverter.GetBytes(values[3]));
            this.Private = priv.ToArray();
        }

        public override byte[] ComputeHash(uint clientToken, uint serverToken)
        {
            var buffer = new List<byte>();
            buffer.AddRange(BitConverter.GetBytes(clientToken));
            buffer.AddRange(BitConverter.GetBytes(serverToken));
            buffer.AddRange(BitConverter.GetBytes(Product));
            buffer.AddRange(Public);
            buffer.AddRange(Private);
            SHA1 sha = new SHA1CryptoServiceProvider();
            return sha.ComputeHash(buffer.ToArray());
        }

    }
}
