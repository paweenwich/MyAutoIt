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
using LuaInterface;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyAutoIt
{
    public interface IAutoBot
    {
        void log(String data);
        void ClickAt(int x,int y);
        void AddTask(String cmd,int time);
        String CaptureScreen();
    }

    public interface ImageClassifier
    {
        int Evaluate(ImageTrainDataSet trainData);
        String Classify(String fileName, String[] map);
    }

    public class BowImageClassifier : ImageClassifier
    {
        public dynamic bow;
        public dynamic network;
        public Bitmap mask;
        public void Init(String bowFile,String networkFile, Bitmap mask)
        {
            bow = Accord.IO.Serializer.Load<BagOfVisualWords>(bowFile);
            network = Accord.IO.Serializer.Load<ActivationNetwork>(networkFile);
            this.mask = mask;
        }
        public int Evaluate(ImageTrainDataSet trainData)
        {
            double[][] features = trainData.GetFeature(bow, mask);
            int[] labelIndexs = trainData.GetLabelIndexs();
            String[] labels = trainData.GetLabels();
            int errorCount = 0;
            for (int i = 0; i < trainData.Count(); i++)
            {
                //double[] feature = bow.Transform(images[i]);
                double[] answer = network.Compute(features[i]);

                int expected = labelIndexs[i];
                int actual; answer.Max(out actual);
                if (actual != expected)
                { 
                    errorCount++;
                }
            }
            return errorCount;

        }

        public String Classify(String fileName, String[] map)
        {
            Bitmap bmp = (Bitmap)Bitmap.FromFile(fileName);
            bmp.ApplyMask(mask);
            double[] features = bow.Transform(bmp);
            double[] answer = network.Compute(features);
            int actual;
            answer.Max(out actual);
            //logger.logStr("classifyImage " + map[actual] + " " + actual);
            bmp.Dispose();
            return map[actual];
        }

        public byte[] ComputeHash(ImageTrainDataSet trainData)
        {
            return trainData.MD5Feature(bow, mask);
        }
    }


    public partial class AutoIt : Form, IAutoBot
    {
        MyLogger logger;
        AutoItConfigure configure;
        String configPath = Application.StartupPath + @"\..\..";
        String testDataPath = Application.StartupPath + @"\Linage2Test";
        String dataPath = Application.StartupPath + @"\Linage2";
        String scriptPath = Application.StartupPath + @"\..\..";
        String scriptFile = "AutoIt.lua";
        ImageTrainDataSet trainData;
        ImageTrainDataSet testData;
        //String imagePath = Application.StartupPath + @"\Linage2\Main";
        BowImageClassifier imgClassifier;
        //dynamic mainBow;
        //dynamic mainNetwork;
        int bowSize = 100;
        Bitmap mask;
        public Lua lua;
        public AutoIt()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
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
            foreach (String s in configure.trainFolders)
            {
                cmbFolder.Items.Add(s);
            }
            Reload();
        }

        public ImageTrainDataSet GetAllTrainImageData(String folder)
        {
            ImageTrainDataSet ret = new ImageTrainDataSet();
            for (int i = 0; i < configure.trainFolders.Length; i++)
            {
                String folderName = folder + @"\" + configure.trainFolders[i];
                ImageFileData[] images = GetImagesFromDir(folderName, configure.imageFilter, configure.imageFilterOut);
                foreach (ImageFileData img in images)
                {
                    //img.image.ApplyMask(mask);
                    ret.Add(

                            new ImageTrainData()
                            {
                                label = configure.trainFolders[i],
                                labelIndex = i,
                                //image = img.image,
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
                    //Bitmap bmp = (Bitmap)Bitmap.FromFile(file.FullName);
                    ret.Add(new ImageFileData() { fileName = file.FullName });
                }
            }
            return ret.ToArray();
        }

        public void Reload()
        {
            lua = new Lua();
            LoadScript();
        }
        public void LoadScript()
        {
            //String lua_script_path = Application.StartupPath + @"\Script\";
            lua["bot"] = this;
            lua["script_path"] = scriptPath;
            logger.logStr("LoadScript: Loading " + scriptFile);
            try
            {
                lua.DoFile(scriptPath + @"\" + scriptFile);
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
            }
            logger.logStr("LoadScript: Done");
        }
        public void ExecScript(String cmd)
        {
            try
            {
                lua.DoString(cmd);
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
            }
        }

        public void log(string data)
        {
            logger.logStr(data);
        }

        public void ClickAt(int x, int y)
        {
            Utils.AdbMouseClick(x, y);
        }

        public void AddTask(string cmd, int time)
        {
            logger.logStr("NotImplementedException AddTask " + cmd + " " + time);
            //throw new NotImplementedException();
        }

        public String CaptureScreen()
        {
            String fileName = dataPath + @"\tmp\" + Path.GetRandomFileName().Replace(".", "") + ".png";
            Utils.AdbCpatureToFile(fileName);
            return fileName;
        }

        public void TestAndCapture(String expectAnswer = "")
        {
            String fileName2 = CaptureScreen();
            logger.logStr("Capture " + fileName2);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            String ret = classifyImage(fileName2, configure.trainFolders);
            logger.logStr("[" + ret + "]");
            stopwatch.Stop();
            long elapsed_time = stopwatch.ElapsedMilliseconds;
            logger.logStr(String.Format("{0:0.00}", elapsed_time / 1000.0));
            if ((ret != expectAnswer) && (expectAnswer != ""))
            {
                //String saveFile = dataPath + @"\tmp\error\" + Path.GetRandomFileName().Replace(".", "") + ".png";
                //testDataPath
                String saveFile = testDataPath + @"\" + expectAnswer + @"\" + Path.GetRandomFileName().Replace(".", "") + ".png";
                File.Copy(fileName2, saveFile);
                logger.logError("Save " + saveFile);
            }else
            {
                File.Delete(fileName2);
            }
        }

        private void test1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void test2ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void test3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }


        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imgClassifier = new BowImageClassifier();
            imgClassifier.Init(
                dataPath + String.Format(@"\train-{0}.bow", bowSize),
                dataPath + String.Format(@"\train-{0}.net", bowSize),
                mask
            );
            logger.logStr("Loaded");
        }

        private void testToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if(imgClassifier == null)
            {
                loadToolStripMenuItem_Click(sender,e);
            }
            int testErrCount = imgClassifier.Evaluate(testData);
            int trainErrCount = imgClassifier.Evaluate(trainData);
            logger.logStr(String.Format("train {0} test {1}", trainErrCount, testErrCount));
        }

        private void testCaptureToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            TestAndCapture(cmbFolder.Text);
        }

        private void loadTrainDataToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            trainData = GetAllTrainImageData(dataPath);
            trainData = trainData.BalanceData();
            trainData.Shuffle();
            testData = trainData.SplitTestData((int)(0.1* trainData.Count()));
            TrainDataInfo[] infos = trainData.GetInfo(configure.trainFolders.Count());
            TrainDataInfo[] testinfos = testData.GetInfo(configure.trainFolders.Count());
            logger.logStr(Utils.ToJsonString(infos, true));
            logger.logStr(Utils.ToJsonString(testinfos, true));
        }

        private void bowsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            var bow = BagOfVisualWords.Create(numberOfWords: bowSize);
            var images = trainData.GetBitmaps(mask);
            bow.Learn(images);
            Accord.IO.Serializer.Save(bow, dataPath + String.Format(@"\train-{0}.bow", bowSize));
            logger.logStr("Done " + bowSize);
            Application.DoEvents();
        }

        private void trainToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ImageTrainDataSet testDataSet = GetAllTrainImageData(testDataPath);
            testDataSet.flgCache = true;

            int[] labelIndexs = trainData.GetLabelIndexs();
            String[] labels = trainData.GetLabels();
            var bow = Accord.IO.Serializer.Load<BagOfVisualWords>(dataPath + String.Format(@"\train-{0}.bow", bowSize));
            double[][] features = trainData.GetFeature(bow, mask);
            int numOutput = trainData.GetNumOutput();
            var function = new SigmoidFunction();
            logger.logStr("Start Training");
            bool flgFound = false;
            while (flgFound == false)
            {
                var network = new ActivationNetwork(function, bow.NumberOfOutputs, 20, numOutput);
                new NguyenWidrow(network).Randomize();
                var teacher = new ParallelResilientBackpropagationLearning(network);

                //creat output
                double[][] outputs = trainData.GetOutputs(numOutput);
                double avgError = 10000.0;
                double prevError = avgError;
                double bestError = avgError;
                int errorCount = 0;
                while ((errorCount < 3) && (avgError > 0.00001))
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
                        int trainError = imgClassifier.Evaluate(trainData);
                        int testError = imgClassifier.Evaluate(testData);
                        int testSetError = imgClassifier.Evaluate(testDataSet);
                        logger.logStr(String.Format("{0} {1} {2} {3} {4} #{5}", avgError, prevError, trainError,testError, testSetError, errorCount));
                        prevError = avgError;
                        //save best error
                        if (bestError > avgError)
                        {
                            bestError = avgError;
                            Accord.IO.Serializer.Save(network, dataPath + String.Format(@"\train-{0}.net", bow.NumberOfOutputs));
                        }
                        if(trainError + testError + testSetError == 0)
                        {
                            flgFound = true;
                            Accord.IO.Serializer.Save(network, dataPath + String.Format(@"\train-{0}.net", bow.NumberOfOutputs));
                            break;
                        }
                    }
                    else
                    {
                        logger.logStr(String.Format("{0}", avgError));
                        prevError = 10000.0;
                        errorCount++;
                    }
                    Application.DoEvents();
                }
                logger.logStr("Done " + bestError);
            }
        }
        /*
        public int TestNetwork(ImageClassifier imgClassifier, ImageTrainDataSet trainData, bool flgSilence = false)
        {
            double[][] features = trainData.GetFeature(imgClassifier.bow, mask);
            int[] labelIndexs = trainData.GetLabelIndexs();
            String[] labels = trainData.GetLabels();
            int errorCount = 0;
            for (int i = 0; i < trainData.Count(); i++)
            {
                //double[] feature = bow.Transform(images[i]);
                double[] answer = network.Compute(features[i]);

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
        }*/

        public String classifyImage(String fileName, String[] map)
        {
            return imgClassifier.Classify(fileName, map);
        }

        private void autoCaptureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = autoCaptureToolStripMenuItem.Checked;
            cmbFolder.Enabled = (!timer1.Enabled);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            String answer = cmbFolder.Text;
            new Thread(() =>
            {
                TestAndCapture(answer);
            }).Start();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadScript();
        }

        private void testWithTestSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageTrainDataSet testDataSet = GetAllTrainImageData(testDataPath);
            testDataSet.flgCache = true;
            byte[] hash = imgClassifier.ComputeHash(testDataSet);
            logger.logStr(hash.ToHex());
            int testErrCount = imgClassifier.Evaluate(testDataSet);
            logger.logStr(String.Format("test {0}", testErrCount));

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                ExecScript("if Auto ~= nil then Auto() end");
            }).Start();
        }

        private void autoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer2.Enabled = autoToolStripMenuItem.Enabled;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)13)
            {
                String cmd = txtInput.Text;
                logger.logStr(">> [" + txtInput.Text + "]");
                new Thread(() =>
                {
                    ExecScript(cmd);
                }).Start();
            }
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
