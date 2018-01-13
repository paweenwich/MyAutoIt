using Magic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TestStack.White.UIItems.WindowItems;
using Accord.Math;
using Accord.MachineLearning;
using Accord.Imaging;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Accord.Math.Optimization.Losses;

namespace MyAutoIt
{
    public partial class Linage2 : Form

    {
        public String dataPath; // = @"D:\Data\Linage2\";
        public String[] windowName = {"BlueStacks", "Bluestacks" };
        public String className = "BlueStacksApp";
        public String autoClickPointsFileName = "autoClickPoints.txt";
        public String clickPointsFileName = "clickPoints.txt";
        
        public SimpleImageClassifier screenClassifier = new SimpleImageClassifier();
        //public String[] screenList = { "Main", "Move", "QuestSkip", "QuestAccept", "QuestComplete", "QuestSingleConfirm", "QuestDo" };
        public Size screenSize = new Size(1049, 570); //1139 641
        //public Size screenSize = new Size(1139, 641);
        public IntPtr mainHwnd;
        public IntPtr screenHwnd;
        // will be load from file
        public Dictionary<String, SimpleImageClassifier> screenClassifiers = new Dictionary<string, SimpleImageClassifier>();
        public Dictionary<String, System.Drawing.Point> autoClickPoints = new Dictionary<String, System.Drawing.Point>();
        public Dictionary<String, System.Drawing.Point> clickPoints = new Dictionary<String, System.Drawing.Point>();
        public Linage2()
        {
            InitializeComponent();
            Console.SetOut(new RichTextWriter(txtDebug));
            dataPath = Application.StartupPath + @"\Linage2\";
            Reload();
        }
        public void Reload()
        {
            logClear();
            logDebug("Reload");
            LoadComboBox();
            LoadFeatures();
            LoadAutoPoints();
            LoadClickPoints();

            Dictionary<String, String> ret = Utils.AdbListDevice();
            cmbADBDevice.Items.Clear();
            foreach (String s in ret.Keys)
            {
                cmbADBDevice.Items.Add(new ComboDeviceItem(s, ret[s]));
                Console.WriteLine("Add ADB Device " + ret[s]);
            }
        }
        public SimpleImageClassifier LoadFeaturesFromDir(String featureDir)
        {
            logDebug("LoadFeaturesFromDir " + featureDir);
            SimpleImageClassifier screenClassifier = new SimpleImageClassifier();
            DirectoryInfo d = new DirectoryInfo(featureDir);
            DirectoryInfo[] dInfo = d.GetDirectories();
            foreach (DirectoryInfo dir in dInfo)
            {
                String screenName = dir.Name;
                String fileName = featureDir + screenName + ".txt";
                if (File.Exists(fileName))
                {
                    screenClassifier.AddFromFile(fileName);
                }
            }
            return screenClassifier;
        }
        public void LoadFeatures()
        {
            screenClassifier = LoadFeaturesFromDir(dataPath);
            screenClassifiers = new Dictionary<string, SimpleImageClassifier>();
            DirectoryInfo d = new DirectoryInfo(dataPath);
            DirectoryInfo[] dInfo = d.GetDirectories();
            foreach (DirectoryInfo dir in dInfo)
            {
                screenClassifiers[dir.Name] = LoadFeaturesFromDir(dir.FullName + @"\");
            }
        }
        public void LoadComboBox()
        {
            DirectoryInfo d = new DirectoryInfo(dataPath);
            DirectoryInfo[] dInfo = d.GetDirectories();
            cmbScreenType.Items.Clear();
            foreach (DirectoryInfo dir in dInfo)
            {
                cmbScreenType.Items.Add(dir.Name);
                DirectoryInfo[] subDirs = dir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    cmbScreenType.Items.Add(dir.Name + @"\" + subDir.Name);
                }

            }
            if (cmbScreenType.Items.Count > 0)
            {
                cmbScreenType.Text = cmbScreenType.Items[0].ToString();
            }
            else
            {
                cmbScreenType.Text = "";
            }
        }

        public void LoadAutoPoints()
        {
            String data = File.ReadAllText(dataPath + autoClickPointsFileName);
            autoClickPoints = JsonConvert.DeserializeObject<Dictionary<String, System.Drawing.Point>>(data);
            logDebug("AutoClickPoints=" + autoClickPoints.Count);
        }

        public void LoadClickPoints()
        {
            String data = File.ReadAllText(dataPath + clickPointsFileName);
            clickPoints = JsonConvert.DeserializeObject<Dictionary<String, System.Drawing.Point>>(data);
            logDebug("ClickPoints=" + clickPoints.Count);
        }
        public void SaveAutoPoints()
        {
            File.WriteAllText(dataPath + autoClickPointsFileName, JsonConvert.SerializeObject(autoClickPoints));
        }


        public void ActiveBlueStackWindow()
        {
            Utils.SearchWindow("BlueStacks App Player", true);
        }

        public bool CorrectScreenSize()
        {
            return true;
        }

        public void logClear()
        {
            txtDebug.Clear();
        }

        public void logDebug(String str)
        {
            //Console.WriteLine(str);
            txtDebug.Focus();
            txtDebug.AppendText(str + "\n");
        }

        public void TestWhiteStack()
        {
            Process proc = Process.GetProcessesByName(windowName[0])[0];
            TestStack.White.Application app = TestStack.White.Application.Attach(proc);
            /*List<Window> windows = app.GetWindows();
            foreach(Window w in windows)
            {
                logDebug(w.Title);
            }*/
            Window w = app.GetWindow("BlueStacks App Player");
            logDebug("Found=" + w.Title);
            logDebug(w.Mouse.Location.ToString());
            w.Mouse.Location = new System.Windows.Point(50, 50);
            logDebug(w.Mouse.Location.ToString());
            //w.Mouse.Click();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Utils.AdbScroll(100,400,100,240);
            //logDebug("Button1 Click1 ");
            Process[] proc = Process.GetProcesses();
            foreach(Process p in proc)
            {
                Console.WriteLine(p);
            }

            IntPtr hwnd = Utils.GetWindowHandleByProcessName(windowName);
            if (hwnd != IntPtr.Zero)
            {
                Utils.SetForegroundWindow(hwnd);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Utils.IsUserAdministrator())
            {
                Utils.SYSTEM_INFO sys_info = new Utils.SYSTEM_INFO();
                Utils.GetSystemInfo(out sys_info);

                IntPtr proc_min_address = sys_info.minimumApplicationAddress;
                IntPtr proc_max_address = sys_info.maximumApplicationAddress;
                long proc_min_address_l = (long)proc_min_address;
                long proc_max_address_l = (long)proc_max_address;
                logDebug(String.Format("Min {0} Max {1}", proc_min_address_l, proc_max_address_l));


                BlackMagic bm = new BlackMagic();
                if (bm.OpenProcessAndThread(SProcess.GetProcessFromProcessName(windowName[0])))
                {
                    logDebug("Found");
                    Utils.MEMORY_BASIC_INFORMATION mem_basic_info = new Utils.MEMORY_BASIC_INFORMATION();

                    while (proc_min_address_l < proc_max_address_l)
                    {

                        // 28 = sizeof(MEMORY_BASIC_INFORMATION)
                        Utils.VirtualQueryEx(bm.ProcessHandle, proc_min_address, out mem_basic_info, 28);

                        //logDebug(mem_basic_info.ToString());
                        // if this memory chunk is accessible
                        if ((mem_basic_info.Protect == Utils.PAGE_READWRITE|| mem_basic_info.Protect == Utils.PAGE_READONLY) && mem_basic_info.State == Utils.MEM_COMMIT)
                        {
                            //logDebug(String.Format("Addr={0:X8} Size={1}", mem_basic_info.BaseAddress, mem_basic_info.RegionSize));
                            byte[] buffer = new byte[mem_basic_info.RegionSize];

                            // read everything in the buffer above
                            buffer = bm.ReadBytes((uint)mem_basic_info.BaseAddress, mem_basic_info.RegionSize);
                            //Utils.ReadProcessMemory((int)bm.ProcessHandle, mem_basic_info.BaseAddress, buffer, mem_basic_info.RegionSize, ref bytesRead);

                            MemoryStream ms = new MemoryStream(buffer);
                            BinaryReader br = new BinaryReader(ms);
                            while (ms.Position != ms.Length)
                            {
                                int data = br.ReadInt32();
                                if((data == 18248) || (data == 18622))
                                {
                                    int readData = bm.ReadInt((uint)(mem_basic_info.BaseAddress + ms.Position - 4));
                                    logDebug(String.Format("Found Addr={0:X8} Size={1}", mem_basic_info.BaseAddress + ms.Position - 4, readData));
                                }
                            }

                            // then output this in the file
                            //for (int i = 0; i < mem_basic_info.RegionSize; i++)
                            //    sw.WriteLine("0x{0} : {1}", (mem_basic_info.BaseAddress + i).ToString("X"), (char)buffer[i]);
                        }

                        // move to the next memory chunk
                        proc_min_address_l += mem_basic_info.RegionSize;
                        proc_min_address = new IntPtr(proc_min_address_l);
                    }

                    bm.Close();
                    logDebug("Done");
                }
            }else
            {
                logDebug("Need admin user");
            }
        }

        public void CaptureToFile(String folder)
        {
            /*Bitmap blueStackBmp3 = Utils.CaptureApplication(windowName);
            String fileName = folder + Path.GetRandomFileName().Replace(".", "") + ".png";
            blueStackBmp3.Save(fileName);
            logDebug("CaptureToFile:" + fileName);*/
            String fileName2 = folder + Path.GetRandomFileName().Replace(".", "") + ".png";
            Utils.AdbCpatureToFile(fileName2);
        }
        private void btnCaptureToFile_Click(object sender, EventArgs e)
        {
            if (cmbScreenType.Text != "")
            {
                CaptureToFile(dataPath + cmbScreenType.Text + @"\");
                logDebug("Captured");
            }
        }

        private void btnScreenCheck_Click(object sender, EventArgs e)
        {
            if (btnScreenCheck.Text == "ScreenCheck Start")
            {
                btnScreenCheck.Text = "ScreenCheck Stop";
            }
            else
            {
                btnScreenCheck.Text = "ScreenCheck Start";
                txtScreenStatus.Text = "";
            }
        }

        private void timer10_Tick(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (CorrectScreenSize())
            {
                if (btnScreenCheck.Text == "ScreenCheck Stop")
                {
                    //Bitmap bmp = Utils.CaptureApplication(windowName);
                    String folder = dataPath + @"tmp\";
                    String tmpfileName = folder + Path.GetRandomFileName().Replace(".", "") + ".png";
                    Utils.AdbCpatureToFile(tmpfileName);
                    Bitmap bmp = Utils.LoadAndDeleteBitmap(tmpfileName);
                    if (bmp != null)
                    {
                        ClassifyResult result = screenClassifier.Classify(bmp, 0.9);
                        if (result != null)
                        {
                            String screenType = result.label;
                            txtScreenStatus.Text = result.ToString() + " " + bmp.Size.ToString();
                            // check if we have sub type
                            if (screenClassifiers.ContainsKey(screenType))
                            {
                                SimpleImageClassifier subScreenClassifier = screenClassifiers[screenType];
                                ClassifyResult subResult = subScreenClassifier.Classify(bmp, 0.9);
                                if (subResult != null)
                                {
                                    txtScreenStatus.Text = result.ToString() + "/" + subResult.ToString();
                                    onScreenType(screenType, subResult.label);
                                }
                                else
                                {
                                    onScreenType(screenType);
                                }
                            }
                            else
                            {
                                onScreenType(screenType);
                            }
                        }
                        else
                        {
                            //String savefileName = dataPath + @"Unknown\" + Path.GetRandomFileName().Replace(".", "") + ".png";
                            //bmp.Save(savefileName);
                            //txtScreenStatus.Text = "Unknown" + " " + bmp.Size.ToString() + " bestmatch=" + screenClassifier.lastResult.ToString();
                        }
                    }
                }
            }
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cmbScreenType.Text != "")
            {
                SimpleImageClassifier imgClassifier = new SimpleImageClassifier();
                ImageFeatureVector fv = imgClassifier.CreateFeatureVector(dataPath + cmbScreenType.Text + @"\");
                if (fv == null)
                {
                    logDebug("fv=null");
                }
                else
                {
                    logDebug("fv=" + fv.Count);
                    fv.SaveToFile(dataPath + cmbScreenType.Text + ".txt");
                    logDebug("Save to " + dataPath + cmbScreenType.Text + ".txt");
                }
            }
            else
            {
                logDebug("ScreenType not select");
            }
        }

        private void verifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logClear();
            logDebug("Verify start");
            foreach (String featureName in screenClassifier.features.Keys)
            {
                String folder = dataPath + featureName + @"\";
                screenClassifier.Verify(folder, featureName);
            }
            logDebug("Verify end");
        }

        private void averageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageFeatureVector fv =  screenClassifier.AverageFeature(dataPath + cmbScreenType.Text + @"\");
            if (fv != null)
            {
                fv.SaveToFile(dataPath + cmbScreenType.Text + ".txt");
                Bitmap bmpMask = (Bitmap)Bitmap.FromFile(dataPath + cmbScreenType.Text + @"\mask.png");
                fv.ToBitmap(bmpMask).Save(dataPath + "avg.png");
                logDebug("avg done");
            }
            else
            {
                logDebug("mask not found");
            }
        }

        public void ClickByName(String pointName)
        {
            if (clickPoints.ContainsKey(pointName))
            {
                Utils.AdbMouseClick((int)clickPoints[pointName].X, (int)clickPoints[pointName].Y);
            }
        }

        public void AutoClick(String screenType)
        {
            // adb Y = Y * 1.35 - 47.5
            // adb X = X * 1.22 - 38
            if (autoClickPoints.ContainsKey(screenType))
            {
                /*Utils.RECT rect;
                Utils.GetWindowRect(Utils.lastHwnd, out rect);
                Utils.SetForegroundWindow(Utils.lastHwnd);
                Thread.Sleep(1000);
                Utils.MouseClick("LEFT", rect.left + autoClickPoints[screenType].X, rect.top + autoClickPoints[screenType].Y);*/
                Console.WriteLine("AutoClick {0}", screenType);
                Utils.AdbMouseClick((int) autoClickPoints[screenType].X,(int) autoClickPoints[screenType].Y);
            }
        }

        public void onScreenType(String screenType,String subScreenType = "")
        {
            if (chkAutoClick.Checked)
            {
                if (subScreenType == "")
                {
                    AutoClick(screenType);
                }
                else
                {
                    AutoClick(screenType + @"/" + subScreenType);
                }
            }
            switch (screenType)
            {
                case "QuestComplete":
                case "Dead":
                case "ClearDungeon":
                    {
                        IntPtr hwnd = Utils.GetWindowHandleByProcessName(windowName);
                        if (hwnd != IntPtr.Zero) { 
                            Utils.SetForegroundWindow(hwnd);
                        }
                    }
                break;
            }
        }

        private void saveAutoPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAutoPoints();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CENet ce = new CENet();
            ce.Connect();
            int handle = ce.OpenProcess(11277);
            ulong addr = 0x724183B0;
            int oldValue = ce.ReadInt(addr);
            Console.WriteLine("{0}",oldValue);
            ce.WriteInt(addr, 0);
            Console.WriteLine("{0}", ce.ReadInt(addr));
            ce.WriteInt(addr, oldValue);
            Console.WriteLine("{0}", ce.ReadInt(addr));
            /*
            byte[] wData = Utils.IntToByteArray(0);
            ce.WriteProcessMemory(, wData);
            byte[] data = ce.ReadProcessMemory(0x724183B0,4);
            Console.WriteLine(Utils.HexDump(data));*/
            ce.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var path = new DirectoryInfo(dataPath + "BoW");
            //var path = new DirectoryInfo(@"C:\tmp\Accord\Resources");
            Dictionary<string, Bitmap> train = new Dictionary<string, Bitmap>();
            Dictionary<string, Bitmap> test = new Dictionary<string, Bitmap>();
            Dictionary<string, Bitmap> all = new Dictionary<string, Bitmap>();
            Dictionary<string, int> labelIndex = new Dictionary<string, int>();

            int labelCount = 0;
            foreach (DirectoryInfo classFolder in path.EnumerateDirectories())
            {
                string name = classFolder.Name;
                labelIndex[name] = ++labelCount;
                logDebug(name + " " + labelIndex[name]);
                FileInfo[] files = Utils.GetFilesByExtensions(classFolder, ".jpg", ".png").ToArray();
                Vector.Shuffle(files);
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo file = files[i];
                    Bitmap image = (Bitmap)Bitmap.FromFile(file.FullName);
                    if ((i / (double)files.Length) < 0.7)
                    {
                        // Put the first 70% in training set
                        train.Add(file.FullName, image);
                    }
                    else
                    {
                        // Put the restant 30% in test set
                        test.Add(file.FullName, image);
                    }
                    all.Add(file.FullName, image);
                    logDebug(file.FullName);
                }
            }

            int numberOfWords = 36;
            IBagOfWords<Bitmap> bow;
            BinarySplit binarySplit = new BinarySplit(numberOfWords);

            // Create bag-of-words (BoW) with the given algorithm
            BagOfVisualWords surfBow = new BagOfVisualWords(binarySplit);

            // Compute the BoW codebook using training images only
            bow = surfBow.Learn(train.Values.ToArray());
            logDebug("BOW Done");

            List<double[]> lstInput = new List<double[]>();
            List<int> lstOutput = new List<int>();
            // Extract Feature in bother training and testing 
            foreach(String fileName  in train.Keys)
            {
                double[] featureVector = (bow as ITransform<Bitmap, double[]>).Transform(train[fileName]);
                //string featureString = featureVector.ToString(DefaultArrayFormatProvider.InvariantCulture);
                //logDebug(featureString);
                lstInput.Add(featureVector);
                //FileInfo fin = new FileInfo(fileName);
                String labelString = Path.GetFileName(Path.GetDirectoryName(fileName));
                lstOutput.Add(labelIndex[labelString]);
            }
            //this.ksvm = teacher.Learn(inputs, outputs);

            double[][] inputs = lstInput.ToArray();
            int[] outputs = lstOutput.ToArray();

            IKernel kernel = new ChiSquare();

            MulticlassSupportVectorLearning<IKernel> teacher = new MulticlassSupportVectorLearning<IKernel>()
            {
                Kernel = kernel,
                Learner = (param) =>
                {
                    return new SequentialMinimalOptimization<IKernel>()
                    {
                        Kernel = kernel,
                        Complexity = 1.0,
                        Tolerance = 0.01,
                        CacheSize = 500,
                        Strategy = SelectionStrategy.Sequential,
                    };
                }
            };
            var ksvm = teacher.Learn(inputs, outputs);
            logDebug("ksvm Done");
            double error = new ZeroOneLoss(outputs).Loss(ksvm.Decide(inputs));
            logDebug("error=" + error);

            int trainingHit = 0;
            int trainintMiss = 0;
            // For each image group (i.e. flowers, dolphins)
            Dictionary<string, Bitmap> data = train;
            foreach (String fileName in data.Keys)
            {
                double[] input = (bow as ITransform<Bitmap, double[]>).Transform(data[fileName]);
                String labelString = Path.GetFileName(Path.GetDirectoryName(fileName));
                int label = labelIndex[labelString];

                int actual = ksvm.Decide(input);
                if(label == actual)
                {
                    trainingHit++;
                }else
                {
                    trainintMiss++;
                    logDebug(labelString + " "  + String.Format("{0} {1}", label, actual));
                }
            }
            logDebug(String.Format("Result {0}/{1}", trainingHit, data.Count));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            String outputPath = dataPath + @"BoW\WeeklyQuest\";
            Rectangle rect = new Rectangle(0,200,100,220);
            DirectoryInfo folder = new DirectoryInfo(dataPath + "Work");
            FileInfo[] files = Utils.GetFilesByExtensions(folder,".png").ToArray();
            foreach(FileInfo f in files)
            {
                logDebug(f.FullName);
                Bitmap bmp = (Bitmap)Bitmap.FromFile(f.FullName);
                Bitmap sub = bmp.Clone(rect, bmp.PixelFormat);
                String fileName2 = outputPath +  Path.GetRandomFileName().Replace(".", "") + ".png";
                sub.Save(fileName2);
            }
        }

        private void cmbADBDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboDeviceItem c = (ComboDeviceItem)cmbADBDevice.SelectedItem;
            if (c != null)
            {
                Utils.ADBDevice = c.deviceName;
                Console.WriteLine("Device=" + Utils.ADBDevice);
            }
        }
    }

    public class ComboDeviceItem
    {
        public String deviceName;
        public String desc;
        public ComboDeviceItem(String deviceName,String desc)
        {
            this.deviceName = deviceName;
            this.desc = desc;
        }
        public override String ToString()
        {
            return desc;
        }
    }
}
