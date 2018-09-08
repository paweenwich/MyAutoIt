using Accord;
using Accord.Imaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyAutoIt
{
    public class ImageFileData
    {
        public String fileName;
        //public Bitmap image;
    }
    public class ImageTrainData : ImageFileData
    {
        public String label;
        public int labelIndex;
        public override String ToString()
        {
            return String.Format("{0} {1} {2}", label, labelIndex, fileName);
        }
        public ImageTrainData Clone()
        {
            ImageTrainData ret = new ImageTrainData();
            ret.label = label;
            ret.labelIndex = labelIndex;
            ret.fileName = fileName;
            //ret.image = (Bitmap)image.Clone();
            return ret;
        }
    }

    public class ImageTrainDataSet : List<ImageTrainData>
    {
        private double[][] features;
        public bool flgCache = false;

        public byte[] MD5Feature(dynamic bow, Bitmap mask)
        {
            HashAlgorithm hasher = new MD5CryptoServiceProvider();
            hasher.Initialize();
            if (mask != null)
            {
                byte[] maskData = mask.ToBytes();
                hasher.TransformBlock(maskData, 0, maskData.Length, null, 0);
            }
            foreach (var a in this)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(a.fileName);
                hasher.TransformBlock(bytes, 0, bytes.Length, null, 0);
            }
            byte[] bData = Accord.IO.Serializer.Save(bow);
            hasher.TransformBlock(bData, 0, bData.Length, null, 0);
            hasher.TransformFinalBlock(new byte[0], 0, 0);
            byte[] hash = hasher.Hash;
            return hash;
        }

        public double[][] GetFeature(dynamic bow, Bitmap mask = null)
        {
            if (features == null)
            {
                //check if we have this feature in cache
                byte[] hash = MD5Feature(bow,mask);
                String fileName = hash.ToHex() + ".feature";
                
                //RawSerializer<double[][]> s = new RawSerializer<double[][]>();
                if (File.Exists(fileName))
                {
                    
                    String data = File.ReadAllText(fileName);
                    features = JsonConvert.DeserializeObject<double[][]>(data);
                    Console.WriteLine("ImageTrainDataSet load feature from cache " + fileName);
                }
                else
                {
                    Bitmap[] images = GetBitmaps(mask);
                    features = bow.Transform(images);
                    if (flgCache)
                    {
                        String data = Utils.ToJsonString(features);
                        File.WriteAllText(fileName, data);
                        Console.WriteLine("ImageTrainDataSet cahce feature " + fileName);
                    }
                }
            }
            return features;
        }
        public Bitmap[] GetBitmaps(Bitmap mask = null)
        {
            List<Bitmap> ret = new List<Bitmap>();
            foreach (var a in this)
            {
                Bitmap bmp = (Bitmap)Bitmap.FromFile(a.fileName);
                if (mask != null)
                {
                    bmp.ApplyMask(mask);
                }
                ret.Add(bmp);
            }
            return ret.ToArray();
        }
        public int[] GetLabelIndexs()
        {
            List<int> ret = new List<int>();
            foreach (var a in this)
            {
                ret.Add(a.labelIndex);
            }
            return ret.ToArray();
        }
        public String[] GetLabels()
        {
            List<String> ret = new List<String>();
            foreach (var a in this)
            {
                ret.Add(a.label);
            }
            return ret.ToArray();
        }
        public int GetNumOutput()
        {
            var item = this.Max(x => x.labelIndex);
            return item + 1;
        }
        public double[][] GetOutputs(int numOutput)
        {
            int[] labelIndexs = this.GetLabelIndexs();
            double[][] outputs = new double[this.Count][];
            for (int i = 0; i < this.Count; i++)
            {
                outputs[i] = new double[numOutput];
                for (int j = 0; j < numOutput; j++)
                {
                    outputs[i][j] = 0;
                    if (j == labelIndexs[i])
                    {
                        outputs[i][j] = 1;
                    }
                }
            }
            return outputs;
        }
        public TrainDataInfo[] GetInfo(int numOutput)
        {
            TrainDataInfo[] ret = new TrainDataInfo[numOutput];
            for (int i = 0; i < numOutput; i++)
            {
                ret[i] = new TrainDataInfo() { index = i, count = 0, label = "" };
            }
            for (int i = 0; i < this.Count; i++)
            {
                ret[this[i].labelIndex].count++;
                ret[this[i].labelIndex].label = this[i].label;
            }
            return ret;
        }
        public ImageTrainDataSet BalanceData()
        {
            Random rand = new Random();
            ImageTrainDataSet ret = new ImageTrainDataSet();
            ret.AddRange(this);
            TrainDataInfo[] infos = this.GetInfo(this.GetNumOutput());
            int maxCount = infos.Max(x => x.count);
            for (int i = 0; i < infos.Length; i++)
            {
                for (int j = infos[i].count; j < maxCount; j++)
                {
                    //Random _self for that index
                    while (true)
                    {
                        int index = rand.Next(this.Count);
                        if (this[index].labelIndex == i)
                        {
                            ret.Add(this[index].Clone());
                            break;
                        }
                    }
                }
            }
            return ret;
        }

        public ImageTrainDataSet SplitTestData(int num)
        {
            Random rand = new Random();
            //List<ImageTrainData> ret = _self.GetRange(0, num);
            //_self.RemoveRange(0, num);
            int numOutput = this.GetNumOutput();
            ImageTrainDataSet ret = new ImageTrainDataSet();
            while (ret.Count() < num)
            {
                for (int i = 0; i < numOutput; i++)
                {
                    while (true)
                    {
                        int index = rand.Next(this.Count);
                        if (this[index].labelIndex == i)
                        {
                            ret.Add(this[index]);
                            this.RemoveAt(index);
                            break;
                        }
                    }
                }
            }
            return ret;
        }
    }


    public class TrainDataInfo
    {
        public int index;
        public String label;
        public int count;
    }

    public static class ImageTrainDataExtension
    {
        public static byte[] ToBytes(this Bitmap _self)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(_self, typeof(byte[]));
        }

        public static byte[] MD5(this Bitmap _self)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(_self.ToBytes());
            return hash;
        }

        // White is color key
        public static void ApplyMask(this Bitmap _self, Bitmap mask)
        {
            Graphics gx = Graphics.FromImage(_self);
            ImageAttributes imageAttr = new ImageAttributes();
            imageAttr.SetColorKey(Color.White, Color.White, ColorAdjustType.Default);
            gx.DrawImage(mask, new Rectangle(0, 0, _self.Width, _self.Height), 0, 0, mask.Width, mask.Height, GraphicsUnit.Pixel, imageAttr);
            gx.Dispose();
        }

       /* public static Bitmap[] GetBitmaps(this ImageFileData[] _self)
        {
            List<Bitmap> ret = new List<Bitmap>();
            foreach (var a in _self)
            {
                ret.Add(a.image);
            }
            return ret.ToArray();
        }*/
        public static void Show(this BagOfVisualWords bow)
        {
            // We can also check some statistics about the dataset:
            int numberOfImages = bow.Statistics.TotalNumberOfInstances; // 6

            // Statistics about all the descriptors that have been extracted:
            int totalDescriptors = bow.Statistics.TotalNumberOfDescriptors; // 4132
            double totalMean = bow.Statistics.TotalNumberOfDescriptorsPerInstance.Mean; // 688.66666666666663
            double totalVar = bow.Statistics.TotalNumberOfDescriptorsPerInstance.Variance; // 96745.866666666669
            IntRange totalRange = bow.Statistics.TotalNumberOfDescriptorsPerInstanceRange; // [409, 1265]
            Console.WriteLine(String.Format("{0} {1} {2} {3}", totalDescriptors, totalMean, totalVar, totalRange.ToString()));


            // Statistics only about the descriptors that have been actually used:
            int takenDescriptors = bow.Statistics.NumberOfDescriptorsTaken; // 4132
            double takenMean = bow.Statistics.NumberOfDescriptorsTakenPerInstance.Mean; // 688.66666666666663
            double takenVar = bow.Statistics.NumberOfDescriptorsTakenPerInstance.Variance; // 96745.866666666669
            IntRange takenRange = bow.Statistics.NumberOfDescriptorsTakenPerInstanceRange; // [409, 1265]
            Console.WriteLine(String.Format("{0} {1} {2} {3}", takenDescriptors, takenMean, takenVar, takenRange.ToString()));
        }
    }

}
