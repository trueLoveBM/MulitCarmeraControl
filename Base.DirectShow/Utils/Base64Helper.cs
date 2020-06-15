using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.DirectShow.Utils
{
    /// <summary>
    /// Base64加密帮助类
    /// </summary>
    public sealed class Base64Helper
    {
        /// <summary>
        /// Base64加密一个文本文件的内容
        /// </summary>
        /// <param name="FileUrl">文件url</param>
        public static void Base64Encode4txtFile(string FileUrl)
        {
            //读取文件的所有文本内容
            string text = System.IO.File.ReadAllText(FileUrl, Encoding.UTF8);
            //将文本内容加密
            var enCodedText = Base64Encode(Encoding.UTF8, text);
            //将加密后的文本保存到文件中
            System.IO.File.WriteAllText(FileUrl, enCodedText, Encoding.UTF8);
        }

        /// <summary>
        /// 解密一个文本文件内容
        /// </summary>
        /// <param name="FileUrl">文件url</param>
        public static void Base64Decode4txtFile(string FileUrl)
        {
            //读取文件的所有文本内容
            string text = System.IO.File.ReadAllText(FileUrl, Encoding.UTF8);
            //将文本内容解密
            var DecodedText = Base64Decode(Encoding.UTF8, text);
            //将解密后的文本保存到文件中
            System.IO.File.WriteAllText(FileUrl, DecodedText, Encoding.UTF8);
        }


        /// <summary>
        /// Base64加密，采用utf8编码方式加密
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <returns>加密后的字符串</returns>
        public static string Base64Encode(string source)
        {
            return Base64Encode(Encoding.UTF8, source);
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="encodeType">加密采用的编码方式</param>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        public static string Base64Encode(Encoding encodeType, string source)
        {
            string encode = string.Empty;
            byte[] bytes = encodeType.GetBytes(source);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }

        /// <summary>
        /// Base64解密，采用utf8编码方式解密
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Base64Decode(string result)
        {
            return Base64Decode(Encoding.UTF8, result);
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="encodeType">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Base64Decode(Encoding encodeType, string result)
        {
            string decode = string.Empty;
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encodeType.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }
    }
}
