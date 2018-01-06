using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MyAutoIt
{
    public class BitmapSearch
    {
        public class BitmapSearchResult
        {
            public Bitmap bitmap;
            public Point point;
            public double distant=0;
            public BitmapSearchResult(Bitmap b,Point p)
            {
                bitmap = b;
                point = p;
            }
        }
        public class KeyColor
        {
            public Utils.MyColor color;
            public Point pos;
            public KeyColor(Utils.MyColor c, Point p)
            {
                color = c;
                pos = p;
            }
        }
        public class KeyColorMyRawBitmapData : Utils.MyRawBitmapData
        {
            public KeyColor keyColor;
            public KeyColorMyRawBitmapData(Bitmap b): base(b)
            {
                List<KeyColor> k = getKeyColor(this, 1);
                if (k.Count > 0)
                {
                    keyColor = k[0];
                }
                else
                {
                    keyColor = null;
                }
            }
        }
        public Dictionary<KeyColorMyRawBitmapData, Bitmap> lstBitmap = new Dictionary<KeyColorMyRawBitmapData, Bitmap>();

        public void Add(ICollection<Bitmap> t)
        {
            foreach (Bitmap b in t)
            {
                lstBitmap.Add(new KeyColorMyRawBitmapData(b), b);
            }
        }

        public class PostValue
        {
            public Point pos;
            public int value;
            public PostValue(int  v, Point p)
            {
                value = v;
                pos = p;
            }
            public void inc()
            {
                value++;
            }
        }
        public static List<KeyColor> getKeyColor(Utils.MyRawBitmapData bitmapData, int num)
        {


            List<KeyColor> ret = new List<KeyColor>();
            Dictionary<Utils.MyColor, PostValue> htColor = new Dictionary<Utils.MyColor, PostValue>(new Utils.MyColor.EqualityComparer());
            for (int y = 0; y < bitmapData.height; y++)
            {
                for (int x = 0; x < bitmapData.width; x++)
                {
                    Utils.MyColor m = bitmapData.GetColor(x, y);
                    if (htColor.ContainsKey(m))
                    {
                        htColor[m].inc();
                    }
                    else
                    {
                        htColor[m] = new PostValue(1, new Point(x, y));
                    }

                }
            }
            foreach (Utils.MyColor m in htColor.Keys)
            {
                if (htColor[m].value == 1)
                {
                    ret.Add(new KeyColor(m,htColor[m].pos));
                    //Console.WriteLine("getKeyColor " + m + " " + htColor[m].pos);
                    if (ret.Count >= num)
                    {
                        break;
                    }
                }
            }
            return (ret);
        }

        public BitmapSearchResult Search(Utils.MyRawBitmapData data, Utils.RECT rect, int step = 1, double colorError = 1, bool flgSingle = true)
        {
            foreach (KeyColorMyRawBitmapData b in lstBitmap.Keys)
            {
                List<Point> listP = GetSubPositions(data, b, rect, step, colorError, flgSingle);
                if (listP.Count > 0)
                {
                    return (new BitmapSearchResult(lstBitmap[b],listP[0]));
                }
            }
            return (null);
        }



        public static List<Point> GetSubPositions(Utils.MyRawBitmapData main, KeyColorMyRawBitmapData sub, Utils.RECT rect, int step = 1, double colorError = 1, bool flgSingle = true)
        {
            List<Point> possiblepos = new List<Point>();

            int subwidth = sub.width;
            int subheight = sub.height;

            int movewidth = rect.right - subwidth;
            int moveheight = rect.bottom - subheight;

            int maxX = rect.left + movewidth - subwidth;
            int maxY = rect.top + moveheight - subheight;
            int cX = subwidth / 2;
            int cY = subheight / 2;
            if (sub.keyColor != null)
            {
                cX = sub.keyColor.pos.X;
                cY = sub.keyColor.pos.Y;
            }


            Utils.MyColor firstColor = sub.GetColor(0, 0);
            Utils.MyColor CenterColor = sub.GetColor(cX, cY);

            for (int y = rect.top; y < moveheight; ++y)
            {
                for (int x = rect.left; x < movewidth; ++x)
                {
                    Utils.MyColor curcolor = main.GetColor(x, y);
                    if (possiblepos.Count > 0)
                    {
                        foreach (var item in possiblepos.ToArray())
                        {
                            int xsub = x - item.X;
                            int ysub = y - item.Y;
                            if (xsub >= subwidth || ysub >= subheight || xsub < 0)
                            {
                                continue;
                            }

                            Utils.MyColor subcolor = sub.GetColor(xsub, ysub);
                            if (!curcolor.nearColor(subcolor, colorError))
                            {
                                possiblepos.Remove(item);
                            }
                        }
                    }
                    // add part 
                    // we should not add pixel that will out of bound 
                    if (x > maxX)
                    {
                        continue;
                    }
                    if (y > maxY)
                    {
                        continue;
                    }

                    if (curcolor.nearColor(firstColor, colorError))
                    {
                        if (CenterColor.nearColor(main.GetColor(x + cX, y + cY),colorError))
                        {
                            possiblepos.Add(new Point(x, y));
                        }
                    }
                }
                if (flgSingle && (possiblepos.Count > 0))
                {
                    if (y - subheight > possiblepos[0].Y)
                    {
                        //Console.WriteLine("found break");
                        break;
                    }
                }
                //Console.WriteLine("Y=" + y + " Count=" + possiblepos.Count);
            }
            return possiblepos;
        }

    }
}
