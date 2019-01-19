﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using System.Xml.Linq;
using GPGO_MultiPLCs.Models;
using Newtonsoft.Json;

namespace GPGO_MultiPLCs.Helpers
{
    /// <summary>各種擴充</summary>
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

        /// <summary>字串靠中央對齊</summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <param name="Lng"></param>
        /// <returns></returns>
        public static string AlignCenter(this string source, int length, Language Lng)
        {
            source = source.Trim();
            var source_length = Encoding.GetEncoding(Lng == Language.CHS ? "GB2312" : "Big5").GetByteCount(source);
            var spaces = length - source_length;
            var d = spaces / 2 + source_length;

            return source.AlignRight(d, Lng).AlignLeft(length, Lng);
        }

        /// <summary>字串靠左對齊</summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <param name="Lng"></param>
        /// <returns></returns>
        public static string AlignLeft(this string source, int length, Language Lng)
        {
            source = source.Trim();
            var source_length = Encoding.GetEncoding(Lng == Language.CHS ? "GB2312" : "Big5").GetByteCount(source);

            return source.PadRight(length - (source_length - source.Length));
        }

        /// <summary>字串靠右對齊</summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <param name="Lng"></param>
        /// <returns></returns>
        public static string AlignRight(this string source, int length, Language Lng)
        {
            source = source.Trim();
            var source_length = Encoding.GetEncoding(Lng == Language.CHS ? "GB2312" : "Big5").GetByteCount(source);

            return source.PadLeft(length - (source_length - source.Length));
        }

        /// <summary>物件深層拷貝，產生一新物件(僅限public屬性和field)</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        public static T Copy<T>(this T source) where T : new()
        {
            var type = typeof(T);
            var target = new T();

            foreach (var property in type.GetProperties())
            {
                if (property.CanWrite)
                {
                    property.SetValue(target, property.GetValue(source, null), null);
                }
            }

            foreach (var Field in type.GetFields())
            {
                if (!Field.IsInitOnly)
                {
                    Field.SetValue(target, Field.GetValue(source));
                }
            }

            return target;
        }

        /// <summary>物件同名屬性值複製，產生一新物件(僅限public屬性和field)</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        public static T Copy<T>(this object source) where T : new()
        {
            var type1 = source.GetType();
            var type2 = typeof(T);
            var target = new T();
            foreach (var targetProperty in type2.GetProperties())
            {
                var sourceProperty = type1.GetProperty(targetProperty.Name);
                if (sourceProperty != null && sourceProperty.CanRead)
                {
                    targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
                }
            }

            foreach (var targetField in type2.GetFields())
            {
                var sourceField = type1.GetField(targetField.Name);
                if (!sourceField.IsInitOnly)
                {
                    targetField.SetValue(target, sourceField.GetValue(source));
                }
            }

            return target;
        }

        /// <summary>從目標物件嘗試拷貝同屬性名的值(僅限public屬性和field)</summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyFrom<T1>(this T1 source, object target) where T1 : class
        {
            var type1 = typeof(T1);
            var type2 = target.GetType();
            foreach (var sourceProperty in type1.GetProperties())
            {
                var targetProperty = type2.GetProperty(sourceProperty.Name);
                if (targetProperty != null && targetProperty.CanRead)
                {
                    sourceProperty.SetValue(source, targetProperty.GetValue(target, null), null);
                }
            }

            foreach (var sourceField in type1.GetFields())
            {
                var targetField = type2.GetField(sourceField.Name);
                if (!targetField.IsInitOnly)
                {
                    sourceField.SetValue(source, targetField.GetValue(target));
                }
            }
        }

        /// <summary>物件深層拷貝(僅限public屬性和field)</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyTo<T>(this T source, T target) where T : class
        {
            var type = typeof(T);

            foreach (var property in type.GetProperties())
            {
                if (property.CanWrite)
                {
                    property.SetValue(target, property.GetValue(source, null), null);
                }
            }

            foreach (var Field in type.GetFields())
            {
                if (!Field.IsInitOnly)
                {
                    Field.SetValue(target, Field.GetValue(source));
                }
            }
        }

        /// <summary>嘗試拷貝同屬性名的值至目標物件(僅限public屬性和field)</summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyTo<T1>(this T1 source, object target) where T1 : class
        {
            var type1 = typeof(T1);
            var type2 = target.GetType();
            foreach (var sourceProperty in type1.GetProperties())
            {
                var targetProperty = type2.GetProperty(sourceProperty.Name);
                if (targetProperty != null && targetProperty.CanWrite)
                {
                    targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
                }
            }

            foreach (var sourceField in type1.GetFields())
            {
                var targetField = type2.GetField(sourceField.Name);
                if (!targetField.IsInitOnly)
                {
                    targetField.SetValue(target, sourceField.GetValue(source));
                }
            }
        }

        /// <summary>傳回Queue(FIFO)的指定數量元素</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static IEnumerable<T> Dequeue<T>(this Queue<T> queue, int chunkSize)
        {
            for (var i = 0; i < chunkSize && queue.Count > 0; i++)
            {
                yield return queue.Dequeue();
            }
        }

        /// <summary>傳回所有符合條件的位置index</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vals"></param>
        /// <param name="val"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static int[] FindIndexes<T>(this IEnumerable<T> vals, T val, Predicate<T> func = null)
        {
            return vals.Select((x, i) => func?.Invoke(val) ?? x.Equals(val) ? i : -1).Where(x => x != -1).ToArray();
        }

        /// <summary>float數(32位元)轉換至4個short值</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static (short, short) FloatToShorts(this float value)
        {
            var temp = ((short)0, (short)0);
            var byarrBufferByte = BitConverter.GetBytes(value);
            temp.Item1 = BitConverter.ToInt16(byarrBufferByte, 0);
            temp.Item2 = BitConverter.ToInt16(byarrBufferByte, 2);

            return temp;
        }

        /// <summary>由HDV產生RGB Color</summary>
        /// <param name="hue">色相，0~1</param>
        /// <param name="sat">飽和度，0~1</param>
        /// <param name="val">亮度，0~1</param>
        /// <returns></returns>
        public static Color FromHsv(double hue, double sat, double val)
        {
            double g, b;
            var r = g = b = 0;

            if (sat.Equals(0))
            {
                r = g = b = val;
            }
            else
            {
                if (hue.Equals(1))
                {
                    hue = 0;
                }

                hue *= 6.0;
                var i = (int)Math.Floor(hue);
                var f = hue - i;
                var aa = val * (1 - sat);
                var bb = val * (1 - sat * f);
                var cc = val * (1 - sat * (1 - f));

                switch (i)
                {
                    case 0:
                        r = val;
                        g = cc;
                        b = aa;

                        break;
                    case 1:
                        r = bb;
                        g = val;
                        b = aa;

                        break;
                    case 2:
                        r = aa;
                        g = val;
                        b = cc;

                        break;
                    case 3:
                        r = aa;
                        g = bb;
                        b = val;

                        break;
                    case 4:
                        r = cc;
                        g = aa;
                        b = val;

                        break;
                    case 5:
                        r = val;
                        g = aa;
                        b = bb;

                        break;
                }
            }

            return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        /// <summary>將Excel欄序數轉為整數</summary>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public static int GetColumnNmuber(this string ColumnName)
        {
            return ColumnName.Trim().Select(c => Convert.ToInt32(c) - Convert.ToInt32("A"[0]) + 1).Aggregate((m, n) => m * 26 + n);
        }

        /// <summary>將欄序整數轉換至Excel的欄序數，1=A，2=B，Z=26...</summary>
        /// <param name="ColumnNumber">欄數</param>
        /// <returns></returns>
        public static string GetExcelColumnName(this int ColumnNumber)
        {
            var dividend = ColumnNumber;
            var columnName = string.Empty;

            while (dividend > 0)
            {
                var modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }

        /// <summary>依據Language設定值取得Property對應的Attribute值</summary>
        /// <param name="info"></param>
        /// <param name="lng">語系</param>
        /// <returns></returns>
        public static string GetName(this PropertyInfo info, Language lng)
        {
            return info.IsDefined(typeof(LanguageTranslator), false) ?
                       ((LanguageTranslator)info.GetCustomAttributes(typeof(LanguageTranslator), false).First()).GetName(lng) is string Name && !string.IsNullOrEmpty(Name) ? Name :
                       info.Name : info.Name;
        }

        /// <summary>Json轉XML</summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static XDocument JsonToXml(this string json)
        {
            return JsonConvert.DeserializeXNode(json, "Root");
        }

        /// <summary>Json轉XML</summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string JsonToXmlString(this string json)
        {
            return JsonConvert.DeserializeXNode(json, "Root").ToString();
        }

        /// <summary>過濾字串，只保留ASCII可辨識字元</summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string OnlyASCII(this string val)
        {
            return string.Concat(val.Where(x => x >= 32 && x <= 127)).Trim();
        }

        /// <summary>過濾字串，只保留Unicode可辨識字元</summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string OnlyPrintable(this string val)
        {
            return string.Concat(val.Where(x => char.IsLetterOrDigit(x) || char.IsPunctuation(x) || char.IsSymbol(x) || char.IsWhiteSpace(x))).Trim();
        }

        /// <summary>字串置中</summary>
        /// <param name="source"></param>
        /// <param name="length">總長度</param>
        /// <returns></returns>
        public static string PadCenter(this string source, int length)
        {
            var spaces = length - source.Length;
            var padLeft = spaces / 2 + source.Length;

            return source.PadLeft(padLeft).PadRight(length);
        }

        /// <summary>將json檔案反序列化</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">檔案路徑</param>
        /// <returns></returns>
        public static T ReadFromJsonFile<T>(this string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(File.ReadAllText(path, Encoding.UTF8));
                }
                catch
                {
                    return default(T);
                }
            }

            return default(T);
        }

        /// <summary>計算標準差</summary>
        /// <param name="vals"></param>
        /// <returns></returns>
        public static double StandardDeviation(this ICollection<double> vals)
        {
            var average = vals.Average();
            var sumOfSquaresOfDifferences = vals.Select(val => (val - average) * (val - average)).Sum();

            return Math.Sqrt(sumOfSquaresOfDifferences / vals.Count);
        }

        /// <summary>計算標準差</summary>
        /// <param name="vals"></param>
        /// <returns></returns>
        public static double StandardDeviation(this ICollection<int> vals)
        {
            var average = vals.Average();
            var sumOfSquaresOfDifferences = vals.Select(val => (val - average) * (val - average)).Sum();

            return Math.Sqrt(sumOfSquaresOfDifferences / vals.Count);
        }

        /// <summary>取得日期的當週起始日</summary>
        /// <param name="dt"></param>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;

            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>將物件序列化為XML</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string To_Json(this object data)
        {
            return JsonConvert.SerializeObject(data is IEnumerable ? new { Row = data } : data, Formatting.Indented);
        }

        /// <summary>將集合物件輸出至CSV(僅public property)</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="lng">欄位語系</param>
        /// <param name="properties">自訂屬性順序</param>
        /// <returns></returns>
        public static string ToCSV<T>(this IEnumerable<T> data, Language lng, PropertyInfo[] properties = null)
        {
            var csv = new StringBuilder();
            if (properties == null)
            {
                properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanRead && !typeof(ICollection).IsAssignableFrom(x.PropertyType)).ToArray();
            }

            csv.Append(string.Join(",", properties.Select(x => x.GetName(lng))));

            foreach (var val in data)
            {
                csv.AppendLine();
                csv.Append(string.Join(",", properties.Select(x => x.GetValue(val))));
            }

            return csv.ToString();
        }

        /// <summary>將物件的公開屬性轉換為Dictionary</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="Lng">欄位語系</param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary<T>(this T source, Language Lng = Language.TW)
        {
            var type = source.GetType();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanWrite && !typeof(ICollection).IsAssignableFrom(x.PropertyType)).ToArray();
            if (Attribute.IsDefined(type, typeof(OrderedObject)))
            {
                properties = properties.OrderBy(x => x.IsDefined(typeof(OrderIndex), false) ? ((OrderIndex)x.GetCustomAttributes(typeof(OrderIndex), false).First()).Index : int.MaxValue).ToArray();
            }

            return properties.ToDictionary(prop => prop.GetName(Lng), prop => prop.GetValue(source));
        }

        /// <summary>轉換至Json(Json.Net)</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToJsonString(this object data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        /// <summary>將物件序列化為XML</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static XDocument ToXml(this object data)
        {
            var IsCollection = data is IEnumerable;
            var json = JsonConvert.SerializeObject(IsCollection ? new { Row = data } : data, Formatting.None);

            return JsonConvert.DeserializeXNode(json, IsCollection ? "Table" : "Root");
        }

        /// <summary>將物件序列化至json並輸出檔案</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="path">檔案輸出路徑</param>
        /// <returns></returns>
        public static bool WriteToJsonFile<T>(this T val, [CallerMemberName] string path = "")
        {
            if (val == null || string.IsNullOrEmpty(path))
            {
                return false;
            }

            if (!(path.EndsWith(".json") || path.EndsWith(".Json") || path.EndsWith(".JSON")))
            {
                path += ".json";
            }

            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(val, Formatting.Indented), Encoding.UTF8);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>XML轉Json</summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string XmlToJson(this string xml)
        {
            return JsonConvert.SerializeXNode(XDocument.Parse(xml), Formatting.None, true);
        }

        /// <summary>XML轉Json</summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string XmlToJson(this XObject xml)
        {
            return JsonConvert.SerializeXNode(xml, Formatting.None, true);
        }

        #region 數據轉換

        /// <summary>short陣列轉成ASCII字串</summary>
        /// <param name="vals"></param>
        /// <returns></returns>
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

        /// <summary>ASCII字串轉換成short陣列</summary>
        /// <param name="val"></param>
        /// <returns></returns>
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

        /// <summary>bit陣列(只有0/1的byte陣列，長度32)轉換至int整數</summary>
        /// <param name="bits"></param>
        /// <returns></returns>
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

        /// <summary>bit陣列(只有0/1的byte陣列，長度16)轉換至short整數</summary>
        /// <param name="bits"></param>
        /// <returns></returns>
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

        /// <summary>bit陣列(boolean陣列，長度32)轉換至int整數</summary>
        /// <param name="bits"></param>
        /// <returns></returns>
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

        /// <summary>bit陣列(boolean陣列，長度16)轉換至short整數</summary>
        /// <param name="bits"></param>
        /// <returns></returns>
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

        /// <summary>byte陣列轉換至16進位字串</summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string BytesToHex(this byte[] val)
        {
            return BitConverter.ToString(val);
        }

        /// <summary>double數(64位元)轉換至4個short值</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static (short, short, short, short) DoubleToShorts(this double value)
        {
            var temp = ((short)0, (short)0, (short)0, (short)0);
            var byarrBufferByte = BitConverter.GetBytes(value);
            temp.Item1 = BitConverter.ToInt16(byarrBufferByte, 0);
            temp.Item2 = BitConverter.ToInt16(byarrBufferByte, 2);
            temp.Item3 = BitConverter.ToInt16(byarrBufferByte, 4);
            temp.Item4 = BitConverter.ToInt16(byarrBufferByte, 6);

            return temp;
        }

        public static byte HexToByte(this string val)
        {
            return Convert.ToByte(val.Trim(), 16);
        }

        /// <summary>16進位字串轉換至byte陣列</summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte[] HexToBytes(this string[] val)
        {
            return val.Select(x => Convert.ToByte(x.Trim(), 16)).ToArray();
        }

        /// <summary>16進位字串轉換成整int數</summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int HexToInt(this string val)
        {
            return int.Parse(val.Trim(), NumberStyles.HexNumber);
        }

        /// <summary>int整數轉換至bit陣列(0/1的byte陣列，長度32)</summary>
        /// <param name="val"></param>
        /// <returns></returns>
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

        /// <summary>int整數轉換至bit陣列(boolean陣列，長度32)</summary>
        /// <param name="val"></param>
        /// <returns></returns>
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

        /// <summary>int整數轉換至2個short值</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static (short, short) IntToShorts(this int value)
        {
            var temp = ((short)0, (short)0);
            var byarrBufferByte = BitConverter.GetBytes(value);
            temp.Item1 = BitConverter.ToInt16(byarrBufferByte, 0);
            temp.Item2 = BitConverter.ToInt16(byarrBufferByte, 2);

            return temp;
        }

        /// <summary>4個short值轉double數</summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double ShortsToDouble(this (short, short, short, short) val)
        {
            var byte1 = BitConverter.GetBytes(val.Item1);
            var byte2 = BitConverter.GetBytes(val.Item2);
            var byte3 = BitConverter.GetBytes(val.Item3);
            var byte4 = BitConverter.GetBytes(val.Item4);
            var bytes = new[] { byte1[0], byte1[1], byte2[0], byte2[1], byte3[0], byte3[1], byte4[0], byte4[1] };
            var final = BitConverter.ToDouble(bytes, 0);

            return final;
        }

        /// <summary>2個short值轉float數</summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float ShortsToFloat(this (short, short) val)
        {
            var byte1 = BitConverter.GetBytes(val.Item1);
            var byte2 = BitConverter.GetBytes(val.Item2);
            var bytes = new[] { byte1[0], byte1[1], byte2[0], byte2[1] };
            var final = BitConverter.ToSingle(bytes, 0);

            return final;
        }

        /// <summary>2個short值轉int整數</summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int ShortsToInt(this (short, short) val)
        {
            var byte1 = BitConverter.GetBytes(val.Item1);
            var byte2 = BitConverter.GetBytes(val.Item2);
            var bytes = new[] { byte1[0], byte1[1], byte2[0], byte2[1] };
            var final = BitConverter.ToInt32(bytes, 0);

            return final;
        }

        /// <summary>short轉換至bit陣列(0/1的byte陣列)</summary>
        /// <param name="val"></param>
        /// <returns></returns>
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

        /// <summary>short轉換至bit陣列(boolean陣列，長度16)</summary>
        /// <param name="val"></param>
        /// <returns></returns>
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

        /// <summary>short陣列轉換至UTF8字串</summary>
        /// <param name="vals"></param>
        /// <returns></returns>
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

        /// <summary>UTF8字串轉換至short陣列</summary>
        /// <param name="val"></param>
        /// <returns></returns>
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

        #endregion
    }
}