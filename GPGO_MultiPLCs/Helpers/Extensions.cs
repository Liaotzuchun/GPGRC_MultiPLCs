using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GPGO_MultiPLCs.Helpers
{
    public static class Extensions
    {
        private static readonly int[] bitnum =
        {
            1,
            2,
            4,
            8,
            16,
            32,
            64,
            128,
            256,
            512,
            1024,
            2048,
            4096,
            8192,
            16384,
            32768,
            65536,
            131072,
            262144,
            524288,
            1048576,
            2097152,
            4194304,
            8388608,
            16777216,
            33554432,
            67108864,
            134217728,
            268435456,
            536870912,
            1073741824
        };

        public static string ASCIIfromShorts(this IEnumerable<short> vals)
        {
            var bytes = new List<byte>();
            foreach (var val in vals)
            {
                bytes.AddRange(BitConverter.GetBytes(val));
            }

            var byteArray = bytes.ToArray();
            var str = Encoding.ASCII.GetString(byteArray);
            str = str.TrimEnd('\0');
            return str;
        }

        public static short[] ASCIItoShorts(this string val)
        {
            var bytes = Encoding.ASCII.GetBytes(val);
            var vals = new List<short>();

            for (var i = 0; i < bytes.Length - 1; i += 2)
            {
                vals.Add(BitConverter.ToInt16(bytes, i));
            }

            if (!(bytes.Length % 2.0 > 0.0))
            {
                return vals.ToArray();
            }

            byte[] temp = { bytes[bytes.Length - 1], 0 };
            vals.Add(BitConverter.ToInt16(temp, 0));

            return vals.ToArray();
        }

        public static int BitBytesToInt(this byte[] bits)
        {
            long val = 0;

            if (bits.Length != 32)
            {
                var temp = new byte[32];
                Array.Copy(bits, temp, bits.Length);
                bits = temp;
            }

            for (var i = 0; i < 31; i++)
            {
                if (bits[i] > 0)
                {
                    val += bitnum[i];
                }
            }

            if (bits[31] > 0)
            {
                val = val - 2147483648;
            }

            return (int)val;
        }

        public static short BitBytesToShort(this byte[] bits)
        {
            var val = 0;

            if (bits.Length != 16)
            {
                var temp = new byte[16];
                Array.Copy(bits, temp, bits.Length);
                bits = temp;
            }

            for (var i = 0; i < 15; i++)
            {
                if (bits[i] > 0)
                {
                    val += bitnum[i];
                }
            }

            if (bits[15] > 0)
            {
                val = val - 32768;
            }

            return (short)val;
        }

        public static int BitsToInt(this bool[] bits)
        {
            long val = 0;

            if (bits.Length != 32)
            {
                var temp = new bool[32];
                Array.Copy(bits, temp, bits.Length);
                bits = temp;
            }

            for (var i = 0; i < 31; i++)
            {
                if (bits[i])
                {
                    val += bitnum[i];
                }
            }

            if (bits[31])
            {
                val = val - 2147483648;
            }

            return (int)val;
        }

        public static short BitsToShort(this bool[] bits)
        {
            var val = 0;

            if (bits.Length != 16)
            {
                var temp = new bool[16];
                Array.Copy(bits, temp, bits.Length);
                bits = temp;
            }

            for (var i = 0; i < 15; i++)
            {
                if (bits[i])
                {
                    val += bitnum[i];
                }
            }

            if (bits[15])
            {
                val = val - 32768;
            }

            return (short)val;
        }

        public static string BytesToHex(this byte[] val)
        {
            return BitConverter.ToString(val);
        }

        public static void CopyAll<T>(T source, T target)
        {
            var type = typeof(T);
            foreach (var sourceProperty in type.GetProperties())
            {
                var targetProperty = type.GetProperty(sourceProperty.Name);
                if (targetProperty != null && targetProperty.CanWrite)
                {
                    targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
                }
            }

            foreach (var sourceField in type.GetFields())
            {
                var targetField = type.GetField(sourceField.Name);
                if (!targetField.IsInitOnly)
                {
                    targetField.SetValue(target, sourceField.GetValue(source));
                }
            }
        }

        public static byte HexToByte(this string val)
        {
            return Convert.ToByte(val, 16);
        }

        public static byte[] HexToBytes(this string[] val)
        {
            return val.Select(x => Convert.ToByte(x, 16)).ToArray();
        }

        public static int HexToInt(this string val)
        {
            return int.Parse(val, NumberStyles.HexNumber);
        }

        public static byte[] IntToBitBytes(this int val)
        {
            var _val = (long)val;
            var bits = new byte[32];

            if (_val < 0)
            {
                bits[31] = 1;
                _val += 2147483648;
            }

            for (var i = 30; i >= 0; i--)
            {
                if ((_val & bitnum[i]) > 0)
                {
                    bits[i] = 1;
                }
            }

            return bits;
        }

        public static bool[] IntToBits(this int val)
        {
            var _val = (long)val;
            var bits = new bool[32];

            if (_val < 0)
            {
                bits[31] = true;
                _val += 2147483648;
            }

            for (var i = 30; i >= 0; i--)
            {
                if ((_val & bitnum[i]) > 0)
                {
                    bits[i] = true;
                }
            }

            return bits;
        }

        public static int ReadShortsToInt(this (short, short) val)
        {
            var byte1 = BitConverter.GetBytes(val.Item1);
            var byte2 = BitConverter.GetBytes(val.Item2);
            var byte3 = new[] { byte1[0], byte1[1], byte2[0], byte2[1] };
            var final = BitConverter.ToInt32(byte3, 0);
            return final;
        }

        public static byte[] ShortToBitBytes(this short val)
        {
            var _val = (int)val;
            var bits = new byte[16];

            if (_val < 0)
            {
                bits[15] = 1;
                _val += 32768;
            }

            for (var i = 14; i >= 0; i--)
            {
                if ((_val & bitnum[i]) > 0)
                {
                    bits[i] = 1;
                }
            }

            return bits;
        }

        public static bool[] ShortToBits(this short val)
        {
            var _val = (int)val;
            var bits = new bool[16];

            if (_val < 0)
            {
                bits[15] = true;
                _val += 32768;
            }

            for (var i = 14; i >= 0; i--)
            {
                if ((_val & bitnum[i]) > 0)
                {
                    bits[i] = true;
                }
            }

            return bits;
        }

        public static string UTF8fromShorts(this IEnumerable<short> vals)
        {
            var bytes = new List<byte>();
            foreach (var val in vals)
            {
                bytes.AddRange(BitConverter.GetBytes(val));
            }

            var byteArray = bytes.ToArray();
            var str = Encoding.UTF8.GetString(byteArray);
            str = str.TrimEnd('\0');
            return str;
        }

        public static short[] UTF8toShorts(this string val)
        {
            var bytes = Encoding.UTF8.GetBytes(val);
            var vals = new List<short>();

            for (var i = 0; i < bytes.Length - 1; i += 2)
            {
                vals.Add(BitConverter.ToInt16(bytes, i));
            }

            if (!(bytes.Length % 2.0 > 0.0))
            {
                return vals.ToArray();
            }

            byte[] temp = { bytes[bytes.Length - 1], 0 };
            vals.Add(BitConverter.ToInt16(temp, 0));

            return vals.ToArray();
        }

        public static (short, short) WriteIntToShorts(this int value)
        {
            var temp = ((short)0, (short)0);
            var byarrBufferByte = BitConverter.GetBytes(value);
            temp.Item1 = BitConverter.ToInt16(byarrBufferByte, 0);
            temp.Item2 = BitConverter.ToInt16(byarrBufferByte, 2);
            return temp;
        }
    }
}