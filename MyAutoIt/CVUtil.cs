using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAutoIt
{
    public class CVUtil
    {
        public static Mat crop_color_frame(Mat input, Rectangle crop_region)
        {
            Image<Bgr, Byte> buffer_im = input.ToImage<Bgr, Byte>();
            buffer_im.ROI = crop_region;
            Image<Bgr, Byte> cropped_im = buffer_im.Copy();
            return cropped_im.Mat;
        }
        public static Mat BitmapToMat(Bitmap bitmap)
        {
            Image<Bgr, Byte> imageCV = new Image<Bgr, byte>(bitmap); //Image Class from Emgu.CV
            Mat mat = imageCV.Mat; //This is your Image converted to Mat
            return mat;
        }

        public static Mat ImageToMat(Image image)
        {
            int stride = 0;
            Bitmap bmp = new Bitmap(image);

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);

            System.Drawing.Imaging.PixelFormat pf = bmp.PixelFormat;
            if (pf == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                stride = bmp.Width * 4;
            }
            else
            {
                stride = bmp.Width * 3;
            }

            Image<Bgra, byte> cvImage = new Image<Bgra, byte>(bmp.Width, bmp.Height, stride, (IntPtr)bmpData.Scan0);

            bmp.UnlockBits(bmpData);

            return cvImage.Mat;
        }
        public static String ToString(MKeyPoint keyPoint, String indent = "")
        {
            return indent + JsonConvert.SerializeObject(keyPoint);
        }
        public static String ToString(MDMatch dMatch, String indent = "")
        {
            return indent + JsonConvert.SerializeObject(dMatch);
        }

        public static String ToString(VectorOfKeyPoint vKeyPoint, String indent = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(indent + "[VectorOfKeyPoint Size=" + vKeyPoint.Size);
            for (int i = 0; i < vKeyPoint.Size; i++)
            {
                sb.Append("\n" + ToString(vKeyPoint[i], indent + "\t"));
            }
            sb.Append("\n" + indent + "]");
            return sb.ToString();
        }
        public static String ToString(VectorOfDMatch vDMatch, String indent = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(indent + "[VectorOfDMatch Size=" + vDMatch.Size);
            for (int i = 0; i < vDMatch.Size; i++)
            {
                sb.Append("\n" + ToString(vDMatch[i], indent + "\t"));
            }
            sb.Append("\n" + indent + "]");
            return sb.ToString();
        }
        public static String ToString(VectorOfVectorOfDMatch vDMatch, String indent = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(indent + "[VectorOfVectorOfDMatch Size=" + vDMatch.Size);
            for (int i = 0; i < vDMatch.Size; i++)
            {
                sb.Append("\n" + ToString(vDMatch[i], indent + "\t"));
            }
            sb.Append("\n" + indent + "]");
            return sb.ToString();
        }



        public static String ToString(Mat mat)
        {
            /*if (mat.NumberOfChannels == 1)
            {
                StringBuilder sb = new StringBuilder();
                Image<Gray, Single> imgsave = mat.ToImage<Gray, Single>();

                (new XmlSerializer(typeof(Image<Gray, Single>))).Serialize(new StringWriter(sb), imgsave);
                return sb.ToString();

            }
            else
            {
                StringBuilder sb = new StringBuilder();
                Image<Bgr, Byte> imgsave = mat.ToImage<Bgr, Byte>();

                (new XmlSerializer(typeof(Image<Bgr, Byte>))).Serialize(new StringWriter(sb), imgsave);
                return sb.ToString();
            }*/

            StringBuilder sb = new StringBuilder();
            sb.Append("[Dims=" + mat.Dims + " " + ToString(mat.SizeOfDimemsion));
            for (int i = 0; i < mat.Height; i++)
            {
                for (int j = 0; j < mat.Width; j++)
                {
                    //Object data = mat.Data.GetValue(i* mat.Width + j);
                    //Console.WriteLine(JsonConvert.SerializeObject(data));
                }
            }
            sb.Append("\n]");
            return sb.ToString();
        }
        public static String ToString<T>(T[] d)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < d.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(",");
                }
                sb.Append(d[i]);
            }
            sb.Append("]");
            return sb.ToString();
        }
    }

}
