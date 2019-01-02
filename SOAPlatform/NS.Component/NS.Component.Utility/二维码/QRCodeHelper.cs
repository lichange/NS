//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using ThoughtWorks.QRCode.Codec;

//namespace NS.Component.Utility.QRCode
//{
//    public class QRCodeHelper
//    {
//        /// <summary>
//        /// 二维码生成方法
//        /// </summary>
//        /// <param name="text">二维码内容</param>
//        /// <param name="scale">像素</param>
//        /// <param name="version">版本</param>
//        /// <param name="mode">编码模式</param>
//        /// <param name="correct">修正模式</param>
//        /// <returns></returns>
//        public static Bitmap CreateQRCode(string text, string scale="4", string version="7", QRCodeEncoder.ENCODE_MODE mode = QRCodeEncoder.ENCODE_MODE.BYTE, QRCodeEncoder.ERROR_CORRECTION correct = QRCodeEncoder.ERROR_CORRECTION.M)
//        {
//            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();

//            qrCodeEncoder.QRCodeEncodeMode = mode;

//            try
//            {
//                int scale1 = Convert.ToInt16(scale);
//                qrCodeEncoder.QRCodeScale = scale1;
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Invalid size!");
//            }
//            try
//            {
//                int version1 = Convert.ToInt16(version);
//                qrCodeEncoder.QRCodeVersion = version1;
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Invalid version !");
//            }

//            qrCodeEncoder.QRCodeErrorCorrect = correct;

//            try
//            {
//                Bitmap image;
//                image = qrCodeEncoder.Encode(text, Encoding.Default);
//                return image;
//            }
//            catch (Exception exe)
//            {
//                throw new Exception("生成二维码的过程中出现错误:" + exe.Message);
//            }
//        }

//        /// <summary>
//        /// 二维码生成方法--可在中心显示图片
//        /// </summary>
//        /// <param name="text">二维码内容</param>
//        /// <param name="myImage">自定义图片</param>
//        /// <param name="scale">像素</param>
//        /// <param name="version">版本</param>
//        /// <param name="mode">编码模式</param>
//        /// <param name="correct">修正模式</param>
//        /// <returns></returns>
//        public static Bitmap CreateQRCode(string text,Image myImage, string scale = "4", string version = "7", QRCodeEncoder.ENCODE_MODE mode = QRCodeEncoder.ENCODE_MODE.BYTE, QRCodeEncoder.ERROR_CORRECTION correct = QRCodeEncoder.ERROR_CORRECTION.M)
//        {
//            var sourceImage = CreateQRCode(text, scale, version, mode, correct);
//            DrawImage(sourceImage, myImage);
//            return sourceImage;
//        }

//        /// <summary>
//        /// 二维码生成方法
//        /// </summary>
//        /// <param name="text">二维码内容</param>
//        /// <param name="scale">像素</param>
//        /// <param name="version">版本</param>
//        /// <param name="mode">编码模式</param>
//        /// <param name="correct">修正模式</param>
//        /// <returns></returns>
//        public static Bitmap CreateQRCode(string text, QRCodeEncoder.ENCODE_MODE mode, int scale, int version, QRCodeEncoder.ERROR_CORRECTION correct)
//        {
//            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
//            qrCodeEncoder.QRCodeEncodeMode = mode;
//            qrCodeEncoder.QRCodeScale = scale;
//            qrCodeEncoder.QRCodeVersion = version;
//            qrCodeEncoder.QRCodeErrorCorrect = correct;

//            try
//            {
//                Bitmap image;
//                image = qrCodeEncoder.Encode(text, Encoding.Default);
//                return image;
//            }
//            catch (Exception exe)
//            {
//                throw new Exception("生成二维码的过程中出现错误:" + exe.Message);
//            }
//        }

//        /// <summary>
//        /// 二维码生成方法--可在中心显示图片
//        /// </summary>
//        /// <param name="text">二维码内容</param>
//        /// <param name="myImage">自定义图片</param>
//        /// <param name="scale">像素</param>
//        /// <param name="version">版本</param>
//        /// <param name="mode">编码模式</param>
//        /// <param name="correct">修正模式</param>
//        /// <returns></returns>
//        public static Bitmap CreateQRCode(string text,Image myImage, QRCodeEncoder.ENCODE_MODE mode, int scale, int version, QRCodeEncoder.ERROR_CORRECTION correct)
//        {
//            var sourceImage = CreateQRCode(text, mode, scale, version, correct);
//            DrawImage(sourceImage, myImage);
//            return sourceImage;
//        }

//        public static Bitmap DrawImage(Bitmap img, Image myimg)
//        {
//            const float multiple = 3.5f; 
//            //计算图片大小  
//            float w = img.Width / multiple;

//            float pw = w / myimg.Width;
//            float ph = w / myimg.Height;
//            if (pw > ph)
//            {
//                pw = ph;
//            }

//            int mw = (int)(pw * myimg.Width);
//            int mh = (int)(pw * myimg.Height);

//            //计算图片在二维上的x,y坐标  
//            int x = (img.Width - mw) / 2;
//            int y = (img.Height - mh) / 2;
//            Graphics graphics = Graphics.FromImage(img);
//            graphics.DrawImage(myimg, x, y, mw, mh);
//            graphics.Dispose();

//            return img;
//        }
//    }
//}
