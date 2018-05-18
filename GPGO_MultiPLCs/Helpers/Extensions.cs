using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GPGO_MultiPLCs.Helpers
{
    public static class Extensions
    {
        private static readonly short[] bitnum = { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };

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

        public static short BitBytesToShort(this byte[] bits)
        {
            short val = 0;

            if (bits.Length != 16)
            {
                return val;
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
                val = (short)(val - 32768);
            }

            return val;
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

        public static short BitsToShort(this bool[] bits)
        {
            short val = 0;

            if (bits.Length != 16)
            {
                return val;
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
                val = (short)(val - 32768);
            }

            return val;
        }

        public static string ASCIIfromShorts(this short[] vals)
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

        public static string UTF8fromShorts(this short[] vals)
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

        public static int ReadShortsToInt(this (short, short) val)
        {
            var byte1 = BitConverter.GetBytes(val.Item1);
            var byte2 = BitConverter.GetBytes(val.Item2);
            var byte3 = new[] { byte1[0], byte1[1], byte2[0], byte2[1] };
            var final = BitConverter.ToInt32(byte3, 0);
            return final;
        }

        public static int HexToInt(this string val)
        {
            return int.Parse(val, NumberStyles.HexNumber);
        }

        public static string BytesToHex(this byte[] val)
        {
            return BitConverter.ToString(val);
        }

        public static byte HexToByte(this string val)
        {
            return Convert.ToByte(val, 16);
        }

        public static byte[] HexToBytes(this string[] val)
        {
            return val.Select(x => Convert.ToByte(x, 16)).ToArray();
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
    }
}