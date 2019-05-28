#if !NET461
#else

using Never.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Never.Web
{
    /// <summary>
    /// 生成图片验证码
    /// </summary>
    public static class DrawingHelper
    {
        #region 图片

        /// <summary>
        /// 创建随机码图片
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NotNullOrEmpty(Name = "text")]
        public static MemoryStream CreateImage(string text)
        {
            return CreateImage(text, 24, 24);
        }

        /// <summary>
        /// 创建随机码图片
        /// </summary>
        /// <param name="text"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        [NotNullOrEmpty(Name = "text")]
        public static MemoryStream CreateImage(string text, int height, int width)
        {
            int randAngle = 60; //随机转动角度

            //生成图片
            MemoryStream ms = new MemoryStream();
            using (Bitmap map = new Bitmap(width, height))//创建图片背景
            using (Graphics graph = Graphics.FromImage(map))
            {
                graph.Clear(Color.AliceBlue);//清除画面，填充背景
                graph.DrawRectangle(new Pen(Color.Black, 0), 0, 0, map.Width - 1, map.Height - 1);//画一个边框
                graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;//模式

                //生成随机生成器
                Random rand = new Random();
                //画图片的背景噪音线
                for (int i = 0; i < 4; i++)
                {
                    int x1 = rand.Next(map.Width);
                    int x2 = rand.Next(map.Width);
                    int y1 = rand.Next(map.Height);
                    int y2 = rand.Next(map.Height);
                    graph.DrawLine(new Pen(Color.Black), x1, y1, x2, y2);
                }

                //背景噪点生成
                Pen blackPen = new Pen(Color.White, 0);
                for (int i = 0; i < 128; i++)
                {
                    int x = rand.Next(0, map.Width);
                    int y = rand.Next(0, map.Height);
                    graph.DrawRectangle(blackPen, x, y, 1, 1);
                }

                //验证码旋转，防止机器识别
                char[] chars = text.ToCharArray();//拆散字符串成单字符数组

                //文字距中
                StringFormat format = new StringFormat(StringFormatFlags.NoClip)
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                //定义颜色
                Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
                //定义字体
                string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };

                for (int i = 0; i < chars.Length; i++)
                {
                    int cindex = rand.Next(7);
                    int findex = rand.Next(5);
                    Font f = new System.Drawing.Font(font[findex], 14, System.Drawing.FontStyle.Bold);//字体样式(参数2为字体大小)
                    Brush b = new System.Drawing.SolidBrush(c[cindex]);

                    //begin mode by lxl 2014-02-28
                    //让图片生成的时候居中

                    //Point dot = new Point(14, 14);

                    Point dot = new Point(14, height / 2);

                    //end mode
                    graph.DrawString(dot.X.ToString(), f, new SolidBrush(Color.Black), 10, 150);//测试X坐标显示间距的
                    float angle = rand.Next(-randAngle, randAngle);//转动的度数

                    graph.TranslateTransform(dot.X, dot.Y);//移动光标到指定位置
                    graph.RotateTransform(angle);
                    graph.DrawString(chars[i].ToString(), f, b, 1, 1, format);
                    //graph.DrawString(chars[i].ToString(), f, new SolidBrush(Color.Blue), 1, 1, format);
                    graph.RotateTransform(-angle);//转回去
                    graph.TranslateTransform(-2, -dot.Y);//移动光标到指定位置，每个字符紧凑显示，避免被软件识别
                }
                map.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            }
            return ms;
        }

        #endregion 图片

        #region 水印

        /// <summary>
        /// 图片水印
        /// </summary>
        private static float[][] colorMatrixElements = {
                    new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                    new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                    new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                    new float[] {0.0f,  0.0f,  0.0f,  0.3f, 0.0f},
                    new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                };

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="picture">图片</param>
        /// <param name="percent">百分比</param>
        /// <returns></returns>
        public static Bitmap CreateThumbnail(Image picture, double percent)
        {
            if (picture == null)
                return null;

            return new Bitmap(picture, Convert.ToInt32(picture.Width * percent), Convert.ToInt32(picture.Height * percent));
        }

        /// <summary>
        /// 加水印图片
        /// </summary>
        /// <param name="picture">imge</param>
        /// <param name="watermark">水印文字图片</param>
        /// <param name="markType">水印位置</param>
        public static void AddWatermark(Image picture, Image watermark, WatermarkType markType)
        {
            int xpos = 0;
            int ypos = 0;
            switch (markType)
            {
                case WatermarkType.左上:
                    {
                        xpos = (int)(picture.Width * (float).01);
                        ypos = (int)(picture.Height * (float).01);
                    }
                    break;

                case WatermarkType.中上:
                    {
                        xpos = (int)(picture.Width * (float).50) - (watermark.Width / 2);
                        ypos = (int)(picture.Height * (float).01);
                    }
                    break;

                case WatermarkType.右上:
                    {
                        xpos = (int)(picture.Width * (float).99) - (watermark.Width / 2);
                        ypos = (int)(picture.Height * (float).01);
                    }
                    break;

                case WatermarkType.左中:
                    {
                        xpos = (int)(picture.Width * (float).01);
                        ypos = (int)(picture.Height * (float).50) - (watermark.Height / 2);
                    }
                    break;

                case WatermarkType.中中:
                    {
                        xpos = (int)(picture.Width * (float).50) - (watermark.Width / 2);
                        ypos = (int)(picture.Height * (float).50) - (watermark.Height / 2);
                    }
                    break;

                case WatermarkType.右中:
                    {
                        xpos = (int)(picture.Width * (float).99) - watermark.Width;
                        ypos = (int)(picture.Height * (float).50) - (watermark.Height / 2);
                    }
                    break;

                case WatermarkType.左下:
                    {
                        xpos = (int)(picture.Width * (float).01);
                        ypos = (int)(picture.Height * (float).99) - watermark.Height;
                    }
                    break;

                case WatermarkType.中下:
                    {
                        xpos = (int)(picture.Width * (float).50) - (watermark.Width / 2);
                        ypos = (int)(picture.Height * (float).99) - watermark.Height;
                    }
                    break;

                case WatermarkType.右下:
                    {
                        xpos = (int)(picture.Width * (float).99) - watermark.Width;
                        ypos = (int)(picture.Height * (float).99) - watermark.Height;
                    }
                    break;
            }

            using (var graphics = Graphics.FromImage(picture))
            using (var imageAttributes = new ImageAttributes())
            {
                var colorMap = new ColorMap()
                {
                    OldColor = Color.FromArgb(255, 0, 255, 0),
                    NewColor = Color.FromArgb(0, 0, 0, 0)
                };

                imageAttributes.SetRemapTable(new[] { colorMap }, ColorAdjustType.Bitmap);
                var colorMatrix = new ColorMatrix(colorMatrixElements);
                graphics.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);
            }
        }

        /// <summary>
        /// 加水印文字
        /// </summary>
        /// <param name="picture">imge</param>
        /// <param name="text">水印文字内容</param>
        /// <param name="markType">水印位置</param>
        /// <param name="font">文字</param>
        public static void AddWatermark(Image picture, string text, WatermarkType markType, Font font)
        {
            SizeF sizef = new SizeF();

            using (var graphics = Graphics.FromImage(picture))
            {
                if (font == null)
                    font = new Font("arial", 16, FontStyle.Bold);

                sizef = graphics.MeasureString(text, font);
                float xpos = 0;
                float ypos = 0;

                switch (markType)
                {
                    case WatermarkType.左上:
                        {
                            xpos = (float)picture.Width * (float).01;
                            ypos = (float)picture.Height * (float).01;
                        }
                        break;

                    case WatermarkType.中上:
                        {
                            xpos = ((float)picture.Width * (float).50) - (sizef.Width / 2);
                            ypos = (float)picture.Height * (float).01;
                        }
                        break;

                    case WatermarkType.右上:
                        {
                            xpos = ((float)picture.Width * (float).99) - sizef.Width;
                            ypos = (float)picture.Height * (float).01;
                        }
                        break;

                    case WatermarkType.左中:
                        {
                            xpos = (float)picture.Width * (float).01;
                            ypos = ((float)picture.Height * (float).50) - (sizef.Height / 2);
                        }
                        break;

                    case WatermarkType.中中:
                        {
                            xpos = ((float)picture.Width * (float).50) - (sizef.Width / 2);
                            ypos = ((float)picture.Height * (float).50) - (sizef.Height / 2);
                        }
                        break;

                    case WatermarkType.右中:
                        {
                            xpos = ((float)picture.Width * (float).99) - sizef.Width;
                            ypos = ((float)picture.Height * (float).50) - (sizef.Height / 2);
                        }
                        break;

                    case WatermarkType.左下:
                        {
                            xpos = (float)picture.Width * (float).01;
                            ypos = ((float)picture.Height * (float).99) - sizef.Height;
                        }
                        break;

                    case WatermarkType.中下:
                        {
                            xpos = ((float)picture.Width * (float).50) - (sizef.Width / 2);
                            ypos = ((float)picture.Height * (float).99) - sizef.Height;
                        }
                        break;

                    case WatermarkType.右下:
                        {
                            xpos = ((float)picture.Width * (float).99) - sizef.Width;
                            ypos = ((float)picture.Height * (float).99) - sizef.Height;
                        }
                        break;
                }

                using (var format = new StringFormat() { Alignment = StringAlignment.Near })
                using (var brush_1 = new SolidBrush(Color.FromArgb(153, 0, 0, 0)))
                using (var brush_2 = new SolidBrush(Color.FromArgb(153, 255, 255, 255)))
                {
                    graphics.DrawString(text, font, brush_1, xpos, ypos, format);
                    graphics.DrawString(text, font, brush_2, xpos, ypos, format);
                }
            }
        }

        #endregion 水印
    }
}

#endif