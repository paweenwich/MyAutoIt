using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Threading;

namespace MyAutoIt
{
    public partial class Form1 : Form
    {
        public class Diamond
        {
            public Color color;
            public Color sample;
            public int count=0;
            public int group=0;
            public int spacial = 0;
            public bool grey = false;
            public Point pos;
            public long checkTime=0;
            public Diamond(int x,int y,Color c,Color s)
            {
                pos = new Point(x,y);
                sample = s;
                color = c;
                if(isWhite()){
                    if (isDiamond())
                    {
                        spacial = 1;
                    }
                    else
                    {
                        if (isGrey())
                        {
                            grey = true;
                        }
                    }
                }
            }
            public bool isEqual(Diamond d){
                return (color.ToArgb() == d.color.ToArgb());
            }
            public bool isWhite()
            {
                return isWhite(color);
            }
            public static bool isWhite(Color c){
                return ((c.R == 255) && (c.G == 255) && (c.B == 255));
            }
            public bool isDiamond(){
                Color c = Utils.getCloseColorIndex(sample, dimondColor, 100);
                return(!isWhite(c));                
            }
            public bool isGrey()
            {
                Color c = Utils.getCloseColorIndex(sample, greyColor, 500);
                return (!isWhite(c));                
            }

        }
        public class DiamondGroup
        {
            public List<Diamond> lstDimond = new List<Diamond>();
            public int left = 10000;
            public int top = 10000;
            public int right =0;
            public int buttom =0;
            public int group = 0;
            public void add(Diamond d)
            {
                group = d.group;
                if (d.pos.X < left)
                {
                    left = d.pos.X;
                }
                if (d.pos.Y < top)
                {
                    top = d.pos.Y;
                }
                if (d.pos.X > right)
                {
                    right = d.pos.X;
                }
                if (d.pos.Y > buttom)
                {
                    buttom = d.pos.Y;
                }
                lstDimond.Add(d);
            }
            public bool isOverLap(DiamondGroup g)
            {
                if (right >= g.left)
                {
                    if (left < g.left)
                    {
                        if (top <= g.top)
                        {
                            if (buttom > g.top)
                            {
                                return (true);
                            }
                        }
                    }
                }
                return (false);
            }

            override public String ToString()
            {
                return ("Group=" + group + " Count=" + lstDimond.Count + " [" + left + "," + top + "," + right + "," + buttom + "]");
            }

        }
        public const int PLAYDX = 538;
        public const int PLAYDY = 470;
        public const int BOARDDX = 75;
        public const int BOARDDY = 130;
        public static Point stateMargin = new Point(732-28, 348- 287);
        public static Point playButton = new Point();
        public static Point refPoint = new Point();
        public static Point statePoint = new Point();
        public static Point dimondSize = new Point(40, 40);
        public static Point boardPoint = new Point();
        public Diamond[,] board = new Diamond[10, 9];
        public List<DiamondGroup> prevList = new List<DiamondGroup>();
        int greyCount = 0;
        static System.Timers.Timer _timer; // From System.Timers
        public static Color[] validColors = new Color[] {
            Color.FromArgb(241, 48, 71),
            Color.FromArgb(186, 98, 250),
            Color.FromArgb(106, 211, 66),
            Color.FromArgb(241, 201, 35),
            Color.FromArgb(61, 124, 248),
            };
        public static Color[] dimondColor = new Color[] {
            Color.FromArgb(192, 176, 127),
            Color.FromArgb(179, 203, 159),
            Color.FromArgb(200, 214, 180),
            Color.FromArgb(211, 204, 132),
            
        };
        public static Color[] greyColor = new Color[] {
            Color.FromArgb(114, 114, 114),
            Color.FromArgb(132, 132, 132),
            Color.FromArgb(128, 128, 128),
        };
        //public static AutoItX3 autoit;
        public Form1()
        {
            InitializeComponent();
            //autoit = new AutoItX3();
            //autoit.AutoItSetOption("WinTitleMatchMode", 2);
            
        }
        public bool isInPrevList(Diamond data)
        {
            foreach (DiamondGroup d in prevList)
            {
                Diamond item = d.lstDimond.ElementAt(d.lstDimond.Count - 1);
                if ((data.pos.X == item.pos.X) && (data.pos.Y == item.pos.Y) )
                {
                    if (item.checkTime == 0)
                    {
                        data.checkTime = System.Environment.TickCount;
                        return (true);                    
                    }
                    else
                    {
                        if (System.Environment.TickCount - item.checkTime > 1000)
                        {
                            item.checkTime = System.Environment.TickCount;
                            data.checkTime = item.checkTime;
                            //txtDebug.AppendText("Time out" + item.pos + "\n");
                            return (false);
                        }
                        data.checkTime = item.checkTime;
                        return (true);
                    }
                    
                }
            }
            return (false);
        }
        public void doPlay(int speed=1)
        {
            adjustPoint();
            getBoard();
            evaluateBoard();
            List<DiamondGroup> m = createClickList();
            foreach (DiamondGroup d in m)
            {
                Diamond item = d.lstDimond.ElementAt(d.lstDimond.Count-1);
                Point pos = boardToPos(item.pos.X, item.pos.Y);
                if(chkSmart.Checked){
                    if (!isInPrevList(item))
                    {
                        Utils.MouseClick("LEFT", pos.X, pos.Y);
                    }
                    prevList = m;
                }
                else
                {
                    Utils.MouseClick("LEFT", pos.X, pos.Y);
                }
            }
            Thread.Sleep(100);
            imgDebug.Refresh();
        }

        public Point boardToPos(int x, int y)
        {
            return (new Point(boardPoint.X + 20 + (x * dimondSize.X), boardPoint.Y + 20 + (y * dimondSize.Y)));
        }

        public void getBoard()
        {
            Color[] cl = new Color[5];

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Point p = boardToPos(i, j);
                    cl[0] = Utils.GetPixelColor(p.X, p.Y);
                    cl[1] = Utils.GetPixelColor(p.X - 10, p.Y - 10);
                    cl[2] = Utils.GetPixelColor(p.X - 10, p.Y + 10);
                    cl[3] = Utils.GetPixelColor(p.X + 10, p.Y - 10);
                    cl[4] = Utils.GetPixelColor(p.X + 10, p.Y + 10);

                    Color sample = Utils.avgColor(cl);
                    //Utils.MouseMove(p.X, p.Y);
                    Color c = Utils.getCloseColorIndex(sample, validColors,5000);
                    //c = sample;

                    board[i, j] = new Diamond(i,j,c, sample);
                    if (board[i, j].isWhite())
                    {
                        //txtDebug.AppendText("W " + i + " " + j + " " + sample.ToString() + " " + sample.ToArgb() + " " +  board[i, j].spacial + "\n");
                    }
                    else
                    {
                        //txtDebug.AppendText("" + i + " " + j + " " + c.ToString() + " " + c.ToArgb() + " " + board[i, j].spacial + "\n");
                    }
                }
            }

        }

        public void clearGroupCount(int group)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board[i, j].group == group)
                    {
                        board[i, j].count = 0;
                    }
                }
            }
        }
        public void countGroup(int maxGroup)
        {
            int[] count = new int[maxGroup];
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (board[i, j].group !=0)
                        {
                            count[board[i, j].group]++;
                        }
                    }
                }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board[i, j].group != 0)
                    {
                        board[i, j].count = count[board[i, j].group];
                    }
                }
            }
        }
        //
        //  Set group and count
        //
        public void evaluateBoard()
        {
            int group = 1;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board[i, j].isWhite())
                    {
                        continue;
                    }
                    bool topFound = false;
                    if (j >0)
                    {
                        if (board[i, j].isEqual(board[i, j-1]))
                        {
                            //txtDebug.AppendText("FoundTop" + i + "," + j + "\n");
                            if(board[i, j - 1].group==0){
                                board[i, j - 1].group = group++;
                            }
                            board[i, j].group = board[i, j - 1].group;
                            topFound = true;
                        }
                    }
                    if (i > 0)
                    {
                        if (board[i, j].isEqual(board[i-1, j]))
                        {
                            //txtDebug.AppendText("FoundLeft" + i + "," + j + "\n");
                            if (board[i-1, j].group == 0)
                            {
                                board[i-1, j].group = group++;
                            }
                            board[i, j].group = board[i-1, j].group;
                            if (topFound)
                            {
                                // popagate top
                                for (int c = j; c >= 0; c--)
                                {
                                    if (board[i, c].isEqual(board[i - 1, j]))
                                    {
                                        board[i, c].group = board[i - 1, j].group;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            continue;
                        }
                    }
                }
            }
            countGroup(group);
        }
        public DiamondGroup createGroup(int group)
        {
            DiamondGroup ret = new DiamondGroup();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board[i, j].group == group)
                    {
                        ret.add(board[i, j]);
                    }
                }
            }
            return (ret);
        }
        public List<DiamondGroup> createClickList()
        {
            List<DiamondGroup> ret = new List<DiamondGroup>();
            List<int> cache = new List<int>();
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (board[i, j].grey)
                    {
                        greyCount++;
                    }
                    if (board[i, j].count >= 3)
                    {
                        // check in cache
                        if (!cache.Exists(g => g == board[i, j].group))
                        {
                            ret.Add(createGroup(board[i, j].group));
                            cache.Add(board[i, j].group);
                            continue;
                        }
                    }
                }
            }
            if (ret.Count < 3)
            {
                for (int j = 0; j < 9; j++)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (board[i, j].spacial != 0)
                        {
                            DiamondGroup d = new DiamondGroup();
                            d.add(board[i, j]);
                            ret.Add(d);
                        }
                    }
                }
            }

//            txtDebug.AppendText("createClickList " + ret.Count + "\n");
//            foreach (DiamondGroup d in ret)
//            {
//                txtDebug.AppendText(d.ToString() + "\n");
//            }
            ret = optimizeClickList(ret);
            //txtDebug.AppendText("optimizeClickList " + ret.Count + " grey=" + greyCount + "\n");
//            foreach (DiamondGroup d in ret)
//            {
//                txtDebug.AppendText(d.ToString() + "\n");
//            }

            return(ret);
        }
        public List<DiamondGroup>  optimizeClickList(List<DiamondGroup> lst)
        {
            List<DiamondGroup> ret = new List<DiamondGroup>();
            for (int i = 0; i < lst.Count; i++)
            {
                bool flgRemove = false;
                DiamondGroup item1 = lst.ElementAt(i);
                for (int j = 1 + 1; j < lst.Count; j++)
                {
                    DiamondGroup item2 = lst.ElementAt(j);
                    if (item1.isOverLap(item2))
                    {
                        // make sure that over lap valid

//                        txtDebug.AppendText(item1.ToString() + " overlap " + item2.ToString() + "\n");
                        //Remove less count item
                        if (item1.lstDimond.Count >= item2.lstDimond.Count)
                        {

//                            txtDebug.AppendText("Remove2 " + item2.ToString()+ "\n");
                            lst.RemoveAt(j);
                        }
                        else
                        {
//                            txtDebug.AppendText("Remove1 " + item1.ToString() + "\n");
                            flgRemove = true;
                            break;
                        }
                    }
                }
                if (!flgRemove)
                {
                    ret.Add(item1);
                    // click special item one at a time
                    if (item1.lstDimond.Count == 1)
                    {
                        break;
                    }
                }
            }
            return (ret);
        }

        public void adjustPoint()
        {
            playButton.X = refPoint.X + PLAYDX;
            playButton.Y = refPoint.Y + PLAYDY;
            boardPoint.X = refPoint.X + BOARDDX;
            boardPoint.Y = refPoint.Y + BOARDDY;
            statePoint.X = refPoint.X + stateMargin.X;
            statePoint.Y = refPoint.Y + stateMargin.Y;
        }
        private void button1_Click(object sender, EventArgs e) 
        {
            String title = "Diamond Dash";
            Color[] refColors = new Color[] {
                Color.FromArgb(0xEBDEE4),
//                Color.FromArgb(235,223,228),
            };
            
            if (Utils.activateWindow(title))
            {
                Utils.RECT rect = new Utils.RECT();
                IntPtr hwnd = Utils.GetForegroundWindow();
                Utils.GetWindowRect(hwnd,out rect);

                //int x = rect.left;
                //int y = rect.top;
                //int w = rect.right;
                //int h = rect.bottom;

                txtDebug.AppendText("Active " + rect + " " + hwnd + "\n");
                Utils.RECT scanRect = new Utils.RECT();
                scanRect.left = rect.left;
                scanRect.top = rect.top;
                scanRect.right = (rect.left + rect.right) / 3;
                scanRect.bottom = (rect.top + rect.bottom)/2;
                //Int32[] pt = new Int32[2];
                for (int i = 0; i < refColors.Length; i++)
                {
                    long start = System.Environment.TickCount;
                    Point p = Utils.PixelSearch(scanRect, refColors[i],1);

                    if ((p.X + p.Y) != 0)
                    {
                        Utils.MouseMove(p.X, p.Y);
                        txtDebug.AppendText("Pos " + p + " in " + (System.Environment.TickCount-start) + "\n");
                        refPoint = p;
                        adjustPoint();
                        chkStart.Enabled = true;
                        break;
                    }
/*                    try
                    {

                        Array.Copy((System.Object[])(autoit.PixelSearch(x, y, w, h, refColors[i], 0)), pt, 2);
                        txtDebug.AppendText("Pos " + pt[0] + " " + pt[1] + "\n");
                        refPoint.X = pt[0];
                        refPoint.Y = pt[1];
                        adjustPoint();
                        chkStart.Enabled = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        //return;
                    }
*/
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            long start = System.Environment.TickCount;
//            Utils.SearchWindow("Google",true);
//            Utils.FindWindowEx(0, IntPtr.Zero, "ComboBoxEx32", IntPtr.Zero);
//                childHandle = FindWindowEx(
            Bitmap bmp = Utils.GetScreenShot();
            for (int i = 0; i < 160000; i++)
            {
            //    bmp.GetPixel(0, 0);
                Utils.GetPixelColor(0, 0);
            }
            txtDebug.AppendText("" + (System.Environment.TickCount - start) + "\n");
//            Utils.MouseClick(0, 0);
//            Utils.mouse_event((int)(Utils.MouseEventFlags.MOVE | Utils.MouseEventFlags.ABSOLUTE), 0, 0, 0, UIntPtr.Zero);
//            Utils.mouse_event((int)(Utils.MouseEventFlags.LEFTDOWN | Utils.MouseEventFlags.LEFTUP | Utils.MouseEventFlags.ABSOLUTE), 0, 0, 0, UIntPtr.Zero);
//            activateWindow("Adobe");
//            refPoint.X = 129; // 170;
//            refPoint.Y = 243; // 418;

//            refPoint.X = 74;
//            refPoint.Y = 407;

//            doPlay();
        }

        private void imgDebug_Paint(object sender, PaintEventArgs e)
        {
            if (board[0, 0] == null)
            {
                return;
            }
            int z = 15;
            Graphics c = e.Graphics;
            Font drawFont = new Font("Arial", 8);
            Brush txtBrush = new System.Drawing.SolidBrush(Color.Black);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Pen myPen = new Pen(board[i,j].color, 1);
                    Brush myBrush = new System.Drawing.SolidBrush(board[i,j].color);
                    c.FillRectangle(myBrush, 0 + (i * z), 0 + (j * z), z, z);
                    if (board[i, j].group != 0)
                    {
                        if (board[i, j].count >= 3)
                        {
                            c.DrawString("" + board[i, j].group, drawFont, txtBrush, 0 + (i * z), 0 + (j * z));
                        }
                    }
                    else
                    {
                        if (board[i, j].spacial != 0)
                        {
                            c.DrawString("x", drawFont, txtBrush, 0 + (i * z), 0 + (j * z));
                        }
                    }
                    //c.DrawRectangle(myPen, 0 + (i*z), 0 + (j*z), z, z);
                }
            }
        }

        private void chkStart_CheckedChanged(object sender, EventArgs e)
        {
            if (chkStart.Checked)
            {
                greyCount = 0;
                txtDebug.AppendText("" + timer1.Interval+"\n");
                _timer = new System.Timers.Timer(trackBar1.Value);
                _timer.SynchronizingObject = this;
                _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
                _timer.Start();

//                timer1.Interval = trackBar1.Value;
//                timer1.Enabled = true;
            }
            else
            {
                _timer.Stop();
                _timer = null;
                txtDebug.AppendText("GreyCount=" + greyCount);
//                timer1.Enabled = false;
            }
        }
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer1_Tick(sender, null);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            int[] refColors = new int[] {
                0x68CAFF,
            };
//            Color c1 = Color.FromArgb(cl);
            Color c = Utils.GetPixelColor(statePoint.X, statePoint.Y);
//            txtDebug.AppendText("" + c + " " + c1 + "\n");
//            txtDebug.AppendText("" + c +  "\n");
            for (int i = 0; i < refColors.Length; i++)
            {
                Color cref = Color.FromArgb(refColors[i]);
                if (Utils.diffColor(c, cref) > 1000)
                {
                    //    this.Text = "Score";
                }
                else
                {
                    //    this.Text = "Play";
                    //txtDebug.AppendText("Play");
                    doPlay();
                    break;
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("123");
            String[] lines = File.ReadAllLines("C:\\usps.txt");
            foreach (String s in lines)
            {
                txtDebug.AppendText(s + "," + OneCode.OneCodeDecode(s) + "\n");
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = trackBar1.Value;
//            txtDebug.AppendText("" + timer1.Interval+"\n");
        }
    }
}
