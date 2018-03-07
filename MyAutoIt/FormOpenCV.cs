using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Features2D;
using Emgu.CV.Util;
using Emgu.CV.Flann;

namespace MyAutoIt
{
    public partial class FormOpenCV : Form
    {
        public FormOpenCV()
        {
            InitializeComponent();
        }

        public void log(String data)
        {
            Console.WriteLine(data);
        }

        Bitmap bmpModel;
        Bitmap bmp2;
        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch();

            Mat modelImage = CvInvoke.Imread(@"Linage2\Bow\SkillAuto.png", ImreadModes.Color);
            bmpModel = modelImage.Bitmap;
            Mat testImage = CvInvoke.Imread(@"Linage2\Main\PartyAuto\2e35av2fwbk.png", ImreadModes.Color);
            UMat uModelImage = modelImage.GetUMat(AccessType.Read);
            KAZE featureDetector = new KAZE();
            Mat modelDescriptors = new Mat();
            VectorOfKeyPoint modelKeyPoints = new VectorOfKeyPoint();
            VectorOfKeyPoint observedKeyPoints = new VectorOfKeyPoint();
            featureDetector.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors, false);
            log("model size = " + modelKeyPoints.Size);
            Mat observedDescriptors = new Mat();
            featureDetector.DetectAndCompute(testImage, null, observedKeyPoints, observedDescriptors, false);

            int k = 2;
            double uniquenessThreshold = 0.80;
            Mat mask;
            Mat homography = null;
            // Bruteforce, slower but more accurate
            // You can use KDTree for faster matching with slight loss in accuracy
            using (Emgu.CV.Flann.LinearIndexParams ip = new Emgu.CV.Flann.LinearIndexParams())
            using (Emgu.CV.Flann.SearchParams sp = new SearchParams())
            using (DescriptorMatcher matcher = new FlannBasedMatcher(ip, sp))
            {
                matcher.Add(modelDescriptors);

                matcher.KnnMatch(observedDescriptors, matches, k, null);
                mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                mask.SetTo(new MCvScalar(255));
                Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

                int nonZeroCount = CvInvoke.CountNonZero(mask);
                if (nonZeroCount >= 4)
                {
                    nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints,
                        matches, mask, 1.5, 20);
                    if (nonZeroCount >= 4)
                    {
                        homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, matches, mask, 2);
                    }
                    Mat result = new Mat();
                    Features2DToolbox.DrawKeypoints(modelImage, modelKeyPoints, result, new Bgr(Color.Red));
                    bmp2 = result.Bitmap;
                }
            }
            log("Done");
            
            Refresh();
        }

        private void FormOpenCV_Paint(object sender, PaintEventArgs e)
        {
            System.Drawing.Graphics g = this.CreateGraphics();
            if (bmpModel != null)
            {
                g.DrawImage(bmpModel, new Point(100, 100));
            }
            if (bmp2 != null)
            {
                g.DrawImage(bmp2, new Point(100,200));
            }
            Pen myPen = new Pen(System.Drawing.Color.Red, 5);
            g.DrawLine(myPen, new Point(0, 0), new Point(100, 100));
            //Refresh();
        }
    }
}
