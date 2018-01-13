﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Drawing;

namespace HslCommunication.BasicFramework
{
    /// <summary>
    /// 一个软件基础类，提供常用的一些静态方法
    /// </summary>
    public class SoftBasic
    {
        #region MD5码计算块


        /// <summary>
        /// 获取文件的md5码
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string CalculateFileMD5(string filePath)
        {
            string str_md5 = string.Empty;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                str_md5 = CalculateStreamMD5(fs);
            }
            return str_md5;
        }

        /// <summary>
        /// 获取数据流的md5码
        /// </summary>
        /// <param name="stream">数据流，可以是内存流，也可以是文件流</param>
        /// <returns></returns>
        public static string CalculateStreamMD5(Stream stream)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes_md5 = md5.ComputeHash(stream);
            return BitConverter.ToString(bytes_md5).Replace("-", "");
        }
        

        #endregion

        #region 数据大小相关


        /// <summary>
        /// 从一个字节大小返回带单位的描述
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetSizeDescription(long size)
        {
            if (size < 1000)
            {
                return size + " B";
            }
            else if (size < 1000 * 1000)
            {
                float data = (float)size / 1024;
                return data.ToString("F2") + " Kb";
            }
            else if (size < 1000 * 1000 * 1000)
            {
                float data = (float)size / 1024 / 1024;
                return data.ToString("F2") + " Mb";
            }
            else
            {
                float data = (float)size / 1024 / 1024 / 1024;
                return data.ToString("F2") + " Gb";
            }
        }


        #endregion

        #region 枚举相关块


        /// <summary>
        /// 获取一个枚举类型的所有枚举值，可直接应用于组合框数据
        /// </summary>
        /// <typeparam name="TEnum">枚举的类型值</typeparam>
        /// <returns>枚举值数组</returns>
        public static TEnum[] GetEnumValues<TEnum>() where TEnum : struct
        {
            return (TEnum[])Enum.GetValues(typeof(TEnum));
        }

        #endregion
        

        #region 异常错误信息格式化
        


        /// <summary>
        /// 获取一个异常的完整错误信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string GetExceptionMessage(Exception ex)
        {
            return StringResources.ExceptionMessage + ex.Message + Environment.NewLine +
                StringResources.ExceptionStackTrace + ex.StackTrace + Environment.NewLine +
                StringResources.ExceptopnTargetSite + ex.TargetSite;
        }

        /// <summary>
        /// 获取一个异常的完整错误信息，和额外的字符串描述信息
        /// </summary>
        /// <param name="extraMsg">额外的信息</param>
        /// <param name="ex">异常对象</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string GetExceptionMessage(string extraMsg, Exception ex)
        {
            if (string.IsNullOrEmpty(extraMsg))
            {
                return GetExceptionMessage(ex);
            }
            else
            {
                return extraMsg + Environment.NewLine + GetExceptionMessage(ex);
            }
        }


        #endregion

        #region Hex字符串和Byte[]相互转化块


        /// <summary>
        /// 字节数据转化成16进制表示的字符串
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string ByteToHexString(byte[] InBytes)
        {
            return ByteToHexString(InBytes, (char)0);
        }

        /// <summary>
        /// 字节数据转化成16进制表示的字符串
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <param name="segment">分割符</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string ByteToHexString(byte[] InBytes, char segment)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte InByte in InBytes)
            {
                if (segment == 0) sb.Append(string.Format("{0:X2}", InByte));
                else sb.Append(string.Format("{0:X2}{1}", InByte, segment));
            }

            if (segment != 0 && sb.Length > 1 && sb[sb.Length - 1] == segment)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }



        /// <summary>
        /// 字符串数据转化成16进制表示的字符串
        /// </summary>
        /// <param name="InString">输入的字符串数据</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string ByteToHexString(string InString)
        {
            return ByteToHexString(Encoding.Unicode.GetBytes(InString));
        }

        /// <summary>
        /// 将16进制的字符串转化成Byte数据，将检测每2个字符转化，也就是说，中间可以是任意字符
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string hex)
        {
            hex = hex.ToUpper();
            List<char> data = new List<char>()
            {
                '0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'
            };

            MemoryStream ms = new MemoryStream();

            for (int i = 0; i < hex.Length; i++)
            {
                if ((i + 1) < hex.Length)
                {
                    if (data.Contains(hex[i]) && data.Contains(hex[i + 1]))
                    {
                        // 这是一个合格的字节数据
                        ms.WriteByte((byte)(data.IndexOf(hex[i]) * 16 + data.IndexOf(hex[i + 1])));
                        i++;
                    }
                }
            }

            byte[] result = ms.ToArray();
            ms.Dispose();
            return result;
        }

        #endregion

        #region Bool[]数组和byte[]相互转化块


        /// <summary>
        /// 将bool数组转换到byte数组
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static byte[] BoolArrayToByte(bool[] array)
        {
            if (array == null) return null;

            int length = array.Length % 8 == 0 ? array.Length / 8 : array.Length / 8 + 1;
            byte[] buffer = new byte[length];

            for (int i = 0; i < array.Length; i++)
            {
                int index = i / 8;
                int offect = i % 8;

                byte temp = 0;
                switch (offect)
                {
                    case 0: temp = 0x01; break;
                    case 1: temp = 0x02; break;
                    case 2: temp = 0x04; break;
                    case 3: temp = 0x08; break;
                    case 4: temp = 0x10; break;
                    case 5: temp = 0x20; break;
                    case 6: temp = 0x40; break;
                    case 7: temp = 0x80; break;
                    default: break;
                }

                if (array[i]) buffer[index] += temp;
            }

            return buffer;
        }

        /// <summary>
        /// 从Byte数组中提取位数组
        /// </summary>
        /// <param name="InBytes">原先的字节数组</param>
        /// <param name="length">想要转换的长度，如果超出自动会缩小到数组最大长度</param>
        /// <returns></returns>
        public static bool[] ByteToBoolArray(byte[] InBytes, int length)
        {
            if (InBytes == null) return null;

            if (length > InBytes.Length * 8) length = InBytes.Length * 8;
            bool[] buffer = new bool[length];

            for (int i = 0; i < length; i++)
            {
                int index = i / 8;
                int offect = i % 8;

                byte temp = 0;
                switch (offect)
                {
                    case 0: temp = 0x01; break;
                    case 1: temp = 0x02; break;
                    case 2: temp = 0x04; break;
                    case 3: temp = 0x08; break;
                    case 4: temp = 0x10; break;
                    case 5: temp = 0x20; break;
                    case 6: temp = 0x40; break;
                    case 7: temp = 0x80; break;
                    default: break;
                }

                if ((InBytes[index] & temp) == temp)
                {
                    buffer[i] = true;
                }
            }

            return buffer;
        }


        #endregion

        #region 基础框架块

        /// <summary>
        /// 设置或获取系统框架的版本号
        /// </summary>
        public static SystemVersion FrameworkVersion { get; set; } = new SystemVersion("1.0.2");


        #endregion

        #region 深度克隆对象

        /// <summary>
        /// 使用序列化反序列化深度克隆一个对象
        /// </summary>
        /// <param name="oringinal"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static object DeepClone(object oringinal)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter()
                {
                    Context = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.Clone)
                };
                formatter.Serialize(stream, oringinal);
                stream.Position = 0;
                return formatter.Deserialize(stream);
            }
        }


        #endregion
    }

}
