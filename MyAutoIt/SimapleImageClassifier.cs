using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using static MyAutoIt.Utils;

namespace MyAutoIt
{
    public static class SimpleImageClassifierConfig
    {
        public static int maxColorDiff = 128;
    }

    public class ImageFeatureVectorData
    {
        public Point pos;
        public Color color;
    }
    public class ImageFeatureVector : List<ImageFeatureVectorData> {
        public static ImageFeatureVector LoadFromFile(String fileName)
        {
            String data = File.ReadAllText(fileName);
            ImageFeatureVector fv = JsonConvert.DeserializeObject<ImageFeatureVector>(data);
            return fv;
        }
        public void SaveToFile(String fileName)
        {
            File.WriteAllText(fileName, JsonConvert.SerializeObject(this));
        }
        public void LoadFromDiffBitmap(Bitmap bmp,Rectangle rect)
        {
            Clear();
            if (rect.IsEmpty)
            {
                rect = new Rectangle(new Point(0, 0), bmp.Size);
            }
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Point p = new Point(x, y);
                    if (rect.Contains(p))
                    {
                        Color color = bmp.GetPixel(x, y);
                        if (Utils.ColourDistance(color, Color.Black) < SimpleImageClassifierConfig.maxColorDiff)
                        {
                            continue;
                        }
                        else
                        {
                            Add(new ImageFeatureVectorData() { pos = p, color = color});
                        }
                    }
                }
            }
        }
        public ImageFeatureVector ApplyBitmap(Bitmap bmp,Rectangle rect)
        {
            if (rect.IsEmpty)
            {
                rect = new Rectangle(new Point(0, 0), bmp.Size);
            }
            ImageFeatureVector ret = new ImageFeatureVector();
            foreach (ImageFeatureVectorData data in this)
            {
                if (rect.Contains(data.pos))
                {
                    Color color = bmp.GetPixel(data.pos.X, data.pos.Y);
                    if (Utils.ColourDistance(color, data.color) < SimpleImageClassifierConfig.maxColorDiff)
                    {
                        ret.Add(data);
                    }
                }
            }
            return ret;
        }

        public ImageFeatureVector MaskBitmap(Bitmap bmp)
        {
            ImageFeatureVector ret = new ImageFeatureVector();
            foreach (ImageFeatureVectorData data in this)
            {
                Color color = bmp.GetPixel(data.pos.X, data.pos.Y);
                ret.Add(new ImageFeatureVectorData() { pos = data.pos, color = color });
            }
            return ret;
        }
        public Bitmap ToBitmap(Bitmap main)
        {
            Bitmap ret = new Bitmap(main);
            for (int x = 0; x < ret.Width; x++)
            {
                for (int y = 0; y < ret.Height; y++)
                {
                    ret.SetPixel(x, y, Color.Black);
                }
            }
            foreach (ImageFeatureVectorData data in this)
            {
                Color color = main.GetPixel(data.pos.X, data.pos.Y);
                ret.SetPixel(data.pos.X, data.pos.Y, color);
            }
            return ret;
        }
        public double Match(Bitmap bmp)
        {
            double numMatch = 0;
            double numPoint = 0;
            Rectangle bmpRect =  new Rectangle(new Point(0, 0), bmp.Size);
            foreach (ImageFeatureVectorData data in this)
            {
                if (bmpRect.Contains(data.pos))
                {
                    numPoint++;
                    Color color = bmp.GetPixel(data.pos.X, data.pos.Y);
                    if (Utils.ColourDistance(color, data.color) < SimpleImageClassifierConfig.maxColorDiff)
                    {
                        numMatch++;
                    }
                }
            }
            return numMatch/ numPoint;
        }
    }

    public class ClassifyResult
    {
        public String label;
        public double score;
        public double evalTime;
        public override String ToString()
        {
            return label + String.Format(" {0:0.00} time={1:0}ms", score*100,evalTime);
        }
    }
    public class SimpleImageClassifier
    {
        public Dictionary<String, ImageFeatureVector> features = new Dictionary<string, ImageFeatureVector>();
        public Dictionary<String, double> classifyDetail = new Dictionary<string, double>();
        public ClassifyResult lastResult;
        public void AddFromFile(String fileName)
        {
            ImageFeatureVector fv = ImageFeatureVector.LoadFromFile(fileName);
            Console.WriteLine("SimpleImageClassifier: AddFromFile " + fileName + " " + fv.Count);
            features.Add(Path.GetFileNameWithoutExtension(fileName), fv);
        }
        public ClassifyResult Classify(Bitmap bmp,double minScore=0.60)
        {
            double bestMatchScore = 0;
            ClassifyResult bestResult = null;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            classifyDetail.Clear();
            ClassifyResult[] results = Classifies(bmp, minScore);
            foreach(ClassifyResult result in results)
            {
                if(result.score> bestMatchScore)
                {
                    bestMatchScore = result.score;
                    bestResult = result;
                }
            }
            /*foreach (String key in features.Keys)
            {
                ImageFeatureVector fv = features[key];
                double score = fv.Match(bmp);
                classifyDetail.Add(key, score);
                if ( score> bestMatchScore)
                {
                    bestMatchScore = score;
                    bestKey = key;
                }
            }*/
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            if (bestResult != null)
            {
                bestResult.evalTime = ts.TotalMilliseconds;
            }
            return bestResult;
        }
        public ClassifyResult[] Classifies(Bitmap bmp, double minScore = 0.60)
        {
            List<ClassifyResult> ret = new List<ClassifyResult>();
            Stopwatch stopWatch = new Stopwatch();
            classifyDetail.Clear();
            foreach (String key in features.Keys)
            {
                ImageFeatureVector fv = features[key];
                double score = fv.Match(bmp);
                classifyDetail.Add(key, score);
                if(score > minScore)
                {

                    ClassifyResult tmp = new ClassifyResult() { label = key, score = score, evalTime = 0};
                    ret.Add(tmp);
                }
            }
            return ret.ToArray();
        }

        public ImageFeatureVector CreateFeatureVector(String folder)
        {
            ImageFeatureVector ret = null;
            DirectoryInfo d = new DirectoryInfo(folder);
            FileInfo[] Files = d.GetFiles("*.png");
            Bitmap main = null;
            foreach (FileInfo file in Files)
            {
                Console.WriteLine(file.FullName);
                Bitmap bmp = (Bitmap)Bitmap.FromFile(file.FullName);
                if(main == null)
                {
                    main = bmp;
                }else
                {
                    //main.Save(@"D:\Data\Linage2\diff.png");
                    if(ret == null)
                    {
                        main = DiffBitmap(main, bmp);
                        ret = new ImageFeatureVector();
                        //ret.LoadFromDiffBitmap(main,new Rectangle(new Point(85,30),new Size(main.Width-85,main.Height-30)));
                        ret.LoadFromDiffBitmap(main, new Rectangle(new Point(0,0), new Size(main.Width, main.Height)));
                    }
                    else
                    {
                        //ret = ret.ApplyBitmap(bmp, new Rectangle(new Point(85, 30), new Size(main.Width-85, main.Height-30)));
                        ret = ret.ApplyBitmap(bmp, new Rectangle(new Point(0,0), new Size(main.Width, main.Height)));
                    }
                }
            }
            main = ret.ToBitmap(main);
            //main.Save(@"D:\Data\Linage2\diff.png");
            ret = AverageFeature(folder, main);
            if (ret != null)
            {
                //ret.ToBitmap(main).Save(@"D:\Data\Linage2\avg.png");
            }
            return ret;
        }
        public ImageFeatureVector CreateFeatureVectorEx(String folder)
        {
            ImageFeatureVector ret = null;
            DirectoryInfo d = new DirectoryInfo(folder);
            FileInfo[] Files = d.GetFiles("*.png");
            Bitmap main = null;
            foreach (FileInfo file in Files)
            {
                Console.WriteLine(file.FullName);
                Bitmap bmp = (Bitmap)Bitmap.FromFile(file.FullName);
                if (main == null)
                {
                    main = bmp;
                }
                else
                {
                    //main.Save(@"D:\Data\Linage2\diff.png");
                    if (ret == null)
                    {
                        main = DiffBitmap(main, bmp);
                        ret = new ImageFeatureVector();
                        ret.LoadFromDiffBitmap(main, new Rectangle(new Point(0,0), new Size(main.Width, main.Height)));
                    }
                    else
                    {
                        ret = ret.ApplyBitmap(bmp, new Rectangle(new Point(0, 0), new Size(main.Width, main.Height)));
                    }
                }
            }
            main = ret.ToBitmap(main);
            //main.Save(@"D:\Data\Linage2\diff.png");
            ret = AverageFeature(folder, main);
            /*if (ret != null)
            {
                ret.ToBitmap(main).Save(@"D:\Data\Linage2\avg.png");
            }*/
            return ret;
        }

        public void Verify(String folder,String name,double level=0.8)
        {
            DirectoryInfo d = new DirectoryInfo(folder);
            FileInfo[] Files = d.GetFiles("*.png");
            foreach (FileInfo file in Files)
            {
                Bitmap bmp = (Bitmap)Bitmap.FromFile(file.FullName);
                ClassifyResult result = Classify(bmp, level);
                if ((result == null)||(result.label != name))
                {
                    Console.WriteLine("Fail: " + file.FullName + " " + lastResult.ToString());
                }
            }
        }

        public ImageFeatureVector AverageFeature(String folder,Bitmap bmpMask = null)
        {
            ImageFeatureVector mask = null;
            if (File.Exists(folder + "mask.png"))
            {
                mask = new ImageFeatureVector();
                if (bmpMask == null)
                {
                    bmpMask = (Bitmap)Bitmap.FromFile(folder + "mask.png");
                }
                mask.LoadFromDiffBitmap(bmpMask, new Rectangle(new Point(0, 0), bmpMask.Size));
                List<ImageFeatureVector> fvs = new List<ImageFeatureVector>();
                DirectoryInfo d = new DirectoryInfo(folder);
                FileInfo[] Files = d.GetFiles("*.png");
                foreach (FileInfo file in Files)
                {
                    Bitmap bmp = (Bitmap)Bitmap.FromFile(file.FullName);
                    ImageFeatureVector fv = mask.MaskBitmap(bmp);
                    fvs.Add(fv);
                }
                for (int i = 0; i < mask.Count; i++)
                {
                    double r = 0;
                    double g = 0;
                    double b = 0;
                    foreach (ImageFeatureVector fv in fvs)
                    {
                        r += fv[i].color.R;
                        g += fv[i].color.G;
                        b += fv[i].color.B;
                    }
                    mask[i].color = Color.FromArgb((byte)(r / fvs.Count), (byte)(g / fvs.Count), (byte)(b / fvs.Count));
                }
            }else
            {
                Console.WriteLine("ERROR: mask.png not found");
            }
            return mask;
        }
        public Bitmap DiffBitmap(Bitmap bmp1,Bitmap bmp2)
        {
            Bitmap ret = new Bitmap(bmp1);
            for(int x=0; x < ret.Width; x++)
            {
                for(int y = 0; y < ret.Height; y++)
                {
                    Color color1 = bmp1.GetPixel(x, y);
                    if(color1 == Color.Black)
                    {
                        continue;
                    }
                    Color color2 = bmp2.GetPixel(x, y);
                    double diff = Utils.ColourDistance(color1, color2);
                    if (diff < SimpleImageClassifierConfig.maxColorDiff)
                    {
                        ret.SetPixel(x, y, color1);
                    }else
                    {
                        ret.SetPixel(x, y, Color.Black);
                    }
                }
            }
            return (ret);
        }
    }
}
