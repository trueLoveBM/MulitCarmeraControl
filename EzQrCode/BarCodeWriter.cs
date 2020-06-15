using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using ZXing;
using ZXing.Common;

namespace EzQrCode
{
    /// <summary>
    /// 条形码生成器
    /// </summary>
    public class BarCodeWriter
    {
        /// <summary>
        /// 生成条形码并保存
        /// </summary>
        /// <param name="Content">条形码内容</param>
        /// <param name="SavePath">存储路径</param>
        /// <param name="ImageFormat">生成二维码图片的格式</param>
        public static void CreateBarCode(string Content, string SavePath, ImageFormat ImageFormat)
        {
            BarcodeWriter writer = new BarcodeWriter();
            //使用ITF 格式，不能被现在常用的支付宝、微信扫出来
            //如果想生成可识别的可以使用 CODE_128 格式
            //writer.Format = BarcodeFormat.ITF;
            writer.Format = BarcodeFormat.CODE_128;
            EncodingOptions options = new EncodingOptions()
            {
                Width = 150,
                Height = 50,
                Margin = 2
            };
            writer.Options = options;
            Bitmap map = writer.Write(Content);
            map.Save(SavePath, ImageFormat);
            map.Dispose();
            map = null;
        }

        /// <summary>
        /// 生成条形码并保存
        /// </summary>
        /// <param name="Content">条形码内容</param>
        public static Bitmap CreateBarCode(string Content)
        {
            BarcodeWriter writer = new BarcodeWriter();
            //使用ITF 格式，不能被现在常用的支付宝、微信扫出来
            //如果想生成可识别的可以使用 CODE_128 格式
            //writer.Format = BarcodeFormat.ITF;
            writer.Format = BarcodeFormat.CODE_128;
            EncodingOptions options = new EncodingOptions()
            {
                Width = 150,
                Height = 50,
                Margin = 2
            };
            writer.Options = options;
            Bitmap map = writer.Write(Content);
            return map;
        }
    }
}
