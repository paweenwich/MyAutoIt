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

            SimpleFeatureDesc autoSkillFeatureDesc = new SimpleFeatureDesc();
            autoSkillFeatureDesc.cropPoint = new Point(0, 200);
            autoSkillFeatureDesc.size = new Size(200, 220);
            autoSkillFeatureDesc.rect = new Rectangle(new Point(0, 0), autoSkillFeatureDesc.size);
            //autoSkillFeatureDesc.maskPoint = new Point(35, 37);
            autoSkillFeatureDesc.colorFilter = new SimpleFeatureColorFilter(new MCvScalar(230-50, 200 -40 , 70-25), new MCvScalar(230 + 25, 200 + 25, 70 + 25));
            autoSkillFeatureDesc.features.Add(new SimpleFeatureInfo(@"Linage2\SIFT\Quest2", "Quest2"));
            SimpleFeature sf = new SimpleFeature(autoSkillFeatureDesc);
            sf.Load(true);
            foreach(SimpleFeatureData fd in sf)
            {
                lstMat.Add(fd.mat);
            }

            /*String json = JsonConvert.SerializeObject(autoSkillFeatureDesc,Formatting.Indented);
            autoSkillFeatureDesc = JsonConvert.DeserializeObject<SimpleFeatureDesc>(json);
            Console.WriteLine(json);
            */
            //Mat mask = new Mat(size, DepthType.Cv8U, 1);
            //CvInvoke.Rectangle(mask, rect, new MCvScalar(255, 255, 255), -1);
            //CvInvoke.Circle(mask, new Point(35, 37), 23, new MCvScalar(0, 0, 0), -1);
            //SimpleFeature sf = new SimpleFeature(autoSkillFeatureDesc);
            //SimpleFeature sf = new SimpleFeature(new Point(842, 646), size, mask);
            //sf.SetColorFilter(new MCvScalar(200, 150, 0), new MCvScalar(255, 250, 200));
            //sf.AddFeature(@"Linage2\SIFT\AutoNoSkill", "AutoNoSkill", true);
            //sf.AddFeature(@"Linage2\SIFT\AutoSkill", "AutoSkill", true);
            //sf.AddFeature(@"Linage2\SIFT\NoAuto", "NoAuto",0, true);
            //SimpleFeature sf = SimpleFeature.CreateFromFile(@"Linage2\SIFT\AutoSkill.json");
            //sf.Load(true);

            foreach (SimpleFeatureData sd in sf)
            {
                //Mat result = new Mat();
                //Mat result1 = new Mat();
                //Mat result2 = new Mat();
                //Mat result3 = new Mat();
                //Features2DToolbox.DrawKeypoints(sd.mat, sd.keyPoints, result, new Bgr(Color.Cyan));
                //lstMat.Add(result);
                /*Features2DToolbox.DrawKeypoints(sd.mat.Split()[0], sd.keyPoints, result1, new Bgr(Color.Blue));
                lstMat.Add(result1);
                Features2DToolbox.DrawKeypoints(sd.mat.Split()[1], sd.keyPoints, result2, new Bgr(Color.Green));
                lstMat.Add(result2);
                Features2DToolbox.DrawKeypoints(sd.mat.Split()[2], sd.keyPoints, result3, new Bgr(Color.Red));
                lstMat.Add(result3);*/
                //log(CVUtil.ToString(sd.keyPoints));
                //log(CVUtil.ToString(sd.descriptors));
                //Refresh();
            }
            foreach (SimpleFeatureTestData sd in sf.testData)
            {
                Bitmap bmp = (Bitmap)Bitmap.FromFile(sd.filePath);
                Console.WriteLine(sd.filePath +  " " + sf.GetFeature(bmp));
                //Mat mat = new Mat();
                //Features2DToolbox.DrawKeypoints(sf.lastObserved, sf.lastObservedKeyPoint, mat, new Bgr(Color.Blue));
                Console.WriteLine(sf.MatchesToString(sf.lastMatches));
                lstMat.Add(sf.lastObserved);
                break;
            }
            String[] testPath = { @"Linage2\SIFT\NoAuto", @"Linage2\SIFT\AutoNoSkill" };
            foreach(String path in testPath)
            {
                FileInfo[] files = Utils.GetFilesByExtensions(new DirectoryInfo(path), ".jpg", ".png").ToArray();
                foreach (FileInfo f in files)
                {
                    Bitmap bmp = (Bitmap)Bitmap.FromFile(f.FullName);
                    String label = sf.GetFeature(bmp);
                    Console.WriteLine(f.FullName + " " + label);
                    Mat mat = new Mat();
                    if (label == "")
                    {
                        CvInvoke.Resize(sf.lastObserved, mat, new Size(50, 50));
                    }else
                    {
                        mat = sf.lastObserved.Clone();
                        CvInvoke.PutText(mat, label, new Point(0, 20), FontFace.HersheyPlain, 1, new MCvScalar(255, 255, 255));
                        Console.WriteLine(sf.MatchesToString(sf.lastMatches));
                    }
                    lstMat.Add(mat);
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



}
