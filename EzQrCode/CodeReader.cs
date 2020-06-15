using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.Multi.QrCode;

namespace EzQrCode
{
    /// <summary>
    /// 二维码/条形码解析器
    /// </summary>
    public class CodeReader
    {
        /// <summary>
        /// 从指定的图片解析二维码/条形码
        /// </summary>
        /// <param name="fileUrl">图片路径</param>
        ///  <param name="Encode">编码，默认utf-8</param>
        /// <returns>返回解析的结果,成功则返回解析结构,失败则返回空值</returns>
        public static string Read(string fileUrl, string Encode = "utf-8")
        {
            return Read(new Bitmap(fileUrl), Encode);
        }

        /// <summary>
        /// 从指定的图片流解析二维码/条形码
        /// </summary>
        /// <param name="fileUrl">图片流</param>
        ///  <param name="Encode">编码，默认utf-8</param>
        /// <returns>返回解析的结果,成功则返回解析结构,失败则返回空值</returns>
        public static string Read(Stream Stream, string Encode = "utf-8")
        {
            return Read(new Bitmap(Stream), Encode);
        }

        /// <summary>
        /// 从指定的图片流解析二维码/条形码
        /// </summary>
        /// <param name="fileUrl">图片流</param>
        ///  <param name="Encode">编码，默认utf-8</param>
        /// <returns>返回解析的结果,成功则返回解析结构,失败则返回空值</returns>
        public static string Read(Bitmap bitmap, string Encode = "utf-8")
        {
            BarcodeReader reader = new BarcodeReader();
            reader.Options.CharacterSet = Encode;
            Result result = reader.Decode(bitmap);
            return result == null ? "" : result.Text;
        }

        /// <summary>
        /// 从指定图片中解析多个二维码/条形码
        /// </summary>
        /// <param name="fileUrl">图片路径</param>
        /// <param name="Encode">编码，默认utf-8</param>
        /// <returns>返回解析的结果，成功则返回解析结果集合，失败则返回空</returns>
        public static List<string> MultiRead(string fileUrl, string Encode = "utf-8")
        {
            using (FileStream fs = new FileStream(fileUrl, FileMode.Open))
            {
                Image image = Image.FromStream(fs);
                Bitmap bitmap = new Bitmap(image);
                return MultiRead(bitmap, Encode);
            }
        }

        /// <summary>
        /// 从指定图片流中解析多个二维码/条形码
        /// </summary>
        /// <param name="Stream">图片流</param>
        /// <param name="Encode">编码，默认utf-8</param>
        /// <returns>返回解析的结果，成功则返回解析结果集合，失败则返回空</returns>
        public static List<string> MultiRead(Stream Stream, string Encode = "utf-8")
        {
            Image image = Image.FromStream(Stream);
            Bitmap bitmap = new Bitmap(image);
            return MultiRead(bitmap, Encode);
        }

        /// <summary>
        /// 从指定图片流中解析多个二维码/条形码
        /// </summary>
        /// <param name="Stream">图片流</param>
        /// <param name="Encode">编码，默认utf-8</param>
        /// <returns>返回解析的结果，成功则返回解析结果集合，失败则返回空</returns>
        public static List<string> MultiRead(Bitmap bitmap, string Encode = "utf-8")
        {
            List<string> result = new List<string>();

            QRCodeMultiReader qc = new QRCodeMultiReader();
            LuminanceSource source = new BitmapLuminanceSource(bitmap);
            BinaryBitmap binarybitmap = new BinaryBitmap(new HybridBinarizer(source));
            IDictionary<DecodeHintType, object> hints = new Dictionary<DecodeHintType, object>();
            hints.Add(DecodeHintType.CHARACTER_SET, Encode);
            //尝试更多读取
            hints.Add(DecodeHintType.TRY_HARDER, "3");
            Result[] r = qc.decodeMultiple(binarybitmap, hints);
            if (r != null)
                foreach (Result res in r)
                {
                    result.Add(res.Text);
                }

            return result.Count > 0 ? result : null;
        }
    }
}
