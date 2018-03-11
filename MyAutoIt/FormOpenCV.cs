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

        List<Mat> lstMat = new List<Mat>();
        List<Mat> lstModelDescriptors = new List<Mat>();

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
            SimpleFeature sf = SimpleFeature.CreateFromFolder(@"Linage2\Main\PartyAuto", "PartyAuto",24);
            foreach(SimpleFeatureData sd in sf)
            {
                Mat result = new Mat();
                Features2DToolbox.DrawKeypoints(sd.mat, sd.keyPoints, result, new Bgr(Color.Red));
                lstMat.Add(result);
            }
            FileInfo[] files = Utils.GetFilesByExtensions(new DirectoryInfo(@"Linage2\Main\PartyAuto"), ".jpg", ".png").ToArray();
            foreach (FileInfo f in files)
            {
                Bitmap bmp = (Bitmap)Bitmap.FromFile(f.FullName);
                if (sf.hasFeature(bmp))
                {
                    Console.WriteLine("HasFeature " + f.FullName);
                }
            }
            //ShowKeyPoints();
            Refresh();
        }


        private void FormOpenCV_SizeChanged(object sender, EventArgs e)
        {
            Refresh();
        }
    }

    public class SimpleFeature: List<SimpleFeatureData>
    {
        public String name;
        public Mat mask;
        public Rectangle cropRect;
        public SimpleFeature(String name)
        {
            this.name = name;
            mask = new Mat(new Size(70, 70), DepthType.Cv8U, 1);
            CvInvoke.Rectangle(mask, new Rectangle(0, 0, 70, 70), new MCvScalar(255, 255, 255), -1);
            CvInvoke.Circle(mask, new Point(35, 37), 22, new MCvScalar(0, 0, 0), -1);
            cropRect = new Rectangle(842, 646, 70, 70);
        }
        public bool hasFeature(Bitmap bmpSrc)
        {
            Bitmap bmp = Utils.cropImage(bmpSrc,cropRect);
            var featureDetector = new SIFT();
            Mat matTest = CVUtil.BitmapToMat(bmp);
            Mat observedDescriptors = new Mat();
            VectorOfKeyPoint observedKeyPoints = new VectorOfKeyPoint();
            featureDetector.DetectAndCompute(matTest, null, observedKeyPoints, observedDescriptors, false);
            int k = 2;
            double uniquenessThreshold = 0.80;
            Mat homography = null;
            // Bruteforce, slower but more accurate
            // You can use KDTree for faster matching with slight loss in accuracy
            using (Emgu.CV.Flann.LinearIndexParams ip = new Emgu.CV.Flann.LinearIndexParams())
            using (Emgu.CV.Flann.SearchParams sp = new SearchParams())
            using (DescriptorMatcher matcher = new FlannBasedMatcher(ip, sp))
            {
                VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch();
                foreach (SimpleFeatureData sd in this)
                {
                    matcher.Add(sd.descriptors);
                }

                matcher.KnnMatch(observedDescriptors, matches, k, null);
                Mat uniqueMask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                uniqueMask.SetTo(new MCvScalar(255));
                Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, uniqueMask);

                int nonZeroCount = CvInvoke.CountNonZero(uniqueMask);
                //Console.WriteLine("nonZeroCount=" + nonZeroCount);
                if(nonZeroCount < 4)
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
        public static SimpleFeature CreateFromFolder(String folder, String name,int numFeatures = 0)
        {
            SimpleFeature ret = new SimpleFeature(name);
            var featureDetector = new SIFT(numFeatures);
            Emgu.CV.Flann.LinearIndexParams ip = new Emgu.CV.Flann.LinearIndexParams();
            Emgu.CV.Flann.SearchParams sp = new SearchParams();
            DescriptorMatcher matcher = new FlannBasedMatcher(ip, sp);


            DirectoryInfo imageFolder = new DirectoryInfo(folder);
            FileInfo[] files = Utils.GetFilesByExtensions(imageFolder, ".jpg", ".png").ToArray();
            foreach (FileInfo finfo in files)
            {
                if(finfo.Name == "mask.png")
                {
                    continue;
                }
                SimpleFeatureData featureData = new SimpleFeatureData();
                Mat img = CvInvoke.Imread(finfo.FullName, ImreadModes.Color);
                featureData.mat = CVUtil.crop_color_frame(img, ret.cropRect);

                VectorOfKeyPoint modelKeyPoints = new VectorOfKeyPoint();
                Mat modelDescriptors = new Mat();

                featureDetector.DetectAndCompute(featureData.mat, ret.mask, featureData.keyPoints, featureData.descriptors, false);
                ret.Add(featureData);
            }
            return ret;
        }
    }
    public class SimpleFeatureData
    {
        public Mat mat = new Mat();
        public VectorOfKeyPoint keyPoints = new VectorOfKeyPoint();
        public Mat descriptors = new Mat();
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

        private static Mat ImageToMat(Image image)
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

    }
}
