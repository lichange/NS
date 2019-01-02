using System;
using System.Text;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace NS.Component.Utility
{
    public class VerifyCode
    {
        /// <summary>
        /// 根据字符生成图片
        /// </summary>
        /// <param name="validateCode">验证码</param>
        public static byte[] CreateValidateGraphic(string validateCode)
        {
            Random rand = new Random();
            int randAngle = rand.Next(30, 60);//随机转动角度
            int iwidth = validateCode.Length * 23;
            //封装GDI+ 位图，此位图由图形图像及其属性的像素数据组成，指定的宽度和高度。以像素为单位
            Bitmap image = new Bitmap(iwidth, 28);
            //封装一个　GDI+绘图图面。无法继承此类。从指定的Image创建新的 Graphics
            Graphics g = Graphics.FromImage(image);
            try
            {
                //清除整个绘图面并以指定背景填充
                g.Clear(Color.AliceBlue);
                //画一个边框
                g.DrawRectangle(new Pen(Color.Silver, 0), 0, 0, image.Width - 1, image.Height - 1);
                //定义绘制直线和曲线的对象。（只是Pen的颜色，指示此Pen的宽度的值）
                Pen blackPen = new Pen(Color.LightGray, 0);
                //Random rand = new Random();
                //划横线的条数 可以根据自己的要求      
                for (int i = 0; i < 50; i++)
                {
                    //随机高度
                    /*绘制一条连线由坐标对指定的两个点的线条
                     线条颜色、宽度和样式，第一个点的x坐标和y坐标，第二个点的x坐标和y坐标*/
                    //g.DrawLine(blackPen, 0, y, image.Width, y);
                    int x = rand.Next(0, image.Width);
                    int y = rand.Next(0, image.Height);
                    //画矩形，坐标（x,y）宽高(1,1)
                    g.DrawRectangle(blackPen, x, y, 1, 1);
                }

                //拆散字符串成单个字符数组
                char[] chars = validateCode.ToCharArray();
                //文字居中
                StringFormat format = new StringFormat(StringFormatFlags.NoClip);
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                //定义字体
                string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体", "Arial Baltic" };

                for (int i = 0; i < chars.Length; i++)
                {
                    int findex = rand.Next(font.Length);
                    //font　封装在特定设备上呈现特定字体所需的纹理和资源（字体，大小，字体样式）
                    Font f = new System.Drawing.Font(font[findex], 16, System.Drawing.FontStyle.Bold);
                    /*Brush定义用于填充图形图像（如矩形、椭圆、圆形、多边形和封闭路径）的内部对象
                    SolidBrush(Color.White)初始化指定的颜色　指定画笔颜色为白色*/
                    Color color = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
                    Brush b = new System.Drawing.SolidBrush(color);
                    Point dot = new Point(16, 16);
                    //转动的度数
                    float angle = rand.Next(-randAngle, randAngle);
                    //移动光标到指定位置
                    g.TranslateTransform(dot.X, dot.Y);
                    g.RotateTransform(angle);
                    /*在指定的位置并且用指定的Brush和Font对象绘制指定的文本字符串
                   （指定的字符串，字符串的文本格式，绘制文本颜色和纹理，所绘制文本的左上角的x坐标，坐标）*/
                    g.DrawString(chars[i].ToString(), f, b, 1, 1, format);
                    //转回去
                    g.RotateTransform(-angle);
                    //移动光标指定位置
                    g.TranslateTransform(2, -dot.Y);
                }
                //创建存储区为内存流
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                //将此图像以指定的格式保存到指定的流中（将其保存在内存流中，图像的格式）
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
    }

    public class VerifyCodeConfig
    {
        /// <summary>
        /// 更新验证码规格参数
        /// </summary>
        /// <param name="codeSetting"></param>
        /// <returns></returns>
        public static bool UpdateCodeSetting(CodeSetting codeSetting)
        {
            string path = Appsettings.InitConfig;
            XElement xel = XElement.Load(path);
            var elements = xel.Descendants("VerifyCode").Select(p => p);
            if (elements != null)
            {
                elements.ToList().ForEach(x =>
                {
                    x.SetAttributeValue("width", codeSetting.Width);
                    x.SetAttributeValue("height", codeSetting.Height);
                    x.SetAttributeValue("framecount", codeSetting.FrameCount);
                    x.SetAttributeValue("delay", codeSetting.Delay);
                    x.SetAttributeValue("noisecount", codeSetting.NoiseCount);
                    x.SetAttributeValue("linecount", codeSetting.LineCount);
                });
            }
            try
            {
                xel.Save(path);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取验证码规格参数
        /// </summary>
        /// <returns></returns>
        public static CodeSetting GetCodeSetting()
        {
            return GetCodeSetting(Appsettings.InitConfig);
        }

        /// <summary>
        /// 获取验证码规格参数
        /// </summary>
        /// <param name="path">配置文件路径</param>
        /// <returns>获取验证码规格参数</returns>
        private static CodeSetting GetCodeSetting(string path)
        {
            var result = XElement.Load(path).Descendants("VerifyCode").Select(p => new
            {
                width = p.Attribute("width").Value,
                height = p.Attribute("height").Value,
                framecount = p.Attribute("framecount").Value,
                delay = p.Attribute("delay").Value,
                noisecount = p.Attribute("noisecount").Value,
                linecount = p.Attribute("linecount").Value
            }).Single();
            if (result != null)
            {
                return new CodeSetting
                {
                    Width = result.width.ToInt32(),
                    Height = result.height.ToInt32(),
                    FrameCount = result.framecount.ToInt32(),
                    Delay = result.delay.ToInt32(),
                    NoiseCount = result.noisecount.ToInt32(),
                    LineCount = result.linecount.ToInt32()
                };
            }
            return new CodeSetting
            {
                Width = 105,
                Height = 30,
                FrameCount = 4,
                Delay = 900,
                NoiseCount = 100,
                LineCount = 6
            };
        }
    }
}
