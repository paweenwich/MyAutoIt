using Accord;
using Accord.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAutoIt
{
    public class ImageFileData
    {
        public String fileName;
        public Bitmap image;
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
            ret.image = (Bitmap)image.Clone();
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
        // White is color key
        public static void ApplyMask(this Bitmap _self, Bitmap mask)
        {
            Graphics gx = Graphics.FromImage(_self);
            ImageAttributes imageAttr = new ImageAttributes();
            imageAttr.SetColorKey(Color.White, Color.White, ColorAdjustType.Default);
            gx.DrawImage(mask, new Rectangle(0, 0, _self.Width, _self.Height), 0, 0, mask.Width, mask.Height, GraphicsUnit.Pixel, imageAttr);
        }

        public static Bitmap[] GetBitmaps(this ImageFileData[] _self)
        {
            List<Bitmap> ret = new List<Bitmap>();
            foreach (var a in _self)
            {
                ret.Add(a.image);
            }
            return ret.ToArray();
        }
        public static Bitmap[] GetBitmaps(this List<ImageTrainData> _self)
        {
            List<Bitmap> ret = new List<Bitmap>();
            foreach (var a in _self)
            {
                ret.Add(a.image);
            }
            return ret.ToArray();
        }
        public static int[] GetLabelIndexs(this List<ImageTrainData> _self)
        {
            List<int> ret = new List<int>();
            foreach (var a in _self)
            {
                ret.Add(a.labelIndex);
            }
            return ret.ToArray();
        }
        public static String[] GetLabels(this List<ImageTrainData> _self)
        {
            List<String> ret = new List<String>();
            foreach (var a in _self)
            {
                ret.Add(a.label);
            }
            return ret.ToArray();
        }
        public static int GetNumOutput(this List<ImageTrainData> _self)
        {
            var item = _self.Max(x => x.labelIndex);
            return item + 1;
        }
        public static double[][] GetOutputs(this List<ImageTrainData> _self, int numOutput)
        {
            int[] labelIndexs = _self.GetLabelIndexs();
            double[][] outputs = new double[_self.Count][];
            for (int i = 0; i < _self.Count; i++)
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
        public static TrainDataInfo[] GetInfo(this List<ImageTrainData> _self, int numOutput)
        {
            TrainDataInfo[] ret = new TrainDataInfo[numOutput];
            for(int i=0;i< numOutput; i++)
            {
                ret[i] = new TrainDataInfo() { index = i, count = 0, label = "" };
            }
            for (int i = 0; i < _self.Count; i++)
            {
                ret[_self[i].labelIndex].count++;
                ret[_self[i].labelIndex].label = _self[i].label;
            }
            return ret;
        }
        public static List<ImageTrainData> BalanceData(this List<ImageTrainData> _self)
        {
            Random rand = new Random();
            List<ImageTrainData> ret = new List<ImageTrainData>(_self);
            TrainDataInfo[] infos = _self.GetInfo(_self.GetNumOutput());
            int maxCount = infos.Max(x => x.count);
            for(int i = 0; i < infos.Length; i++)
            {
                for (int j = infos[i].count; j < maxCount; j++)
                {
                    //Random _self for that index
                    while (true)
                    {
                        int index = rand.Next(_self.Count);
                        if (_self[index].labelIndex == i)
                        {
                            ret.Add(_self[index].Clone());
                            break;
                        }
                    }
                }
            }
            return ret;
        }

        public static List<ImageTrainData> SplitTestData(this List<ImageTrainData> _self,int num)
        {
            Random rand = new Random();
            //List<ImageTrainData> ret = _self.GetRange(0, num);
            //_self.RemoveRange(0, num);
            int numOutput = _self.GetNumOutput();
            List<ImageTrainData> ret = new List<ImageTrainData>();
            while(ret.Count() < num)
            {
                for(int i = 0; i < numOutput; i++)
                {
                    while (true)
                    {
                        int index = rand.Next(_self.Count);
                        if (_self[index].labelIndex == i)
                        {
                            ret.Add(_self[index]);
                            _self.RemoveAt(index);
                            break;
                        }
                    }
                }
            }
            return ret;
        }
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
