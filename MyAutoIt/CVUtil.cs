using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Flann;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAutoIt
{
    public class CVUtil
    {
        public static Rectangle GetRect(VectorOfKeyPoint vKeyPoint)
        {
            //Rectangle ret = new Rectangle(0,0,0,0);
            int minx = 10000;
            int miny = 10000;
            int maxx = 0;
            int maxy = 0;
            for (int i = 0; i < vKeyPoint.Size; i++)
            {
                MKeyPoint p = vKeyPoint[i];
                if (p.Point.X < minx) minx = (int)p.Point.X;
                if (p.Point.X > maxx) maxx = (int)p.Point.X;
                if (p.Point.Y < miny) miny = (int)p.Point.Y;
                if (p.Point.Y > maxy) maxy = (int)p.Point.Y;


            }
            return new Rectangle(minx, miny, maxx - minx, maxy - miny);
        }
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

    public class SimpleFeatureColorFilter
    {
        public MCvScalar lowerBound;
        public MCvScalar upperBound;
        public SimpleFeatureColorFilter(MCvScalar lowerBound, MCvScalar upperBound)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }
    }

    public class SimpleFeatureInfo
    {
        public String folder;
        public String label;
        public SimpleFeatureInfo(String folder, String label)
        {
            this.folder = folder;
            this.label = label;
        }
    }

    public class SimpleFeatureDesc
    {
        public Point cropPoint;
        public Size size;
        public Rectangle rect;
        public Point maskPoint;
        public SimpleFeatureColorFilter colorFilter;
        public List<SimpleFeatureInfo> features = new List<SimpleFeatureInfo>();
    }

    public class SimpleFeature : List<SimpleFeatureData>
    {
        public Mat mask;
        public Mat lower;
        public Mat upper;
        public int colorIndex=-1;
        public Rectangle cropRect;
        public SIFT featureDetector;
        public DescriptorMatcher matcher;
        public List<SimpleFeatureTestData> testData = new List<SimpleFeatureTestData>();
        public Size size;
        public List<SimpleFeatureInfo> features = new List<SimpleFeatureInfo>();
        public Mat lastObserved;
        public VectorOfKeyPoint lastObservedKeyPoint;
        public VectorOfVectorOfDMatch lastMatches;
        public SimpleFeatureData lastMatchFeatureData;
        public static SimpleFeature CreateFromFile(String fileName)
        {
            String data = File.ReadAllText(fileName);
            SimpleFeatureDesc desc = JsonConvert.DeserializeObject<SimpleFeatureDesc>(data);
            SimpleFeature sf = new SimpleFeature(desc);
            return sf;
        }
        public SimpleFeature(SimpleFeatureDesc desc, SIFT featureDetector = null)
        {
            size = desc.size;
            cropRect = new Rectangle(desc.cropPoint, size);
            mask = new Mat(desc.size, DepthType.Cv8U, 1);
            features = desc.features;
            CvInvoke.Rectangle(mask, desc.rect, new MCvScalar(255, 255, 255), -1);
            CvInvoke.Circle(mask, desc.maskPoint, 23, new MCvScalar(0, 0, 0), -1);

            //SimpleFeature sf = new SimpleFeature(new Point(842, 646), size, mask);
            if(desc.colorFilter != null)
            {
                SetColorFilter(desc.colorFilter.lowerBound, desc.colorFilter.upperBound);
            }
            //featureDetector = new SIFT(0,2,0.02,20,0.4);
            if (featureDetector == null)
            {
                this.featureDetector = new SIFT(0, 3, 0.04, 10, 0.8);
            }
            Emgu.CV.Flann.LinearIndexParams ip = new Emgu.CV.Flann.LinearIndexParams();
            Emgu.CV.Flann.SearchParams sp = new SearchParams();
            matcher = new FlannBasedMatcher(ip, sp);

        }
        public void SetColorFilter(MCvScalar lowerBound, MCvScalar upperBound)
        {
            //Set color filter
            lower = new Mat(size, DepthType.Cv8U, 3);
            CvInvoke.Rectangle(lower, new Rectangle(new Point(0, 0), size), lowerBound, -1);
            upper = new Mat(size, DepthType.Cv8U, 3);
            CvInvoke.Rectangle(upper, new Rectangle(new Point(0, 0), size), upperBound, -1);
        }
        public void Load(bool flgTrain = false)
        {
            foreach(SimpleFeatureInfo f in this.features)
            {
                AddFeature(f.folder, f.label, flgTrain);
            }
            
        }

        public String GetLabel(int index)
        {
            return this[index].label;
        }
        public String GetLabelFromMatches(VectorOfVectorOfDMatch vDMatch)
        {
            Dictionary<String, int> labelCount = new Dictionary<string, int>();
            for (int i = 0; i < vDMatch.Size; i++)
            {
                VectorOfDMatch vMatch = vDMatch[i];
                for (int j = 0; j < vMatch.Size; j++)
                {
                    MDMatch dmatch = vMatch[j];
                    //sb.Append("\n\t" + JsonConvert.SerializeObject(dmatch) + " " + );
                    String label = GetLabel(dmatch.ImgIdx);
                    if (labelCount.ContainsKey(label))
                    {
                        labelCount[label] += 1;
                    }
                    else
                    {
                        labelCount[label] = 1;
                    }
                }
            }
            String ret = labelCount.Keys.Aggregate((i, j) => labelCount[i] >= labelCount[j] ? i : j);
            // Scan to find best imgIndex
            Dictionary<int, int> imgIndexCount = new Dictionary<int, int>();
            for (int i = 0; i < vDMatch.Size; i++)
            {
                VectorOfDMatch vMatch = vDMatch[i];
                for (int j = 0; j < vMatch.Size; j++)
                {
                    MDMatch dmatch = vMatch[j];
                    if(GetLabel(dmatch.ImgIdx) == ret)
                    {
                        if (imgIndexCount.ContainsKey(dmatch.ImgIdx))
                        {
                            imgIndexCount[dmatch.ImgIdx] += 1;
                        }
                        else
                        {
                            imgIndexCount[dmatch.ImgIdx] = 1;
                        }
                    }
                }
            }
            int imgIndex = imgIndexCount.Keys.Aggregate((i, j) => imgIndexCount[i] >= imgIndexCount[j] ? i : j);
            Console.WriteLine("imgIndex=" + imgIndex);
            lastMatchFeatureData = this[imgIndex];
            return ret;

        }
        public String MatchesToString(VectorOfVectorOfDMatch vDMatch, String indent = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(indent + "[VectorOfVectorOfDMatch Size=" + vDMatch.Size);
            for (int i = 0; i < vDMatch.Size; i++)
            {
                VectorOfDMatch vMatch = vDMatch[i];
                for (int j = 0; j < vMatch.Size; j++)
                {
                    MDMatch dmatch = vMatch[j];
                    sb.Append("\n\t" + JsonConvert.SerializeObject(dmatch) + " " + GetLabel(dmatch.ImgIdx));
                }
                //sb.Append("\n" + ToString(vDMatch[i], indent + "\t"));
            }
            sb.Append("\n" + indent + "]");
            return sb.ToString();
        }
        public String GetFeature(Bitmap bmpSrc)
        {
            Mat matTest = CVUtil.BitmapToMat(bmpSrc);
            matTest = ProcessImage(matTest);
            /*Bitmap bmp = Utils.cropImage(bmpSrc, cropRect);
            Mat matTest = CVUtil.BitmapToMat(bmp);
            if (colorIndex != -1)
            {
                matTest = matTest.Split()[colorIndex];
            }*/
            Mat observedDescriptors = new Mat();
            VectorOfKeyPoint observedKeyPoints = new VectorOfKeyPoint();
            featureDetector.DetectAndCompute(matTest, mask, observedKeyPoints, observedDescriptors, false);
            int k = 2;
            double uniquenessThreshold = 0.80;
            //Mat homography = null;
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
                lastMatches = matches;
                lastObserved = matTest;
                lastObservedKeyPoint = observedKeyPoints;
                //Mat mat = new Mat();
                //Features2DToolbox.DrawKeypoints(matTest, observedKeyPoints, mat, new Bgr(Color.Blue));
                //FormOpenCV.lstMat.Add(mat);
                //Console.WriteLine(CVUtil.ToString(observedDescriptors));
                //Console.WriteLine(CVUtil.ToString(observedKeyPoints));
                //Console.WriteLine(CVUtil.ToString(matches));
                //Console.WriteLine(MatchesToString(matches));
                Mat uniqueMask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                uniqueMask.SetTo(new MCvScalar(255));
                Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, uniqueMask);

                int nonZeroCount = CvInvoke.CountNonZero(uniqueMask);
                if (nonZeroCount > 4)
                {
                    //int nonZeroCount2 = Features2DToolbox.VoteForSizeAndOrientation(this[0].keyPoints, observedKeyPoints,
                    //matches, uniqueMask, 1.5, 20);
                    //Console.WriteLine("nonZeroCount2=" + nonZeroCount2);
                    return GetLabelFromMatches(matches);
                }
                else
                {
                    return "";
                }
                //Console.WriteLine(CVUtil.ToString(uniqueMask));
                //Console.WriteLine("nonZeroCount=" + nonZeroCount);
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
        }

        public Mat ProcessImage(Mat img)
        {
            if (colorIndex != -1)
            {
                img = img.Split()[colorIndex];
            }

            //CvInvoke.InRange(img, lowerLimit, upperLimit, imgOut);
            img = CVUtil.crop_color_frame(img, cropRect);
            Mat ret = new Mat();
            if (lower != null)
            {
                CvInvoke.InRange(img, lower, upper, ret);
            }
            else
            {
                ret = img;
            }
            return ret;
        }

        public void AddFeature(String folder, String label, bool flgTrain = false)
        {
            DirectoryInfo imageFolder = new DirectoryInfo(folder);
            FileInfo[] files = Utils.GetFilesByExtensions(imageFolder, ".jpg", ".png").ToArray();
            int numFeature = files.Length;
            if (flgTrain)
            {
                numFeature = (int)(0.7 * numFeature);
            }
            int index = 0;
            foreach (FileInfo finfo in files)
            {
                if (finfo.Name == "mask.png")
                {
                    continue;
                }
                if (index <= numFeature)
                {
                    SimpleFeatureData featureData = new SimpleFeatureData();
                    featureData.label = label;
                    Mat img = CvInvoke.Imread(finfo.FullName, ImreadModes.Color);
                    img = ProcessImage(img);
                    /*
                        if (colorIndex != -1)
                        {
                            img = img.Split()[colorIndex];
                        }

                        //CvInvoke.InRange(img, lowerLimit, upperLimit, imgOut);
                        featureData.mat = CVUtil.crop_color_frame(img, cropRect);
                        Mat imgOut = new Mat();
                        CvInvoke.InRange(featureData.mat, lower, upper, imgOut);
                        //FormOpenCV.lstMat.Add(imgOut);
                        featureData.mat = imgOut;
                    */
                    featureData.mat = img;
                    //FormOpenCV.lstMat.Add(img);

                    VectorOfKeyPoint modelKeyPoints = new VectorOfKeyPoint();
                    Mat modelDescriptors = new Mat();

                    featureDetector.DetectAndCompute(featureData.mat, mask, featureData.keyPoints, featureData.descriptors, false);

                    //Mat result = new Mat();
                    //Features2DToolbox.DrawKeypoints(featureData.mat, featureData.keyPoints, result, new Bgr(Color.Cyan));
                    //FormOpenCV.lstMat.Add(result);

                    Add(featureData);
                }
                else
                {
                    testData.Add(new SimpleFeatureTestData(finfo.FullName, label));
                }
                index++;
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
    public class SimpleFeatureTestData
    {
        public String filePath;
        public String label;
        public SimpleFeatureTestData(String filePath, String label)
        {
            this.filePath = filePath;
            this.label = label;
        }
    }

}
