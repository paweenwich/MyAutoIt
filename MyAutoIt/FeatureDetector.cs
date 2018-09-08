using Accord.Imaging;
using Accord.Neuro;
using Accord.Neuro.Learning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAutoIt
{
    public class FeatureDetector
    {
        public MyLogger logger = new MyLogger();
        public String path;
        public Dictionary<String, SceneFeatureData> classifier = new Dictionary<string, SceneFeatureData>();
        // path = xxxxx/feature
        // feature = [xxx,yyyy]
        public void Init(String path, List<SceneFeature> features)
        {
            this.path = path;
            logger.logStr("FeatureDetector: Init path " + path);
            foreach (SceneFeature f in features)
            {
                classifier.Add(f.name, new SceneFeatureData() { feature = f,classifier = new BowImageClassifier() });
                logger.logStr("FeatureDetector: Init add " + f.name);
            }
        }
        public void LoadFeatureTrainData()
        {
            foreach(SceneFeatureData scenefeatureData in classifier.Values)
            {
                scenefeatureData.trainData = FeatureDetector.GetAllTrainImageData(path + @"\" + scenefeatureData.feature.name, scenefeatureData.feature.trainFolders);
            }
        }

        public bool Train()
        {
            foreach (SceneFeatureData scenefeatureData in classifier.Values)
            {
                if (TrainSceneFeature(scenefeatureData) == false)
                {
                    logger.logError("FeatureDetector: Train " + scenefeatureData.feature.name + " fail");
                    return false;
                }
            }
            return true;
        }
        public bool TrainSceneFeature(SceneFeatureData scenefeatureData)
        {
            //Create bow 
            var bow = BagOfVisualWords.Create(numberOfWords: scenefeatureData.feature.bowSize);
            var images = scenefeatureData.trainData.GetBitmaps();
            bow.Learn(images);
            Accord.IO.Serializer.Save(bow, path + @"\" + scenefeatureData.feature.name + String.Format(@"\train-{0}.bow", scenefeatureData.feature.bowSize));
            return Train(bow, scenefeatureData);
        }

        private bool Train(dynamic bow, SceneFeatureData scenefeatureData)
        {
            var trainData = scenefeatureData.trainData;
            int[] labelIndexs = trainData.GetLabelIndexs();
            String[] labels = trainData.GetLabels();
            double[][] features = trainData.GetFeature(bow);
            int numOutput = trainData.GetNumOutput();
            var function = new SigmoidFunction();
            bool flgFound = false;
            int count = 0;
            while ((flgFound == false) && (count < 100))
            {
                count++;
                var network = new ActivationNetwork(function, bow.NumberOfOutputs, 20, numOutput);
                new NguyenWidrow(network).Randomize();
                var teacher = new ParallelResilientBackpropagationLearning(network);

                BowImageClassifier trainImgClassifier = new BowImageClassifier();
                trainImgClassifier.Init(bow, network);
                //creat output
                double[][] outputs = trainData.GetOutputs(numOutput);
                double avgError = 10000.0;
                double prevError = avgError;
                double bestError = avgError;
                int errorCount = 0;
                while ((errorCount < 3) && (avgError > 0.00001))
                {
                    //Application.DoEvents();
                    double[] errors = new double[10];
                    for (int i = 0; i < 10; i++)
                    {
                        errors[i] = teacher.RunEpoch(features, outputs);
                    }
                    avgError = errors.Average();
                    if (prevError > avgError)
                    {
                        int trainError = trainImgClassifier.Evaluate(trainData);
                        //int testError = trainImgClassifier.Evaluate(testData);
                        //int testSetError = trainImgClassifier.Evaluate(testDataSet);
                        logger.logStr(String.Format("{0} {1} {2}", avgError, prevError, trainError));
                        prevError = avgError;
                        //save best error
                        if (bestError > avgError)
                        {
                            bestError = avgError;
                            //Accord.IO.Serializer.Save(network, dataPath + String.Format(@"\train-{0}.net", bow.NumberOfOutputs));
                        }
                        if (trainError /*+ testError + testSetError*/ == 0)
                        {
                            Accord.IO.Serializer.Save(network, path + @"\" + scenefeatureData.feature.name + String.Format(@"\train-{0}.net", bow.NumberOfOutputs));
                            return true;
                        }
                    }
                    else
                    {
                        logger.logStr(String.Format("{0}", avgError));
                        prevError = 10000.0;
                        errorCount++;
                    }
                    //Application.DoEvents();
                }
                logger.logStr("Done " + bestError + " " + count);
            }
            return false;
        }

        public static ImageTrainDataSet GetAllTrainImageData(String folder, String[] subFolders, String imageFilter = "*.png", String imageFilterOut = "mask.png")
        {
            ImageTrainDataSet ret = new ImageTrainDataSet();
            for (int i = 0; i < subFolders.Length; i++)
            {
                String folderName = folder + @"\" + subFolders[i];
                ImageFileData[] images = GetImagesFromDir(folderName, imageFilter, imageFilterOut);
                foreach (ImageFileData img in images)
                {
                    ret.Add(

                            new ImageTrainData()
                            {
                                label = subFolders[i],
                                labelIndex = i,
                                fileName = img.fileName,
                            }
                    );
                }
            }
            return ret;
        }
        public static ImageFileData[] GetImagesFromDir(String folder, String filter = "*.png", String filterOut = "mask.png")
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
                        continue;
                    }
                    //Bitmap bmp = (Bitmap)Bitmap.FromFile(file.FullName);
                    ret.Add(new ImageFileData() { fileName = file.FullName });
                }
            }
            return ret.ToArray();
        }
    }
}
