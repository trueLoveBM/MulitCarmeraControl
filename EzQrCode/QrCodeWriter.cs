using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace EzQrCode
{
    /// <summary>
    /// 二维码生成器
    /// </summary>
    public class QrCodeWriter
    {
        /// <summary>
        /// 生成普通二维码,保存成图片
        /// </summary>
        /// <param name="Content">二维码内容</param>
        /// <param name="SavePath">二维码保存路径</param>
        /// <param name="ImageFormat">二维码图片格式</param>
        /// <param name="Encode">编码，默认utf-8</param>
        /// <param name="Width">二维码宽</param>
        /// <param name="Height">二维码高</param>
        public static void CreateQrCode(string Content, string SavePath, ImageFormat ImageFormat, string Encode = "utf-8", int Width = 500, int Height = 500)
        {
            Bitmap map = CreateQrCode(Content, ImageFormat, Encode, Width, Height);
            map.Save(SavePath, ImageFormat);
            map.Dispose();
        }

        /// <summary>
        /// 生成普通二维码,并返回其Bitmap对象
        /// </summary>
        /// <param name="Content">二维码内容</param>
        /// <param name="SavePath">二维码保存路径</param>
        /// <param name="ImageFormat">二维码图片格式</param>
        /// <param name="Encode">编码，默认utf-8</param>
        /// <param name="Width">二维码宽</param>
        /// <param name="Height">二维码高</param>
        public static Bitmap CreateQrCode(string Content, ImageFormat ImageFormat, string Encode = "utf-8", int Width = 500, int Height = 500)
        {
            BarcodeWriter writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            QrCodeEncodingOptions options = new QrCodeEncodingOptions();
            options.DisableECI = true;
            //设置内容编码
            options.CharacterSet = Encode;
            //设置二维码的宽度和高度
            options.Width = Width;
            options.Height = Height;
            //设置二维码的边距,单位不是固定像素
            options.Margin = 1;
            writer.Options = options;

            Bitmap map = writer.Write(Content);
            return map;
        }

        /// <summary>
        /// 生成带Logo二维码,保存成图片
        /// </summary>
        /// <param name="Content">二维码内容</param>
        /// <param name="SavePath">二维码保存路径</param>
        /// <param name="ImageFormat">二维码图片格式</param>
        /// <param name="LogoBitmap">二维码的bitmap对象</param>
        /// <param name="Encode">编码，默认utf-8</param>
        /// <param name="Width">二维码宽</param>
        /// <param name="Height">二维码高</param>
        public static void CreateQrCodeWithLogo(string Content, string SavePath, ImageFormat ImageFormat, Bitmap LogoBitmap, string Encode = "utf-8", int Width = 500, int Height = 500)
        {
            Bitmap bmpimg = CreateQrCodeWithLogo(Content, LogoBitmap, Encode, Width, Height);
            //保存成图片
            bmpimg.Save(SavePath, ImageFormat);
        }

        /// <summary>
        /// 生成带Logo二维码,保存成图片
        /// </summary>
        /// <param name="Content">二维码内容</param>
        /// <param name="LogoBitmap">二维码的bitmap对象</param>
        /// <param name="Encode">编码，默认utf-8</param>
        /// <param name="Width">二维码宽</param>
        /// <param name="Height">二维码高</param>
        public static Bitmap CreateQrCodeWithLogo(string Content, Bitmap LogoBitmap, string Encode = "utf-8", int Width = 500, int Height = 500)
        {
            //构造二维码写码器
            MultiFormatWriter writer = new MultiFormatWriter();
            Dictionary<EncodeHintType, object> hint = new Dictionary<EncodeHintType, object>();
            hint.Add(EncodeHintType.CHARACTER_SET, Encode);
            hint.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);

            //生成二维码 
            BitMatrix bm = writer.encode(Content, BarcodeFormat.QR_CODE, Width, Height, hint);
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            Bitmap map = barcodeWriter.Write(bm);


            //获取二维码实际尺寸（去掉二维码两边空白后的实际尺寸）
            int[] rectangle = bm.getEnclosingRectangle();

            //计算插入图片的大小和位置
            int middleW = Math.Min((int)(rectangle[2] / 3.5), LogoBitmap.Width);
            int middleH = Math.Min((int)(rectangle[3] / 3.5), LogoBitmap.Height);
            int middleL = (map.Width - middleW) / 2;
            int middleT = (map.Height - middleH) / 2;

            //将img转换成bmp格式，否则后面无法创建Graphics对象
            Bitmap bmpimg = new Bitmap(map.Width, map.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmpimg))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.DrawImage(map, 0, 0);
            }
            //将二维码插入图片
            Graphics myGraphic = Graphics.FromImage(bmpimg);
            //白底
            myGraphic.FillRectangle(Brushes.White, middleL, middleT, middleW, middleH);
            myGraphic.DrawImage(LogoBitmap, middleL, middleT, middleW, middleH);

            //保存成图片
            return bmpimg;
        }
    }
}
