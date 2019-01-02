using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;

namespace NS.Component.Utility
{
    public class VerificationCode
    {
        private AnimatedGifEncoder _Coder = new AnimatedGifEncoder();
        private Random _Random = new Random();

        private int _Width = 150;
        /// <summary>
        /// 验证码宽
        /// </summary>
        public int Width { get { return _Width; } }
        private int _Height = 28;
        /// <summary>
        /// 验证码高
        /// </summary>
        public int Height { get { return _Height; } }
        private int _FrameCount = 4;
        /// <summary>
        /// 验证码帧数
        /// </summary>
        public int FrameCount { get { return _FrameCount; } }
        private int _Delay = 900;
        /// <summary>
        /// 每帧延迟时间
        /// </summary>
        public int Delay { get { return _Delay; } }
        private int _NoiseCount = 100;
        /// <summary>
        /// 噪点个数
        /// </summary>
        public int NoiseCount { get { return _NoiseCount; } }
        private int _LineCount = 6;
        /// <summary>
        /// 干扰线个数
        /// </summary>
        public int LineCount { get { return _LineCount; } }

        /// <summary>
        /// 验证码构造函数
        /// </summary>
        /// <param name="codeSetting">验证码规格参数设置</param>
        public VerificationCode(CodeSetting codeSetting)
            : this(codeSetting.Width, codeSetting.Height, codeSetting.FrameCount, codeSetting.Delay, codeSetting.NoiseCount, codeSetting.LineCount)
        { }

        /// <summary>
        /// 验证码构造函数
        /// </summary>
        /// <param name="width">验证码宽</param>
        /// <param name="height">验证码高</param>
        public VerificationCode(int width, int height)
            : this(width, height, 4, 900, 100, 6)
        {
        }

        /// <summary>
        /// 验证码构造函数
        /// </summary>
        /// <param name="width">验证码宽</param>
        /// <param name="height">验证码高</param>
        /// <param name="frameCount">帧数</param>
        public VerificationCode(int width, int height, int frameCount)
            : this(width, height, frameCount, 900, 100, 6)
        {
        }

        /// <summary>
        /// 验证码构造函数
        /// </summary>
        /// <param name="width">验证码宽</param>
        /// <param name="height">验证码高</param>
        /// <param name="frameCount">帧数</param>
        /// <param name="delay">延迟</param>
        /// <param name="noiseCount">噪点高数</param>
        /// <param name="lineCount">干扰线个数</param>
        public VerificationCode(int width, int height, int frameCount, int delay, int noiseCount, int lineCount)
        {
            _Width = width < 1 ? 1 : width;
            _Height = height < 1 ? 1 : height;
            _Coder.SetSize(Width, Height);
            _Coder.SetRepeat(0);
            _FrameCount = frameCount;
            _Delay = delay;
            _NoiseCount = noiseCount;
            _LineCount = lineCount;
        }

        private void ProcessGraphicGif(string verifyCode)
        {
            Brush br = Brushes.White;
            Rectangle rect = new Rectangle(0, 0, Width, Height);
            Pen pen = new Pen(Color.LightGray, 0);
            int randAngle = _Random.Next(30, 60);//随机转动角度
            for (int i = 0; i < FrameCount; i++)
            {
                Image image = new Bitmap(Width, Height);
                Graphics ga = Graphics.FromImage(image);
                ga.FillRectangle(br, rect);
                //添加噪点
                pen.Color = Color.LightGray;//噪点颜色
                for (int j = 0; j < NoiseCount; j++)
                {
                    ga.DrawRectangle(pen, _Random.Next(0, image.Width), _Random.Next(0, image.Height), 1, 1);
                }
                //添加干扰横线
                for (int j = 0; j < LineCount; j++)
                {
                    pen.Color = Color.FromArgb(_Random.Next(0, 256), _Random.Next(0, 256), _Random.Next(0, 256));
                    ga.DrawLine(pen, new Point(_Random.Next(1, Width - 1), _Random.Next(1, Height - 1)), new Point(_Random.Next(1, Width - 1), _Random.Next(1, Height - 1)));
                }
                char[] chars = verifyCode.ToCharArray();
                StringFormat format = new StringFormat(StringFormatFlags.NoClip);
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                //定义字体
                string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体", "Arial Baltic" };
                for (int k = 0; k < chars.Length; k++)
                {
                    int findex = _Random.Next(font.Length);
                    //font　封装在特定设备上呈现特定字体所需的纹理和资源（字体，大小，字体样式）
                    Font f = new System.Drawing.Font(font[findex], 20, System.Drawing.FontStyle.Bold);
                    /*Brush定义用于填充图形图像（如矩形、椭圆、圆形、多边形和封闭路径）的内部对象
                    SolidBrush(Color.White)初始化指定的颜色　指定画笔颜色为白色*/
                    Color color = Color.FromArgb(_Random.Next(0, 256), _Random.Next(0, 256), _Random.Next(0, 256));
                    Brush b = new System.Drawing.SolidBrush(color);
                    Point dot = new Point(16, 16);
                    //转动的度数
                    float angle = _Random.Next(-randAngle, randAngle);
                    //移动光标到指定位置
                    ga.TranslateTransform(dot.X, dot.Y);
                    ga.RotateTransform(angle);
                    /*在指定的位置并且用指定的Brush和Font对象绘制指定的文本字符串
                   （指定的字符串，字符串的文本格式，绘制文本颜色和纹理，所绘制文本的左上角的x坐标，坐标）*/
                    ga.DrawString(chars[k].ToString(), f, b, 1, 1, format);
                    //转回去
                    ga.RotateTransform(-angle);
                    //移动光标指定位置
                    ga.TranslateTransform(2, -dot.Y);
                }
                ga.Flush();
                _Coder.SetDelay(Delay);
                _Coder.AddFrame(image);
                image.Dispose();
            }
            _Coder.Finish();
        }

        public void Create(string verifyCode, string path)
        {
            //coder.Start(path);用它的这个方法,比用 stream 生成的要大!
            FileStream fs = new FileStream(path, FileMode.Create);
            _Coder.Start(fs);
            ProcessGraphicGif(verifyCode);
            fs.Close();
        }

        public Stream Create(string verifyCode, Stream stream)
        {
            _Coder.Start(stream);
            ProcessGraphicGif(verifyCode);
            return stream;
        }

        public MemoryStream Create(string verifyCode)
        {
            MemoryStream stream = new MemoryStream();
            _Coder.Start(stream);
            ProcessGraphicGif(verifyCode);
            return stream;
        }

        public void ProcessRequest(string verifyCode, HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = "image/Gif";
            var stream = Create(verifyCode);
            context.Response.Body.Write(stream.ToArray(), 0, stream.ToArray().Length);
            // context.Response.BinaryWrite(stream.ToArray());
            stream.Dispose();
        }
    }
}