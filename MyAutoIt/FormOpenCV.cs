using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Features2D;
using Emgu.CV.Util;
using Emgu.CV.Flann;
using Emgu.CV.XFeatures2D;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace MyAutoIt
{
    public partial class FormOpenCV : Form
    {
        public FormOpenCV()
        {
            InitializeComponent();
        }

        public void log(String data)
        {
            Console.WriteLine(data);
        }

        public static List<Mat> lstMat = new List<Mat>();
        public static List<Mat> lstModelDescriptors = new List<Mat>();

        public void ShowKeyPoints()
        {
            lstMat.Clear();
            lstModelDescriptors.Clear();
            var featureDetector = new SIFT();
            Emgu.CV.Flann.LinearIndexParams ip = new Emgu.CV.Flann.LinearIndexParams();
            Emgu.CV.Flann.SearchParams sp = new SearchParams();
            DescriptorMatcher matcher = new FlannBasedMatcher(ip, sp);
            Rectangle cropRect = new Rectangle(842, 646, 70, 70);
            Mat mask = new Mat(new Size(70, 70), DepthType.Cv8U, 1);
            CvInvoke.Rectangle(mask, new Rectangle(0, 0, 70, 70), new MCvScalar(255, 255, 255),-1);
            CvInvoke.Circle(mask, new Point(35, 37), 22, new MCvScalar(0, 0, 0), -1);


            lstMat.Add(mask);
            String[] folders = { @"Linage2\Main\PartyAuto", @"Linage2\Main\PartyManual" };
            foreach (String folder in folders)
            {
                DirectoryInfo imageFolder = new DirectoryInfo(folder);
                FileInfo[] files = Utils.GetFilesByExtensions(imageFolder, ".jpg", ".png").ToArray();
                foreach (FileInfo finfo in files)
                {
                    Mat img = CvInvoke.Imread(finfo.FullName, ImreadModes.Color);
                    Mat crop = CVUtil.crop_color_frame(img, cropRect);
                    //lstMat.Add(crop);
                    VectorOfKeyPoint modelKeyPoints = new VectorOfKeyPoint();
                    Mat modelDescriptors = new Mat();
                    featureDetector.DetectAndCompute(crop, mask, modelKeyPoints, modelDescriptors, false);
                    lstModelDescriptors.Add(modelDescriptors);
                    Mat result = new Mat();
                    Features2DToolbox.DrawKeypoints(crop, modelKeyPoints, result, new Bgr(Color.Red));

                    lstMat.Add(result);
                    //BOWImgDescriptorExtractor bow = new BOWImgDescriptorExtractor(featureDetector, matcher);
                }
            }


            /*BOWKMeansTrainer bowtrainer = new BOWKMeansTrainer(1000, new MCvTermCriteria(10, 0.001), 1, Emgu.CV.CvEnum.KMeansInitType.PPCenters);
            foreach (Mat m in lstModelDescriptors) {
                bowtrainer.Add(m);
            }
            Mat dict = new Mat();
            bowtrainer.Cluster();
            StringBuilder sb = new StringBuilder();
            Image<Bgr, Byte> imgsave = dict.ToImage<Bgr, Byte>();

            (new XmlSerializer(typeof(Image<Bgr, Byte>))).Serialize(new StringWriter(sb), imgsave);
            Console.WriteLine(sb.ToString());*/
        }
        public static Point PointFToPoint(PointF pf)
        {
            return new Point(((int)pf.X), ((int)pf.Y));
        }

        public bool hasFeature(String folder)
        {
            
            return false;
        }


        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstMat.Clear();
            VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch();
            Mat testImage = CvInvoke.Imread(@"Linage2\Main\PartyAuto\2e35av2fwbk.png", ImreadModes.Color);
            Mat modelImage = CVUtil.crop_color_frame(testImage, new Rectangle(842, 646, 70, 70));
            log(modelImage.ToString());
            Image<Bgr, Byte> img = modelImage.ToImage<Bgr, Byte>();
            CvInvoke.cvSetImageROI(img, new Rectangle(0, 0, 35, 35));

            //UMat uModelImage = modelImage.GetUMat(AccessType.Read);
            var featureDetector = new SIFT();
            Mat modelDescriptors = new Mat();
            VectorOfKeyPoint modelKeyPoints = new VectorOfKeyPoint();
            
            VectorOfKeyPoint observedKeyPoints = new VectorOfKeyPoint();
            featureDetector.DetectAndCompute(modelImage, null, modelKeyPoints, modelDescriptors, false);
            log("model size = " + modelKeyPoints.Size);
            Mat observedDescriptors = new Mat();
            featureDetector.DetectAndCompute(testImage, null, observedKeyPoints, observedDescriptors, false);

            int k = 2;
            double uniquenessThreshold = 0.80;
            Mat mask;
            Mat homography = null;
            // Bruteforce, slower but more accurate
            // You can use KDTree for faster matching with slight loss in accuracy
            using (Emgu.CV.Flann.LinearIndexParams ip = new Emgu.CV.Flann.LinearIndexParams())
            using (Emgu.CV.Flann.SearchParams sp = new SearchParams())
            using (DescriptorMatcher matcher = new FlannBasedMatcher(ip, sp))
            {
                matcher.Add(modelDescriptors);

                matcher.KnnMatch(observedDescriptors, matches, k, null);
                mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                mask.SetTo(new MCvScalar(255));
                Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

                int nonZeroCount = CvInvoke.CountNonZero(mask);
                if (nonZeroCount >= 4)
                {
                    nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints,
                        matches, mask, 1.5, 20);
                    if (nonZeroCount >= 4)
                    {
                        
                        homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, matches, mask, 2);
                        PointF[] src = {
                            new PointF(0,0),new PointF(0,modelImage.Height-1),new PointF(modelImage.Width-1,modelImage.Height-1),new PointF(modelImage.Width-1,0)
                        };
                        PointF[] points = CvInvoke.PerspectiveTransform(src, homography);
                        foreach(var p in points)
                        {
                            Console.WriteLine(p.ToString());
                        }
                        Point[] ap = Array.ConvertAll(points,
                        new Converter<PointF, Point>(PointFToPoint));

                        CvInvoke.Polylines(testImage, ap, true, new MCvScalar(255, 0, 0));
                        CvInvoke.Rectangle(testImage, new Rectangle(0, 0, 100, 100), new MCvScalar(255, 255, 0));
                        CvInvoke.Circle(testImage, new Point(100, 100), 50, new MCvScalar(255, 255, 0), -1);
                        lstMat.Add(testImage);
                    }
                    //Mat modelMatches = new Mat();
                    //Features2DToolbox.DrawKeypoints(modelImage, modelKeyPoints, result, new Bgr(Color.Red));
                    //Features2DToolbox.DrawKeypoints(testImage, observedKeyPoints, result, new Bgr(Color.Red));
                    //Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, testImage, observedKeyPoints, matches, modelMatches,
                    //    new MCvScalar(255, 0, 0), new MCvScalar(0, 255, 0));
                    //lstMat.Add(modelMatches);

                    //Mat model1 = new Mat();
                    //Features2DToolbox.DrawKeypoints(modelImage, modelKeyPoints, model1, new Bgr(Color.Red));
                    //lstMat.Add(model1);
                    //modelMatches = crop_color_frame(testImage,new Rectangle(842,646,70,70));
                }
            }
            log("Done " + mask.Size);
            
            Refresh();
        }

        private void FormOpenCV_Paint(object sender, PaintEventArgs e)
        {
            System.Drawing.Graphics g = this.CreateGraphics();
            int y = 50;
            int x = 0;
            int prevHeight = 0;
            g.Clear(Color.White);
            foreach (Mat m in lstMat)
            {
                if (x + m.Width + 50 > this.Width)
                {
                    x = 0;
                    y += prevHeight;
                    prevHeight = 0;
                }
                g.DrawImage(m.Bitmap, new Point(x, y));
                x += m.Width;
                if(m.Height > prevHeight)
                {
                    prevHeight = m.Height;
                }
            }
            //Refresh();
        }
        
        private void shoePointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SimpleFeature sf = new SimpleFeature();
            sf.AddFeature(@"Linage2\Main\PartyAuto", "PartyAuto");
            sf.AddFeature(@"Linage2\Main\PartyAutoNoSkill", "PartyAutoNoSkill");
            foreach (SimpleFeatureData sd in sf)
            {
                Mat result = new Mat();
                Mat result1 = new Mat();
                Mat result2 = new Mat();
                Mat result3 = new Mat();
                Features2DToolbox.DrawKeypoints(sd.mat, sd.keyPoints, result, new Bgr(Color.Cyan));
                lstMat.Add(result);
                /*Features2DToolbox.DrawKeypoints(sd.mat.Split()[0], sd.keyPoints, result1, new Bgr(Color.Blue));
                lstMat.Add(result1);
                Features2DToolbox.DrawKeypoints(sd.mat.Split()[1], sd.keyPoints, result2, new Bgr(Color.Green));
                lstMat.Add(result2);
                Features2DToolbox.DrawKeypoints(sd.mat.Split()[2], sd.keyPoints, result3, new Bgr(Color.Red));
                lstMat.Add(result3);*/
                //log(CVUtil.ToString(sd.keyPoints));
                //log(CVUtil.ToString(sd.descriptors));
            }
            FileInfo[] files = Utils.GetFilesByExtensions(new DirectoryInfo(@"Linage2\Main\PartyAutoNoSkill"), ".jpg", ".png").ToArray();
            foreach (FileInfo f in files)
            {
                Bitmap bmp = (Bitmap)Bitmap.FromFile(f.FullName);
                Console.Write(f.FullName + " " );
                if (sf.hasFeature(bmp))
                {
                    //Console.WriteLine("HasFeature " + f.FullName);
                }
                //break;
            }
            //ShowKeyPoints();
            Refresh();
        }


        private void FormOpenCV_SizeChanged(object sender, EventArgs e)
        {
            Refresh();
        }
    }

    public class SimpleFeature : List<SimpleFeatureData>
    {
        public Mat mask;
        public Rectangle cropRect;
        public SIFT featureDetector;
        public DescriptorMatcher matcher;
        public SimpleFeature(int numFeatures = 0)
        {
            mask = new Mat(new Size(70, 70), DepthType.Cv8U, 1);
            CvInvoke.Rectangle(mask, new Rectangle(0, 0, 70, 70), new MCvScalar(255, 255, 255), -1);
            CvInvoke.Circle(mask, new Point(35, 37), 22, new MCvScalar(0, 0, 0), -1);
            cropRect = new Rectangle(842, 646, 70, 70);

            featureDetector = new SIFT(numFeatures);
            Emgu.CV.Flann.LinearIndexParams ip = new Emgu.CV.Flann.LinearIndexParams();
            Emgu.CV.Flann.SearchParams sp = new SearchParams();
            matcher = new FlannBasedMatcher(ip, sp);

        }
        public bool hasFeature(Bitmap bmpSrc)
        {
            Console.WriteLine("num feature=" + this.Count());
            Bitmap bmp = Utils.cropImage(bmpSrc, cropRect);
            var featureDetector = new SIFT();
            Mat matTest = CVUtil.BitmapToMat(bmp);
            Mat observedDescriptors = new Mat();
            VectorOfKeyPoint observedKeyPoints = new VectorOfKeyPoint();
            featureDetector.DetectAndCompute(matTest, mask, observedKeyPoints, observedDescriptors, false);
            int k = 2;
            double uniquenessThreshold = 0.80;
            Mat homography = null;
            // Bruteforce, slower but more accurate
            // You can use KDTree for faster matching with slight loss in accuracy
            using (Emgu.CV.Flann.KdTreeIndexParams ip = new Emgu.CV.Flann.KdTreeIndexParams())
            using (Emgu.CV.Flann.SearchParams sp = new SearchParams())
            using (DescriptorMatcher matcher = new FlannBasedMatcher(ip, sp))
            {
                VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch();
                foreach (SimpleFeatureData sd in this)
                {
                    matcher.Add(sd.descriptors);
                    //break;
                }

                matcher.KnnMatch(observedDescriptors, matches, k, null);
                Mat mat = new Mat();
                Features2DToolbox.DrawKeypoints(matTest, observedKeyPoints, mat, new Bgr(Color.Blue));
                FormOpenCV.lstMat.Add(mat);
                //Console.WriteLine(CVUtil.ToString(observedDescriptors));
                //Console.WriteLine(CVUtil.ToString(observedKeyPoints));
                Console.WriteLine(CVUtil.ToString(matches));
                Mat uniqueMask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                uniqueMask.SetTo(new MCvScalar(255));
                Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, uniqueMask);

                int nonZeroCount = CvInvoke.CountNonZero(uniqueMask);
                //Console.WriteLine(CVUtil.ToString(uniqueMask));
                Console.WriteLine("nonZeroCount=" + nonZeroCount);
                /*                if (nonZeroCount < 4)
                                {
                                    return false;
                                }
                                foreach (SimpleFeatureData sd in this)
                                {
                                    try
                                    {
                                        int nonZeroCount2 = Features2DToolbox.VoteForSizeAndOrientation(sd.keyPoints, observedKeyPoints, matches, uniqueMask, 1.5, 20);
                                        //Console.WriteLine("nonZeroCount2=" + nonZeroCount2);
                                    }catch(Exception ex)
                                    {
                                        //Console.WriteLine(ex.Message);
                                    }
                                }
                                */
                /*if (nonZeroCount >= 4)
                {
                    nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints,
                        matches, mask, 1.5, 20);
                    if (nonZeroCount >= 4)
                    {

                        homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, matches, mask, 2);
                        PointF[] src = {
                            new PointF(0,0),new PointF(0,modelImage.Height-1),new PointF(modelImage.Width-1,modelImage.Height-1),new PointF(modelImage.Width-1,0)
                        };
                        PointF[] points = CvInvoke.PerspectiveTransform(src, homography);
                        foreach (var p in points)
                        {
                            Console.WriteLine(p.ToString());
                        }
                        Point[] ap = Array.ConvertAll(points,
                        new Converter<PointF, Point>(PointFToPoint));

                        CvInvoke.Polylines(testImage, ap, true, new MCvScalar(255, 0, 0));
                        CvInvoke.Rectangle(testImage, new Rectangle(0, 0, 100, 100), new MCvScalar(255, 255, 0));
                        CvInvoke.Circle(testImage, new Point(100, 100), 50, new MCvScalar(255, 255, 0), -1);
                        lstMat.Add(testImage);
                    }
                }*/
            }
            return true;
        }

        public void AddFeature(String folder, String label)
        {
            DirectoryInfo imageFolder = new DirectoryInfo(folder);
            FileInfo[] files = Utils.GetFilesByExtensions(imageFolder, ".jpg", ".png").ToArray();
            foreach (FileInfo finfo in files)
            {
                if (finfo.Name == "mask.png")
                {
                    continue;
                }
                SimpleFeatureData featureData = new SimpleFeatureData();
                featureData.label = label;
                Mat img = CvInvoke.Imread(finfo.FullName, ImreadModes.Color);
                featureData.mat = CVUtil.crop_color_frame(img, cropRect);

                VectorOfKeyPoint modelKeyPoints = new VectorOfKeyPoint();
                Mat modelDescriptors = new Mat();

                featureDetector.DetectAndCompute(featureData.mat, mask, featureData.keyPoints, featureData.descriptors, false);
                Add(featureData);
            }

        }
    }
    public class SimpleFeatureData
    {
        public Mat mat = new Mat();
        public VectorOfKeyPoint keyPoints = new VectorOfKeyPoint();
        public Mat descriptors = new Mat();
        public String label;
    }

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
        public static String ToString(MDMatch dMatch, String indent="")
        {
            return indent + JsonConvert.SerializeObject(dMatch);
        }

        public static String ToString(VectorOfKeyPoint vKeyPoint, String indent = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(indent + "[VectorOfKeyPoint Size=" + vKeyPoint.Size);
            for(int i = 0; i < vKeyPoint.Size; i++)
            {
                sb.Append("\n" + ToString(vKeyPoint[i],indent + "\t"));
            }
            sb.Append("\n" + indent + "]");
            return sb.ToString();
        }
        public static String ToString(VectorOfDMatch vDMatch,String indent = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(indent+ "[VectorOfDMatch Size=" + vDMatch.Size);
            for (int i = 0; i < vDMatch.Size; i++)
            {
                sb.Append("\n" + ToString(vDMatch[i], indent + "\t"));
            }
            sb.Append("\n" + indent + "]");
            return sb.ToString();
        }
        public static String ToString(VectorOfVectorOfDMatch vDMatch,String indent = "")
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
            for (int i = 0; i < mat.Height; i++) {
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
