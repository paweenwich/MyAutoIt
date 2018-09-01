using Accord;
using Accord.Imaging;
using Accord.MachineLearning;
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
        String configPath = Application.StartupPath + @"\..\..";
        String testDataPath = Application.StartupPath + @"\Linage2Test";
        String dataPath = Application.StartupPath + @"\Linage2";
        //String imagePath = Application.StartupPath + @"\Linage2\Main";

        public AutoIt()
        {
            InitializeComponent();
            Accord.Math.Random.Generator.Seed = 0;
            logger = new MyLogger(txtDebug);
            logger.logStr("Start");
            String configFile = configPath + @"\AutoIt.json";
            try
            {
                
                configure = JsonConvert.DeserializeObject<AutoItConfigure>(File.ReadAllText(configFile));
                
            }catch(Exception ex)
            {
                logger.logError("Config not found [" + configFile + "]");
                configure = new AutoItConfigure();
            }
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

        public ImageFileData[] GetImagesFromDir(String folder, String filter = "*.png", String filterOut = "mask.png")
        {
            List<ImageFileData> ret = new List<ImageFileData>();
            DirectoryInfo d = new DirectoryInfo(folder);
            if (d.Exists) { 
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
                    ret.Add(new ImageFileData() { fileName = file.FullName, image = bmp });
                }
            }
            return ret.ToArray();
        }

        private void test1ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            List<ImageTrainData> trainData = GetAllTrainImageData(dataPath);
            var bow = BagOfVisualWords.Create(numberOfWords: 10);
            //var bow = BagOfVisualWords.Create(new BinarySplit(10));
            
            var images = trainData.GetBitmaps();
            //bow.Learn(images,new double[] {1,1,1,1, 1, 1, 1, 1, 1, 1, 1, 1, 4 });
            bow.Learn(images);
            Accord.IO.Serializer.Save(bow, dataPath + @"\train.bow");
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
            TestData(bow, svm, trainData);
        }

        private void test2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //List<ImageTrainData> trainDataSet = GetAllTrainImageData(dataPath);
            List<ImageTrainData> testDataSet = GetAllTrainImageData(testDataPath);
            var bow = Accord.IO.Serializer.Load<BagOfVisualWords>(dataPath + @"\train.bow");
            bow.Show();
            var machine = Accord.IO.Serializer.Load<MulticlassSupportVectorMachine<Linear>>(dataPath + @"\train.svm");
            TestData(bow, machine, testDataSet);

        }

        private void test3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(var a in GetAllTrainImageData(dataPath))
            {
                logger.logStr(a.ToString());
            }
        }

        public void TestData(dynamic bow, MulticlassSupportVectorMachine<Linear> machine, List<ImageTrainData> trainData)
        {
            var images = trainData.GetBitmaps();
            double[][] features = bow.Transform(images);
            int[] labelIndexs = trainData.GetLabelIndexs();
            String[] labels = trainData.GetLabels();

            int[] prediction = machine.Decide(features);
            double[] scores = machine.Score(features);
            double[][] prob = machine.Probabilities(features);
            for (int i = 0; i < prediction.Length; i++)
            {
                if (prediction[i] != trainData[i].labelIndex)
                {
                    logger.logError(trainData[i].ToString() + " " + prediction[i] + " " + scores[i] + " " + prob[i][prediction[i]]);
                }
                else
                {
                    logger.logStr(trainData[i].ToString() + " " + prediction[i] + " " + scores[i] + " " + prob[i][prediction[i]]);
                }
            }
            double error = new ZeroOneLoss(labelIndexs).Loss(prediction);
            logger.logStr("" + error);
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
        public String[] trainFolders = new string[] { };
        public String imageFilter = "*.png";
        public String imageFilterOut = "mask.png";
        public Rectangle[] areas = new Rectangle[] {
            new Rectangle(0,0,1280,720)
        }; 
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
