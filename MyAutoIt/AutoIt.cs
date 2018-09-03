using Accord;
using Accord.Imaging;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Math.Optimization.Losses;
using Accord.Neuro;
using Accord.Neuro.Learning;
using Accord.Statistics.Kernels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
        List<ImageTrainData> trainData;
        List<ImageTrainData> testData;
        //String imagePath = Application.StartupPath + @"\Linage2\Main";
        dynamic mainBow;
        dynamic mainNetwork;
        int bowSize = 100;
        Bitmap mask;
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

            }
            catch (Exception ex)
            {
                logger.logError("Config not found [" + configFile + "]");
                configure = new AutoItConfigure();
            }
            mask = Utils.CreateMaskBitmap(new Size(1280, 720), configure.areas);
            logger.logStr(JsonConvert.SerializeObject(configure, Formatting.Indented));
            foreach (String s in configure.trainFolders) {
                cmbFolder.Items.Add(s);
            }
        }

        public List<ImageTrainData> GetAllTrainImageData(String folder)
        {
            List<ImageTrainData> ret = new List<ImageTrainData>();
            for (int i = 0; i < configure.trainFolders.Length; i++)
            {
                String folderName = folder + @"\" + configure.trainFolders[i];
                ImageFileData[] images = GetImagesFromDir(folderName, configure.imageFilter, configure.imageFilterOut);
                foreach (ImageFileData img in images)
                {
                    img.image.ApplyMask(mask);
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
            if (d.Exists)
            {
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
            //List<ImageTrainData> trainData = GetAllTrainImageData(dataPath);
            int[] labelIndexs = trainData.GetLabelIndexs();
            String[] labels = trainData.GetLabels();
            var bow = BagOfVisualWords.Create(numberOfWords: 10);
            //bow.Detector.Threshold = 0.0001;
            //var bow = BagOfVisualWords.Create(new BinarySplit(10));

            var images = trainData.GetBitmaps();
            bow.Learn(images);

            double[][] features = bow.Transform(images);
            //bow.Learn(images,new double[] {1,1,1,1, 1, 1, 1, 1, 1, 1, 1, 1, 4 });
            Accord.IO.Serializer.Save(bow, dataPath + @"\train.bow");

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
            {
                String fileName2 = dataPath + @"\tmp\error\yf3gsclgd2h.png";
                String ret = classifyImage(fileName2, configure.trainFolders);
                logger.logStr(ret);
            }
            {
                String fileName2 = @"D:\kwang\ccharp\MyAutoIt\MyAutoItGit\MyAutoIt\bin\Debug\Linage2\Main\0ruksbay3lf.png";
                String ret = classifyImage(fileName2, configure.trainFolders);
                logger.logStr(ret);

            }
        }


        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainBow = Accord.IO.Serializer.Load<BagOfVisualWords>(dataPath + String.Format(@"\train-{0}.bow", bowSize));
            mainNetwork = Accord.IO.Serializer.Load<ActivationNetwork>(dataPath + String.Format(@"\train-{0}.net", mainBow.NumberOfOutputs));
            logger.logStr("Loaded");
        }

        private void testToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if(mainBow == null)
            {
                loadToolStripMenuItem_Click(sender,e);
            }
            int testErrCount = TestNetwork(mainBow, mainNetwork, testData);
            int trainErrCount = TestNetwork(mainBow, mainNetwork, trainData);
            logger.logStr(String.Format("train {0} test {1}", trainErrCount, testErrCount));
        }

        private void testCaptureToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            String fileName2 = dataPath + @"\tmp\" + Path.GetRandomFileName().Replace(".", "") + ".png";
            Utils.AdbCpatureToFile(fileName2);
            logger.logStr("Capture " + fileName2);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            String ret = classifyImage(fileName2, configure.trainFolders);
            stopwatch.Stop();
            long elapsed_time = stopwatch.ElapsedMilliseconds;
            logger.logStr(String.Format("{0:0.00}", elapsed_time/1000.0));
            if((ret != cmbFolder.Text) && (cmbFolder.Text != "") )
            {
                String saveFile = dataPath + @"\tmp\error\" + Path.GetRandomFileName().Replace(".", "") + ".png";
                File.Copy(fileName2, saveFile);
                logger.logError("Save " + saveFile);

            }
        }

        private void loadTrainDataToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            trainData = GetAllTrainImageData(dataPath);
            //foreach (var t in trainData)
            //{
            //    logger.logStr(t.ToString());
            //}
            trainData = trainData.BalanceData();
            trainData.Shuffle();
            testData = trainData.SplitTestData((int)(0.1* trainData.Count()));
            TrainDataInfo[] infos = trainData.GetInfo(configure.trainFolders.Count());
            TrainDataInfo[] testinfos = testData.GetInfo(configure.trainFolders.Count());
            logger.logStr(Utils.ToJsonString(infos, true));
            logger.logStr(Utils.ToJsonString(testinfos, true));
            //foreach (var t in trainData)
            //{
            //    logger.logStr(t.ToString());
            //}

        }

        private void bowsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //List<ImageTrainData> trainData = GetAllTrainImageData(dataPath);
            //int[] labelIndexs = trainData.GetLabelIndexs();
            //String[] labels = trainData.GetLabels();
            //for (int i = 10; i <= 100; i += 10)
            //{
                var bow = BagOfVisualWords.Create(numberOfWords: bowSize);
                var images = trainData.GetBitmaps();
                bow.Learn(images);
                Accord.IO.Serializer.Save(bow, dataPath + String.Format(@"\train-{0}.bow", bowSize));
                logger.logStr("Done " + bowSize);
                Application.DoEvents();
            //}

        }

        private void trainToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //List<ImageTrainData> testDataSet = GetAllTrainImageData(testDataPath);

            int[] labelIndexs = trainData.GetLabelIndexs();
            String[] labels = trainData.GetLabels();
            var images = trainData.GetBitmaps();
            var bow = Accord.IO.Serializer.Load<BagOfVisualWords>(dataPath + String.Format(@"\train-{0}.bow", bowSize));
            double[][] features = bow.Transform(images);
            int numOutput = trainData.GetNumOutput();


            var function = new SigmoidFunction();
            var network = new ActivationNetwork(function, bow.NumberOfOutputs, 20, numOutput);
            new NguyenWidrow(network).Randomize();



            // Teach the network using parallel Rprop:
            var teacher = new ParallelResilientBackpropagationLearning(network);
            //var teacher = new BackPropagationLearning(network);

            
            //creat output
            double[][] outputs = trainData.GetOutputs(numOutput);
            double avgError = 10000.0;
            double prevError = avgError;
            double bestError = avgError;
            int errorCount = 0;
            while ((errorCount < 10) && (avgError > 0.00001))
            {
                Application.DoEvents();
                double[] errors = new double[10];
                for (int i = 0; i < 10; i++)
                {
                    errors[i] = teacher.RunEpoch(features, outputs);
                }
                avgError = errors.Average();
                if (prevError > avgError)
                {
                    logger.logStr(String.Format("{0} {1} #{2}", avgError, prevError, errorCount));
                    prevError = avgError;
                    //save best error
                    if (bestError > avgError)
                    {
                        bestError = avgError;
                        Accord.IO.Serializer.Save(network, dataPath + String.Format(@"\train-{0}.net", bow.NumberOfOutputs));
                    }
                    //Accord.IO.Serializer.Save(teacher, dataPath + String.Format(@"\train-{0}.teacher", bow.NumberOfOutputs));
                }
                else
                {
                    logger.logStr(String.Format("{0}", avgError));
                    //network = Accord.IO.Serializer.Load<ActivationNetwork>(dataPath + String.Format(@"\train-{0}.net", bow.NumberOfOutputs));
                    //teacher = Accord.IO.Serializer.Load<ParallelResilientBackpropagationLearning>(dataPath + String.Format(@"\train-{0}.teacher", bow.NumberOfOutputs));
                    prevError = 10000.0;
                    //teacher.
                    //break;
                    errorCount++;
                }
                Application.DoEvents();
                //int errCount = TestNetwork(bow, network, trainData,true);
                //if(errCount == 0)
                //{
                //int testErrorCount = TestNetwork(bow, network, testDataSet,true);
                //logger.logStr(String.Format("{0}", testErrorCount));
                //}
            }
            logger.logStr("Done " + bestError);
            //Accord.IO.Serializer.Save(network, dataPath + String.Format(@"\train-{0}.net", bow.NumberOfOutputs));
            //TestNetwork(bow, network, trainData);
            //TestNetwork(bow, network, testData);

        }

        public int TestNetwork(dynamic bow, dynamic network, List<ImageTrainData> trainData, bool flgSilence = false)
        {
            var images = trainData.GetBitmaps();
            //double[][] features = bow.Transform(images);
            int[] labelIndexs = trainData.GetLabelIndexs();
            String[] labels = trainData.GetLabels();
            int errorCount = 0;
            for (int i = 0; i < images.Length; i++)
            {
                double[] feature = bow.Transform(images[i]);
                double[] answer = network.Compute(feature);

                int expected = labelIndexs[i];
                int actual; answer.Max(out actual);
                if (actual == expected)
                {
                    //if (flgSilence == false) logger.logStr(trainData[i].ToString() + " " + actual);
                }
                else
                {
                    if (flgSilence == false) logger.logError(trainData[i].ToString() + " " + actual);
                    errorCount++;
                }
            }
            return errorCount;
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

        public String classifyImage(String fileName, String[] map)
        {
            Bitmap bmp = (Bitmap)Bitmap.FromFile(fileName);
            bmp.ApplyMask(mask);
            //var images = trainData.GetBitmaps();
            double[] features = mainBow.Transform(bmp);
            double[] answer = mainNetwork.Compute(features);
            int actual;
            answer.Max(out actual);
            logger.logStr("classifyImage "+ map[actual] + " " + actual);
            return map[actual];
        }

        private void autoCaptureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = autoCaptureToolStripMenuItem.Checked;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            testCaptureToolStripMenuItem_Click_1(sender,e);
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

}
