using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyAutoIt.Properties;
using System.IO;
using Newtonsoft.Json;
using System.Xml;

namespace MyAutoIt
{
    public partial class CandyCrush : Form
    {

        public static Candy emptyCandy = new Candy(Color.Black,CandyType.Empty);
        public Dictionary<Bitmap, Candy> htBmp = new Dictionary<Bitmap, Candy>();
        public BitmapSearch candySearch = new BitmapSearch();
        public BitmapSearch cellTypeSearch = new BitmapSearch();
        //public Dictionary<Utils.MyRawBitmapData, Candy> htBmpData = new Dictionary<Utils.MyRawBitmapData, Candy>();
        public Board board = new Board();
        public Dictionary<Bitmap, CellType> htJelly = new Dictionary<Bitmap, CellType>();
        public Bitmap screen;
        public Point refPoint = new Point(0,0);
        public Rectangle mainRect;
        public int cellW = 71;
        public int cellH = 63;
        public static int NUMCELL = 9;
        public static List<CandyPattern> lstPattern = new List<CandyPattern>();
        public static List<BoardPattern> lstBoardPattern = new List<BoardPattern>();
        public List<FindPatternResult> lstResult = new List<FindPatternResult>();
        public List<Board> lstHistory = new List<Board>();
        public FindPatternResult currentBestMove;
        public static bool flgDebug = false;
        public Pos2D moveFrom;
        public Pos2D moveTo;
        public CandyCrush()
        {
            htBmp.Add(new Bitmap("candycrush/R.png"),new Candy(Color.Red));
            htBmp.Add(new Bitmap("candycrush/R1.png"), new Candy(Color.Red,CandyType.StripH));
            htBmp.Add(new Bitmap("candycrush/R2.png"), new Candy(Color.Red,CandyType.StripV));
            htBmp.Add(new Bitmap("candycrush/G.png"), new Candy(Color.Green));
            htBmp.Add(new Bitmap("candycrush/G1.png"), new Candy( Color.Green, CandyType.StripH));
            htBmp.Add(new Bitmap("candycrush/G2.png"), new Candy(Color.Green, CandyType.StripV));
            htBmp.Add(new Bitmap("candycrush/G2_1.png"), new Candy(Color.Green, CandyType.StripV));
            htBmp.Add(new Bitmap("candycrush/G3.png"), new Candy(Color.Green, CandyType.Big));
            htBmp.Add(new Bitmap("candycrush/GF.png"), new Candy(Color.Green));
            htBmp.Add(new Bitmap("candycrush/B.png"), new Candy(Color.Blue));
            htBmp.Add(new Bitmap("candycrush/B1.png"), new Candy(Color.Blue, CandyType.StripH));
            htBmp.Add(new Bitmap("candycrush/B2.png"), new Candy(Color.Blue, CandyType.StripV));
            htBmp.Add(new Bitmap("candycrush/P.png"), new Candy(Color.Purple));
            htBmp.Add(new Bitmap("candycrush/P1.png"), new Candy( Color.Purple, CandyType.StripH));
            htBmp.Add(new Bitmap("candycrush/P2.png"), new Candy(Color.Purple, CandyType.StripV));
            htBmp.Add(new Bitmap("candycrush/P3.png"), new Candy(Color.Purple, CandyType.Big));
            htBmp.Add(new Bitmap("candycrush/Y.png"), new Candy(Color.Yellow));
            htBmp.Add(new Bitmap("candycrush/Y1.png"), new Candy(Color.Yellow, CandyType.StripH));
            htBmp.Add(new Bitmap("candycrush/Y2.png"), new Candy(Color.Yellow,CandyType.StripV));
            htBmp.Add(new Bitmap("candycrush/Y3.png"), new Candy(Color.Yellow, CandyType.Big));
            htBmp.Add(new Bitmap("candycrush/O.png"), new Candy( Color.Orange));
            htBmp.Add(new Bitmap("candycrush/O1.png"), new Candy( Color.Orange,CandyType.StripV));
            htBmp.Add(new Bitmap("candycrush/O2.png"), new Candy(Color.Orange, CandyType.StripV));
            htBmp.Add(new Bitmap("candycrush/O3.png"), new Candy(Color.Orange, CandyType.Big));
            htBmp.Add(new Bitmap("candycrush/OF.png"), new Candy(Color.Orange));
            htBmp.Add(new Bitmap("candycrush/FRUIT1.png"), new Candy(Color.Black,CandyType.Fruit));
            htBmp.Add(new Bitmap("candycrush/FRUIT2.png"), new Candy(Color.Black,CandyType.Fruit));
            htBmp.Add(new Bitmap("candycrush/FRUIT3.png"), new Candy(Color.Black));

            //foreach(Bitmap b in htBmp.Keys){
            //    htBmpData.Add(new Utils.MyRawBitmapData(b), htBmp[b]);
            //}

            candySearch.Add(htBmp.Keys);

            for (int i = 1; i <= 7; i++)
            {
                htJelly.Add(new Bitmap("candycrush/J" + i + ".png"),CellType.Jelly1);
            }
            htJelly.Add(new Bitmap("candycrush/JO2.png"),CellType.Jelly2);
            htJelly.Add(new Bitmap("candycrush/JP2.png"), CellType.Jelly2);
            htJelly.Add(new Bitmap("candycrush/JG2.png"), CellType.Jelly2);
            htJelly.Add(new Bitmap("candycrush/JB2.png"), CellType.Jelly2);

            cellTypeSearch.Add(htJelly.Keys);

/*            lstPattern.Add(new CandyPattern(new int[2, 3] { { 0, 1, 1 }, { 1, 0, 0 } },
                    new Point[] { new Point(0, 1), new Point(0, 0) }
                ));
*/


            lstPattern.Add(new CandyPattern(new int[3, 2] { { 0, 1 }, { 1, 0 }, { 1, 0 } },
                    new Point[] { new Point(1, 0), new Point(0, 0) }
                ));
            lstPattern.Add(new CandyPattern(new int[3, 2] { { 1, 0 }, { 1, 0 }, { 0, 1 } },
                    new Point[] { new Point(1, 2), new Point(0, 2) }
                ));
            lstPattern.Add(new CandyPattern(new int[3, 2] { { 0, 1 }, { 0, 1 }, { 1, 0 } },
                    new Point[] { new Point(0, 2), new Point(1, 2) }
                ));
            lstPattern.Add(new CandyPattern(new int[3, 2] { { 1, 0 }, { 0, 1 }, { 0, 1 } },
                    new Point[] { new Point(0, 0), new Point(1, 0) }
                ));

            lstPattern.Add(new CandyPattern(new int[1, 4] { { 1, 1, 0, 1 } },
                    new Point[]{new Point(3,0),new Point(2,0)}
                ));
            lstPattern.Add(new CandyPattern(new int[1, 4] { { 1, 0, 1, 1 } },
                    new Point[]{new Point(0,0),new Point(1,0)}
                ));

            lstPattern.Add(new CandyPattern(new int[4, 1] { { 1 }, { 1 }, { 0 }, { 1 } },
                    new Point[]{new Point(0,3),new Point(0,2)}
                ));
            lstPattern.Add(new CandyPattern(new int[4, 1] { { 1 }, { 0 }, { 1 }, { 1 } },
                    new Point[]{new Point(0,0),new Point(0,1)}
                ));

            lstPattern.Add(new CandyPattern(new int[2, 3] { { 0, 1, 0 }, { 1, 0, 1 } },
                    new Point[]{new Point(1,0),new Point(1,1)}
                ));
            lstPattern.Add(new CandyPattern(new int[2, 3] { { 1, 0, 1 }, { 0, 1, 0 } },
                    new Point[]{new Point(1,1),new Point(1,0)}
                ));

            lstPattern.Add(new CandyPattern(new int[3, 2] { { 1, 0 }, { 0, 1 }, { 1, 0 } },
                    new Point[]{new Point(1,1),new Point(0,1)}
                ));
            lstPattern.Add(new CandyPattern(new int[3, 2] { { 0, 1 }, { 1, 0 }, { 0, 1 } },
                    new Point[]{new Point(0,1),new Point(1,1)}
                ));

            lstPattern.Add(new CandyPattern(new int[2, 3] { { 0, 0, 1 }, { 1, 1, 0 } },
                    new Point[] { new Point(2, 0), new Point(2, 1) }
                ));
            lstPattern.Add(new CandyPattern(new int[2, 3] { { 1, 1, 0 }, { 0, 0, 1 } },
                    new Point[] { new Point(2, 1), new Point(2, 0) }
                ));
            lstPattern.Add(new CandyPattern(new int[2, 3] { { 1, 0, 0 }, { 0, 1, 1 } },
                    new Point[] { new Point(0, 0), new Point(0, 1) }
                ));

            lstPattern.Add(new CandyPattern(new int[2, 3] { { 0, 1, 1 }, { 1, 0, 0 } },
                    new Point[] { new Point(0, 1), new Point(0, 0) }
                ));

            lstPattern.Add(new SpecialPattern(new int[1, 2] { { 1, 1 }},
                    new Point[] { new Point(0, 0), new Point(1, 0) }
                ));
/*            lstPattern.Add(new SpecialPattern(new int[2, 1] { { 1}, {1} },
                    new Point[] { new Point(0, 0), new Point(0, 1) }
                ));
*/


            lstBoardPattern.Add(new BoardPattern(new int[1, 5] { { 1,1,1,1,1 } }, BoardPatternType.MultiColorH));
            lstBoardPattern.Add(new BoardPattern(new int[5, 1] { { 1}, {1}, {1}, {1}, {1 } }, BoardPatternType.MultiColorV));

            lstBoardPattern.Add(new BoardPattern(new int[3, 3] { { 1, 1, 1 }, { 1, 0, 0 }, { 1, 0, 0 } }, BoardPatternType.Big));
            lstBoardPattern.Add(new BoardPattern(new int[3, 3] { { 1, 1, 1 }, { 0, 1, 0 }, { 0, 1, 0 } }, BoardPatternType.Big));
            lstBoardPattern.Add(new BoardPattern(new int[3, 3] { { 1, 1, 1 }, { 0, 0, 1 }, { 0, 0, 1 } }, BoardPatternType.Big));
            lstBoardPattern.Add(new BoardPattern(new int[3, 3] { { 1, 0, 0 }, { 1, 1, 1 }, { 1, 0, 0 } }, BoardPatternType.Big));
            lstBoardPattern.Add(new BoardPattern(new int[3, 3] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } }, BoardPatternType.Big));
            lstBoardPattern.Add(new BoardPattern(new int[3, 3] { { 0, 0, 1 }, { 1, 1, 1 }, { 0, 0, 1 } }, BoardPatternType.Big));
            lstBoardPattern.Add(new BoardPattern(new int[3, 3] { { 1, 0, 0 }, { 1, 0, 0 }, { 1, 1, 1 } }, BoardPatternType.Big));
            lstBoardPattern.Add(new BoardPattern(new int[3, 3] { { 0, 1, 0 }, { 0, 1, 0 }, { 1, 1, 1 } }, BoardPatternType.Big));
            lstBoardPattern.Add(new BoardPattern(new int[3, 3] { { 0, 0, 1 }, { 0, 0, 1 }, { 1, 1, 1 } }, BoardPatternType.Big));




            lstBoardPattern.Add(new BoardPattern(new int[1, 4] { { 1, 1, 1, 1 } }, BoardPatternType.FourHorizontal));
            lstBoardPattern.Add(new BoardPattern(new int[4, 1] { { 1 }, { 1 }, { 1 }, { 1 } }, BoardPatternType.FourVirtical));
            lstBoardPattern.Add(new BoardPattern(new int[1, 3] { { 1, 1, 1 } }, BoardPatternType.ThreeHorizontal));
            lstBoardPattern.Add(new BoardPattern(new int[3, 1] { { 1 }, { 1 }, { 1 } }, BoardPatternType.ThreeVirtical));

            lstBoardPattern.Add(new SpecialBoardPattern(new int[1, 2] { { 1 , 1 }}, BoardPatternType.SpecialH));
            lstBoardPattern.Add(new SpecialBoardPattern(new int[2, 1] { { 1}, {1 } }, BoardPatternType.SpecialV));
            InitializeComponent();

        }

        public void dbgLine(String str)
        {
            txtDebug.AppendText(str + Environment.NewLine);
        }

        private void CandyCrush_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.location = this.Location;
            Settings.Default.flgSlide = chkSlide.Checked;
            Settings.Default.Save();
        }

        private void CandyCrush_Load(object sender, EventArgs e)
        {
            if (Settings.Default.location != null)
            {
                this.Location = Settings.Default.location;
            }
            chkSlide.Checked = Settings.Default.flgSlide;

        }

        public Bitmap saveGetScreenShot(Rectangle rect)
        {
            Bitmap bmp1 = Utils.GetScreenShot();
            while (true)
            {
                Bitmap cbmp1 = Utils.cropImage(bmp1, rect);
                System.Threading.Thread.Sleep(100);
                Bitmap bmp2 = Utils.GetScreenShot();
                Bitmap cbmp2 = Utils.cropImage(bmp2, rect);
                if (Utils.CompareMemCmp(cbmp1, cbmp2))
                {
                    return (bmp1);
                }
                bmp1 = bmp2;
                Console.WriteLine("saveGetScreenShot fail");
            }
        }

        public CellType getCellTYpe(Utils.MyRawBitmapData src, Utils.RECT rect)
        {
            BitmapSearch.BitmapSearchResult ret = cellTypeSearch.Search(src, rect, 2, 100);
            if (ret != null)
            {
                return (htJelly[ret.bitmap]);
            }
            return (CellType.Normal);
        }

        public CellType getCellTYpe(Bitmap src,Utils.RECT rect)
        {
            foreach (Bitmap b in htJelly.Keys)
            {
                Point pRet = Utils.findBitmap(src, rect, b, 2,100,100);
                if (pRet.X != 0)
                {
                    return (htJelly[b]);
                }
            }
            return (CellType.Normal);
        }
        public static bool isTableMatch(Candy[,] tab,Pattern p,int x,int y)
        {
            if ((x + p.GetLength(1) <= NUMCELL) && (y + p.GetLength(0) <= NUMCELL))
            {
               Candy r = null;
                for (int i = 0; i < p.GetLength(1); i++)
                {
                    for (int j = 0; j < p.GetLength(0); j++)
                    {
                        Candy c = tab[x + i, y + j];
                        if (p.isMatch(j, i, c, r))
                        {
                            if(c!=null){
                                if ((r == null)&&(!c.isEmpty()))
                                {
                                    if (p.pattern[j, i] == 1)
                                    {
                                        r = c;
                                    }
                                }
                            }
                        }
                        else
                        {
                            return (false);
                        }
                    }
                }
                return (true);
            }
            return (false);
        }
        private void imgMain_Paint(object sender, PaintEventArgs e)
        {
            float w = cellW;
            float h = cellH;
            Graphics g = e.Graphics;
            if (screen != null)
            {
                g.DrawImage(screen, 0, 0, mainRect, GraphicsUnit.Pixel);
            }
            Font myFont = new Font("Arial", 14);
            Pen myPen1 = new Pen(Color.Black, 2);
            Pen myPen2 = new Pen(Color.White, 2);
            Pen myPen3 = new Pen(Color.Red, 3);
            Candy[,] table = board.table;

            for (int i = 0; i < NUMCELL; i++)
            {
                for (int j = 0; j < NUMCELL; j++)
                {
                    if (board.isJelly(i, j))
                    {
                        g.DrawRectangle(Pens.Black, new Rectangle((int)w * i, (int)h * j, (int)w - 1, (int)h - 1));
                        if (board.cell[i, j] == CellType.Jelly2)
                        {
                            g.DrawRectangle(Pens.Black, new Rectangle((int)w * i + 5, (int)h * j + 5, (int)w - 10, (int)h - 10));
                        }
                    }
                    if(table[i, j] != null)
                    {
                        g.DrawString(table[i, j].getName(), myFont, new SolidBrush(table[i, j].color), new PointF(w * i, h * j));
                        if (table[i, j].movenum != 0)
                        {
                            g.DrawString("" + table[i, j].movenum, myFont, new SolidBrush(table[i, j].color), new PointF(w * i + w / 2, h * j + h / 2));
                        }

                        switch (table[i, j].type)
                        {
                            case CandyType.StripH:
                            {
                                g.DrawRectangle(new Pen(table[i, j].color,2), new Rectangle((int)w * i, (int)h * j, (int)w - 1, (int)h - 1));
                                break;
                            }
                            case CandyType.StripV:
                            {
                                g.DrawRectangle(new Pen(table[i, j].color,3), new Rectangle((int)w * i, (int)h * j, (int)w - 1, (int)h - 1));
                                break;
                            }
                            case CandyType.Big:
                            {
                                g.DrawRectangle(new Pen(table[i, j].color, 3), new Rectangle((int)w * i, (int)h * j, (int)w - 1, (int)h - 1));
                                break;
                            }

                        }
                    }
                    else
                    {
                        g.DrawLine(myPen1, w * i, h * j, w * (i + 1), h * (j + 1));
                        g.DrawLine(myPen1, w * i, h * (j + 1), w * (i + 1), h * j);
                    }
                }
            }
            foreach (FindPatternResult p in lstResult)
            {
                g.DrawLine(myPen1, w * p.to.X + (w / 2) -2, h * p.to.Y + (h / 2) + 2, w * p.from.X + w / 2 -2, h * p.from.Y + h / 2 +2);
                g.DrawLine(myPen2, w * p.to.X + w / 2, h * p.to.Y + h / 2, w * p.from.X + w / 2, h * p.from.Y + h / 2);
            }
            if (currentBestMove != null)
            {
                FindPatternResult p = currentBestMove;
                g.DrawLine(myPen1, w * p.to.X + (w / 2) - 2, h * p.to.Y + (h / 2) + 2, w * p.from.X + w / 2 - 2, h * p.from.Y + h / 2 + 2);
                g.DrawLine(myPen3, w * p.to.X + w / 2, h * p.to.Y + h / 2, w * p.from.X + w / 2, h * p.from.Y + h / 2);
            }
            if (moveFrom != null)
            {
                g.DrawRectangle(myPen3, new Rectangle((int)w * moveFrom.X, (int)h * moveFrom.Y, (int)w - 1, (int)h - 1));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String title = "Candy Crush";
            Candy[,] table = board.table;
            Color[] refColors = new Color[] {
                Color.FromArgb(0xEBDEE4),
//                Color.FromArgb(235,223,228),
            };

            if (Utils.activateWindow(title))
            {
                screen = Utils.GetScreenShot();
                Bitmap pngImage = new Bitmap("candycrush/ref.png");
                Utils.RECT rect = new Utils.RECT();
                IntPtr hwnd = Utils.GetForegroundWindow();
                Utils.GetWindowRect(hwnd, out rect);

                long start = System.Environment.TickCount;
                txtDebug.AppendText("Active " + rect + " " + hwnd + "\n");
                Utils.RECT scanRect = new Utils.RECT();
                scanRect.left = rect.left;
                scanRect.top = rect.top;
                scanRect.right = (rect.left + rect.right) / 4;
                scanRect.bottom = (rect.top + rect.bottom) / 3;
                refPoint = Utils.SmartFindBitmap(screen, scanRect, pngImage);
                if (refPoint.X <= 0)
                {
                    txtDebug.AppendText("Not found");
                    return;
                }
                refPoint.X += 106;
                refPoint.Y += 69;

                mainRect = new Rectangle(refPoint.X, refPoint.Y, NUMCELL * cellW, NUMCELL * cellH);
                screen = saveGetScreenShot(mainRect);
                screen.Save("candycrush.png");
                //Utils.MouseMove(refPoint.X, refPoint.Y);

                Bitmap crop = Utils.cropImage(screen, new Rectangle(refPoint.X, refPoint.Y, cellW * NUMCELL, cellH * NUMCELL));
                Utils.RECT CELLRECT = Utils.RECT.fromInt(0, 0, cellW, cellH);
                for (int i = 0; i < NUMCELL; i++)
                {
                    for (int j = 0; j < NUMCELL; j++)
                    {
                        board.table[i, j] = null;
                        Rectangle cellRect = new Rectangle((i * cellW), (j * cellH), cellW, cellH);

                        Bitmap cropCell = Utils.cropImage(crop, cellRect);
                        Utils.MyRawBitmapData cropCellBitmapData = new Utils.MyRawBitmapData(cropCell);
                        BitmapSearch.BitmapSearchResult searchRet = candySearch.Search(cropCellBitmapData, CELLRECT, 2, 1000);
                        if (searchRet != null)
                        {
                            Point pRet = searchRet.point;
                            board.table[i, j] = htBmp[searchRet.bitmap].clone();
                            CellType ct = getCellTYpe(cropCellBitmapData, CELLRECT);
                            board.cell[i, j] = ct;
                        }
                    }
                }
            }
            board.save("table.txt");
            board.load("table.txt");
            currentBestMove = board.getBestMove();
            lstResult = board.getPossibleMove();
            imgMain.Refresh();
            //txtDebug.AppendText(table.ToString());
        }

        public void move(FindPatternResult f)
        {
            txtDebug.AppendText("Move " + f.ToString() + Environment.NewLine);
            lstHistory.Add(board);
            //flgDebug = true;
            board = board.process(f);
            flgDebug = false;
            currentBestMove = board.getBestMove();
            lstResult = board.getPossibleMove();
            imgMain.Refresh();

        }


        private void button2_Click(object sender, EventArgs e)
        {
            board.load("table.txt");
            currentBestMove = board.getBestMove();
            lstResult = board.getPossibleMove();
            imgMain.Refresh();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
//            FindPatternResult f = board.getBestMove();
            FindPatternResult f = new FindPatternResult();
            f.to = new Point(7,6);
            f.from = new Point(7,7);
            if (f != null)
            {
                move(f);
            }
            else
            {
                txtDebug.AppendText("No more move");
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (lstHistory.Count > 0)
            {
                board = lstHistory[lstHistory.Count -1];
                lstHistory.Remove(board);
                currentBestMove = board.getBestMove();
                lstResult = board.getPossibleMove();
                imgMain.Refresh();
            }
            else
            {
                txtDebug.AppendText("No more move");
            }
        }

        private void imgMain_Click(object sender, EventArgs e)
        {

        }

        public bool isInFrom(List<FindPatternResult> lst,int x,int y)
        {
            foreach (FindPatternResult p in lst)
            {
                if ((p.from.X == x) && (p.from.Y == y))
                {
                    return (true);
                }
            }
            return (false);
        }

        public bool isInTo(List<FindPatternResult> lst, int x, int y)
        {
            foreach (FindPatternResult p in lst)
            {
                if ((p.to.X == x) && (p.to.Y == y))
                {
                    return (true);
                }
            }
            return (false);
        }

        private void imgMain_MouseDown(object sender, MouseEventArgs e)
        {
            int w = cellW;
            int h = cellH;
            int cellX = e.X / w;
            int cellY = e.Y / h;
            if (isInFrom(lstResult, cellX, cellY) || isInTo(lstResult, cellX, cellY))
            {
                if (moveFrom == null)
                {
                    if (isInFrom(lstResult,cellX, cellY))
                    {
                        moveFrom = new Pos2D(cellX, cellY);
                        moveTo = null;
                    }
                }
                else
                {
                    if ((moveFrom.X == cellX) && (moveFrom.Y == cellY))
                    {
                        moveFrom = null;
                    }
                    else
                    {
                        if (isInTo(lstResult, cellX, cellY))
                        {
                            moveTo = new Pos2D(cellX, cellY);
                            FindPatternResult f = new FindPatternResult();
                            f.to = moveTo.ToPoint();
                            f.from = moveFrom.ToPoint();
                            move(f);
                            moveFrom = null;
                            moveTo = null;
                        }

                    }
                }
            }
            else
            {
                moveFrom = null;
            }
            imgMain.Refresh();
        }

        private void chkSlide_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.flgSlide = chkSlide.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            board = new Board();
            for (int i = 0; i < 3; i++)
            {
                   board.table[i, 8] = new Candy(Color.Orange);
                   board.table[i, 7] = new Candy(Color.Orange);
            }
            board.table[0, 7] = emptyCandy.clone();
            board.table[1, 8] = emptyCandy.clone();
            board.table[2, 8] = emptyCandy.clone();
            //board.table[1, 6] = new Candy(Color.Orange, CandyType.StripV);
            //board.table[2, 6] = emptyCandy.clone();
            
            //currentBestMove = board.getBestMove();
            lstResult = board.getPossibleMove();
            imgMain.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
           screen = new Bitmap("candycrush.png");
           Bitmap pngImage = new Bitmap("candycrush/ref.png");
           Utils.RECT rect = Utils.RECT.fromInt(0, 0, screen.Width, screen.Height);
           long start = System.Environment.TickCount;

           Utils.RECT scanRect = new Utils.RECT();
           scanRect.left = rect.left;
           scanRect.top = rect.top;
           scanRect.right = (rect.left + rect.right) / 4;
           scanRect.bottom = (rect.top + rect.bottom) / 3;
           refPoint = Utils.SmartFindBitmap(screen, scanRect, pngImage);
           if (refPoint.X == 0)
           {
               txtDebug.AppendText("Not found");
               return;
           }
           refPoint.X += 106;
           refPoint.Y += 69;

           Bitmap crop = Utils.cropImage(screen, new Rectangle(refPoint.X, refPoint.Y, cellW * NUMCELL, cellH*NUMCELL));
           Utils.RECT CELLRECT = Utils.RECT.fromInt(0, 0, cellW, cellH);
           for (int i = 0; i < NUMCELL; i++)
           {
               for (int j = 0; j < NUMCELL; j++)
               {
                   board.table[i, j] = null;
                   Rectangle cellRect = new Rectangle((i * cellW),(j * cellH),cellW ,cellH );

                   Bitmap cropCell = Utils.cropImage(crop, cellRect);
                   Utils.MyRawBitmapData cropCellBitmapData = new Utils.MyRawBitmapData(cropCell);
                   BitmapSearch.BitmapSearchResult searchRet = candySearch.Search(cropCellBitmapData, CELLRECT, 2, 1000);
                   if (searchRet != null)
                   {
                       Point pRet = searchRet.point;
                       board.table[i, j] = htBmp[searchRet.bitmap].clone();
                       CellType ct = getCellTYpe(cropCellBitmapData, CELLRECT);
                       board.cell[i, j] = ct;
                   }
               }
           }
           long end = System.Environment.TickCount;
           dbgLine(refPoint + " " + (end - start));
           mainRect = new Rectangle(refPoint.X, refPoint.Y, NUMCELL * cellW, NUMCELL * cellH);
           currentBestMove = board.getBestMove();
           lstResult = board.getPossibleMove();
           imgMain.Refresh();

        }






    }
}
