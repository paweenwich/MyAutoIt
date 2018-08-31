using Accord.Imaging;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Kernels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyAutoIt
{
    public partial class AutoIt : Form
    {
        MyLogger logger;
        AutoItConfigure configure;
        String dataPath = Application.StartupPath + @"\Linage2";
        //String imagePath = Application.StartupPath + @"\Linage2\Main";

        public AutoIt()
        {
            InitializeComponent();
            logger = new MyLogger();
            logger.logStr("Start");
            configure = JsonConvert.DeserializeObject<AutoItConfigure>(File.ReadAllText(dataPath + @"\AutoIt.json"));
            logger.logStr(JsonConvert.SerializeObject(configure,Formatting.Indented));
        }

        public List<ImageTrainData> GetAllTrainImageData(String folder)
        {
            List<ImageTrainData> ret = new List<ImageTrainData>();
            for(int i = 0; i < configure.trainFolders.Length; i++)
            {
                String folderName = folder + @"\" + configure.trainFolders[i];
                ImageFileData[] images = GetImagesFromDir(folderName, configure.imageFilter, configure.imageFilterOut);
                foreach(ImageFileData img in images)
                {
                    ret.Add(
                            new ImageTrainData()
                            {
                                label = configure.trainFolders[i],
                                labelIndex = i,
                                image = img.image,
                                fileName = img.fileName,
                            }
                    );
                }
            }
            return ret;
        }

        public ImageFileData[] GetImagesFromDir(String folder,String filter="*.png", String filterOut = "mask.png")
        {
            List<ImageFileData> ret = new List<ImageFileData>();
            DirectoryInfo d = new DirectoryInfo(folder);
            FileInfo[] Files = d.GetFiles(filter);
            foreach (FileInfo file in Files)
            {
                //Console.WriteLine(file.FullName);
                if (file.Name.Equals(filterOut, StringComparison.InvariantCultureIgnoreCase))
                {
                    logger.logStr("Skip " + file.FullName);
                    continue;
                }
                Bitmap bmp = (Bitmap)Bitmap.FromFile(file.FullName);
                ret.Add(new ImageFileData() {fileName = file.FullName,image = bmp });
            }
            return ret.ToArray();
        }

        private void test1ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            List<ImageTrainData> trainData = GetAllTrainImageData(dataPath);
            var bow = BagOfVisualWords.Create(numberOfWords: 10);
            var images = trainData.GetBitmaps();
            bow.Learn(images);
            double[][] features = bow.Transform(images);
            int[] labelIndexs = trainData.GetLabelIndexs();
            String[] labels = trainData.GetLabels();
            /*var teacher = new SequentialMinimalOptimization<Gaussian>()
            {
                //Complexity = 100 // make a hard margin SVM
                UseComplexityHeuristic = true,
                UseKernelEstimation = true

            };*/

            var teacher = new MulticlassSupportVectorLearning<Linear>()
            {
                // Configure the learning algorithm to use SMO to train the
                //  underlying SVMs in each of the binary class subproblems.
                Learner = (param) => new SequentialMinimalOptimization<Linear>()
                {
                    // If you would like to use other kernels, simply replace
                    // the generic parameter to the desired kernel class, such
                    // as for example, Polynomial or Gaussian:

                    Kernel = new Linear() // use the Linear kernel
                }
            };



            // Obtain a learned machine

            var svm = teacher.Learn(features, labelIndexs);

            logger.logStr("Learn Done");
            //bow.Save();
            Accord.IO.Serializer.Save(svm, dataPath + @"\train.svm");
        }

        private void test2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<ImageTrainData> trainData = GetAllTrainImageData(dataPath);
            var bow = BagOfVisualWords.Create(numberOfWords: 10);
            var images = trainData.GetBitmaps();
            bow.Learn(images);
            double[][] features = bow.Transform(images);
            int[] labelIndexs = trainData.GetLabelIndexs();
            String[] labels = trainData.GetLabels();

            var machine = Accord.IO.Serializer.Load<MulticlassSupportVectorMachine<Linear>>(dataPath + @"\train.svm");
            int[] prediction = machine.Decide(features);
            double[] scores = machine.Score(features);
            double[][] prob =  machine.Probabilities(features);
            for (int i=0;i < prediction.Length; i++)
            {
                logger.logStr(trainData[i].ToString() + " " + prediction[i] + " " + scores[i] + " " + prob[i][prediction[i]]);
            }
            double error = new ZeroOneLoss(labelIndexs).Loss(prediction);
            logger.logStr("" + error);
            //ImageFileData[] images = GetImagesFromDir(imagePath);
            //double[][] features = bow.Transform(images.GetBitmaps());
            //logger.logStr("" + features[0].Length);


        }

        private void test3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(var a in GetAllTrainImageData(dataPath))
            {
                logger.logStr(a.ToString());
            }
        }
    }

    public class ImageFileData
    {
        public String fileName;
        public Bitmap image;
    }

    public class ImageTrainData: ImageFileData
    {
        public String label;
        public int labelIndex;
        public override String ToString()
        {
            return String.Format("{0} {1} {2}", label, labelIndex, fileName);
        }
    }

    public class AutoItConfigure
    {
        public String[] trainFolders;
        public String imageFilter = "*.png";
        public String imageFilterOut = "mask.png";
    }

    public static class ImageTrainDataExtension
    {
        public static Bitmap[] GetBitmaps(this ImageFileData[] _self)
        {
            List<Bitmap> ret = new List<Bitmap>();
            foreach(var a in _self)
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
    }
}
