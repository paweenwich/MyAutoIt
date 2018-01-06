using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace MyAutoIt
{
    public partial class CandyCrush : Form
    {
        public enum CandyType
        {
            Normal,
            Empty,
            StripH,
            StripV,
            Big,
            MultiColor,
            Fruit,
        }
        public enum CellType: int
        {
            Normal=0x00,
            Jelly1=0x01,
            Jelly2=0x02,
        }
        public class Pos2D
        {
            public int X;
            public int Y;
            public Pos2D(int x, int y)
            {
                X = x;
                Y = y;
            }
            public override String ToString(){
                return("{" + X + "," + Y  + "}");
            }
            public Point ToPoint()
            {
                return (new Point(X, Y));
            }
        }
        public class Candy
        {
            //public String name;
            public Color color;
            public CandyType type;
            //public bool flgJelly = false;
            public int movenum = 0;
            public Candy(Color c, CandyType t = CandyType.Normal)
            {
                color = c;
                type = t;
            }
            public Candy clone()
            {
                Candy ret = new Candy(color, type);
                //ret.flgJelly = flgJelly;
                return (ret);
            }
            public String getName()
            {
                if (type == CandyType.Empty)
                {
                    return ("Nil");
                }
                if (type == CandyType.Fruit)
                {
                    return ("Frt");
                }
                if (type == CandyType.Big)
                {
                    return (color.Name.Substring(0, 1) + "B");
                }
                if (type == CandyType.StripH)
                {
                    return (color.Name.Substring(0, 1) + "H");
                }
                if (type == CandyType.StripV)
                {
                    return (color.Name.Substring(0, 1) + "V");
                }
                return (color.Name.Substring(0, 1));
            }
            public override String ToString()
            {

                return (getName() + " " + color.Name + " " + type);
            }
            public static Candy FromString(String data)
            {
                String[] item = data.Split(' ');
                Candy ret = new Candy(Color.FromName(item[1]), (CandyType)Enum.Parse(typeof(CandyType), item[2]));
                
                //ret.flgJelly = Boolean.Parse(item[2]);
                return (ret);
            }
            public bool isEmpty()
            {
                //return (name.Equals(emptyCandy.name));
                return (type == CandyType.Empty);
            }
            //public bool isJelly()
            //{
            //    return (flgJelly);
            //}
        }
        public class Pattern
        {
            public int[,] pattern;
            public virtual bool isMatch(int x, int y, Candy c, Candy r)
            {
                if (pattern[x, y] == 1)
                {
                    if (c == null)
                    {
                        return (false);
                    }
                    if (r == null)
                    {
                        if (c.isEmpty())
                        {
                            return (false);
                        }
                        else
                        {
                            return (true);
                        }
                    }
                    else
                    {
                        if (c.isEmpty())
                        {
                            return (false);
                        }
                        if (c.color.ToArgb() == r.color.ToArgb())
                        {
                            return (true);
                        }
                        else
                        {
                            return (false);
                        }
                    }

                }
                else
                {
                    return (true);
                }
                //return (pattern[x, y] == 1);
            }
            public int GetLength(int index)
            {
                return (pattern.GetLength(index));
            }
        }
        public class CandyPattern : Pattern
        {
            public Point[] target;
            public CandyPattern(int[,] p, Point[] t)
            {
                pattern = p;
                target = t;
            }
        }
        public class SpecialPattern : CandyPattern
        {
            public SpecialPattern(int[,] p, Point[] t): base(p,t)
            {
                
            }

            public override bool isMatch(int x, int y, Candy c, Candy r)
            {
                if (c != null)
                {
                    if ((c.type == CandyType.StripH) || (c.type == CandyType.StripV) || ((c.type == CandyType.Big)))
                    {
                        return (base.isMatch(x, y, c, r));
                    }
                }
                return(false);
            }
        }

        public class Score
        {
            public int jelly = 1000000;
            public int empty = 0;
            public int candy = 0;
            public int possibleMove = 0;
            public int fruit = 1000000;
            public override String ToString()
            {
                return ("j=" + jelly + " e=" + empty + " c=" + candy + " p=" + possibleMove + " f=" + fruit);
            }
            public int getValue()
            {
                return (1000000 - jelly + empty + candy + possibleMove - fruit);
            }
            public void clear()
            {
                jelly = 0;
                fruit = 0;

            }
        }
        public class FindPatternResult
        {
            public Point from;
            public Point to;
            public Score score;
            public override String ToString()
            {
                return ("[" + from.X + "," + from.Y + "]->[" + +to.X + "," + to.Y + "]");
            }
        }
        public enum BoardPatternType
        {
            ThreeHorizontal,
            ThreeVirtical,
            FourHorizontal,
            FourVirtical,
            Big,
            MultiColorV,
            MultiColorH,
            SpecialV,
            SpecialH,

        }
        public class BoardPattern : Pattern
        {
            public BoardPatternType type = BoardPatternType.ThreeHorizontal;
            public BoardPattern(int[,] p, BoardPatternType t)
            {
                pattern = p;
                type = t;
            }

        }
        public class SpecialBoardPattern : BoardPattern
        {
            public SpecialBoardPattern(int[,] p, BoardPatternType t)
                : base(p, t)
            {
            }
            public override bool isMatch(int x, int y, Candy c, Candy r)
            {
                if (c != null)
                {
                    if ((c.type == CandyType.StripH) || (c.type == CandyType.StripV) || ((c.type == CandyType.Big)))
                    {
                        return (base.isMatch(x, y, c, r));
                    }
                }
                return (false);
            }
        }

        public class Board
        {
            public Candy[,] table;
            public CellType[,] cell = new CellType[NUMCELL, NUMCELL];
            public Board()
            {
                init();
            }
            public void init()
            {
                table = new Candy[NUMCELL, NUMCELL];
                for (int i = 0; i < NUMCELL; i++)
                {
                    for (int j = 0; j < NUMCELL; j++)
                    {
                        cell[i, j] = CellType.Normal;
                    }
                }
            }

            public Board clone()
            {
                Board ret = new Board();
                for (int i = 0; i < NUMCELL; i++)
                {
                    for (int j = 0; j < NUMCELL; j++)
                    {
                        if (table[i, j] != null)
                        {
                            ret.table[i, j] = table[i, j].clone();
                        }
                        ret.cell[i, j] = cell[i, j];
                    }
                }
                return (ret);
            }
            public void save(String filename)
            {
                List<String> lines = new List<String>();
                for (int i = 0; i < NUMCELL; i++)
                {
                    for (int j = 0; j < NUMCELL; j++)
                    {
                        if (table[i, j] != null)
                        {
                            lines.Add("" + i + "," + j + "," + table[i, j].ToString() + "," + cell[i,j].ToString());
                        }
                    }
                }
                File.WriteAllLines(filename, lines);
                Console.WriteLine("Save " + filename + " " + lines.Count);
            }

            public void load(String filename)
            {
                init();
                String[] lines = File.ReadAllLines(filename);
                for (int i = 0; i < lines.Count(); i++)
                {
                    String[] token = lines[i].Split(',');
                    int x = Int32.Parse(token[0]);
                    int y = Int32.Parse(token[1]);
                    Candy c = Candy.FromString(token[2]);
                    CellType ct = (CellType)Enum.Parse(typeof(CellType), token[3]);
                    table[x, y] = c;
                    cell[x, y] = ct;
                }
                Console.WriteLine("load " + filename + " " + lines.Count());
            }

            public void move(Point from, Point to)
            {
                Candy cFrom = table[from.X, from.Y];
                Candy cTo = table[to.X, to.Y];
                table[from.X, from.Y] = cTo;
                //table[from.X, from.Y].flgJelly = cFrom.flgJelly;
                table[to.X, to.Y] = cFrom;
                //table[to.X, to.Y].flgJelly = cTo.flgJelly;
                table[from.X, from.Y].movenum = 1;
                table[to.X, to.Y].movenum = 2;
            }

            public void effect(Candy c, int x, int y)
            {
                if (c == null)
                {
                    return;
                }
                switch (c.type)
                {
                    case CandyType.StripH:
                        {
                            for (int i = 0; i < NUMCELL; i++)
                            {
                                if (x != i)
                                {
                                    if (table[i, y] != null)
                                    {
                                        if (!table[i, y].isEmpty())
                                        {
                                            blow(i, y);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case CandyType.StripV:
                        {
                            for (int i = 0; i < NUMCELL; i++)
                            {
                                if (y != i)
                                {
                                    if (table[x, i] != null)
                                    {
                                        if (!table[x, i].isEmpty())
                                        {
                                            blow(x, i);
                                        }
                                    }
                                }
                            }
                            break;
                        }

                }
            }
            public void blow(int x, int y)
            {
                Candy c = table[x, y];
                table[x, y] = emptyCandy.clone();
                switch(cell[x, y]){
                    case CellType.Jelly2: cell[x, y]=CellType.Jelly1;break;
                    case CellType.Jelly1: cell[x, y]=CellType.Normal;break;
                }
                effect(c, x, y);

            }

            public Board process(FindPatternResult f)
            {
                Board ret = clone();
                Candy cFrom = table[f.from.X, f.from.Y];
                Candy cTo = table[f.to.X, f.to.Y];

                ret.move(f.from, f.to);
                bool flgDo = true;
                int count = 0;
                while (flgDo)
                {
                    flgDo = false;
                    for (int y = 0; y < NUMCELL; y++)
                    {
                        for (int x = 0; x < NUMCELL; x++)
                        {
                            foreach (BoardPattern b in lstBoardPattern)
                            {
                                if (isTableMatch(ret.table, b, x, y))
                                {
                                    if (flgDebug)
                                    {
                                        Console.WriteLine("Match " + x + "," + y + "," + b.type);
                                    }
                                    switch (b.type)
                                    {
                                        case BoardPatternType.ThreeHorizontal:
                                            {
                                                ret.blow(x, y);
                                                ret.blow(x + 1, y);
                                                ret.blow(x + 2, y);
                                                flgDo = true;
                                                break;
                                            }
                                        case BoardPatternType.ThreeVirtical:
                                            {
                                                ret.blow(x, y);
                                                ret.blow(x, y + 1);
                                                ret.blow(x, y + 2);
                                                flgDo = true;
                                                break;
                                            }
                                        case BoardPatternType.FourHorizontal:
                                            {
                                                Candy c = ret.table[x + 2, y];
                                                Point target = new Point(x + 2, y);
                                                if (cFrom != null)
                                                {
                                                    c = cFrom;
                                                    target = f.to;
                                                }
                                                for (int i = 0; i < 4; i++)
                                                {
                                                    ret.blow(x + i, y);
                                                }
                                                ret.table[target.X, target.Y] = new Candy(c.color, CandyType.StripH);

                                                flgDo = true;
                                                break;
                                            }
                                        case BoardPatternType.FourVirtical:
                                            {
                                                int maxMove = 0;
                                                Point maxPoint = new Point();
                                                Candy c = null;
                                                for (int i = 0; i < 4; i++)
                                                {
                                                    if (ret.table[x, y + i].movenum > 0)
                                                    {
                                                        maxMove = ret.table[x, y + i].movenum;
                                                        maxPoint = new Point(x, y + i);
                                                        c = ret.table[x, y + i].clone();
                                                    }
                                                    ret.blow(x, y + i);
                                                }
                                                if (c != null)
                                                {
                                                    ret.table[maxPoint.X, maxPoint.Y] = new Candy(c.color, CandyType.StripH);
                                                }

                                                flgDo = true;
                                                break;
                                            }
                                        case BoardPatternType.Big:
                                            {
                                                //ret.dump("Big before");
                                                for (int i = 0; i < 3; i++)
                                                {
                                                    for (int j = 0; j < 3; j++)
                                                    {
                                                        if (b.isMatch(j, i, ret.table[x + i, y + j],null))
                                                        {
                                                            ret.blow(x + i, y + j);
                                                        }
                                                    }
                                                }
                                                //ret.dump("Big after");
                                                flgDo = true;
                                                break;
                                            }
                                        case BoardPatternType.MultiColorH:
                                            {
                                                for(int i=0;i<5;i++){
                                                    ret.blow(x+i, y);

                                                }
                                                flgDo = true;
                                                break;
                                            }
                                        case BoardPatternType.MultiColorV:
                                            {
                                                for (int i = 0; i < 5; i++)
                                                {
                                                    ret.blow(x , y+i);

                                                }
                                                flgDo = true;
                                                break;
                                            }

                                        case BoardPatternType.SpecialH:
                                            {
                                                ret.blow(x, y);
                                                ret.blow(x + 1, y);
                                                flgDo = true;
                                                break;
                                            }
                                        case BoardPatternType.SpecialV:
                                            {
                                                ret.blow(x, y);
                                                ret.blow(x + 1, y);
                                                flgDo = true;
                                                break;
                                            }

                                    }
                                }
                            }
                        }
                    }
                    ret.autoFill();
                    //if (count == 1)
                    //{
                    //    break;
                    //}
                    f = null;
                    cFrom = null;
                    cTo = null;
                    count++;
                }// while
                //Console.WriteLine("process count=" + count);
                return (ret);
            }

            public bool isNullOrEmpty(int x,int y)
            {
                if (table[x, y] == null) return (true);
                return (table[x, y].isEmpty());
                    
            }

            public Point upper(int x, int y)
            {
                if (y > 0)
                {
                    bool flgEmpty = false; ;
                    for (int j=y-1; j >= 0; j--)
                    {
                        if (!isNullOrEmpty(x, j))
                        {
                            return (new Point(x, j));
                        }
                        else
                        {
                            flgEmpty = true;
                        }
                    }
                    if (flgEmpty)
                    {

                    }
                    if (x > 0)
                    {
                        if (!isNullOrEmpty(x - 1, y - 1))
                        {
                            return (new Point(x - 1, y - 1));
                        }
                    }
                    if (x + 1 < NUMCELL)
                    {
                        if (!isNullOrEmpty(x + 1, y - 1))
                        {
                            return (new Point(x + 1, y - 1));
                        }
                    }
                }
                return (new Point(-1, y));
            }
            public Pos2D findFillCandy(int x, int y)
            {
                //Console.WriteLine("findFillCandy " + x + " " + y);
                Pos2D lastEmpty=null;
                Pos2D target=null;
                for (int Y = y - 1; Y >= 0; Y--)
                {
                    if (table[x,Y] == null)
                    {
                        continue;
                    }
                    if (table[x, Y].isEmpty())
                    {
                        lastEmpty = new Pos2D(x, Y);
                        continue;
                    }
                    target = new Pos2D(x, Y);
                    break;
                }
                if (target!=null)
                {
                    return (target);
                }
                if (table[x, 0] == null)
                {
                    if (Properties.Settings.Default.flgSlide)
                    {
                        if ((lastEmpty != null) && (x > 0))
                        {
                            target = findFillCandy(lastEmpty.X - 1, lastEmpty.Y);
                        }
                        if ((lastEmpty != null) && (x + 1 < NUMCELL))
                        {
                            target = findFillCandy(lastEmpty.X + 1, lastEmpty.Y);
                        }
                    }

                }

/*
                Point up = upper(x,y);
                while (up.X == -1)
                {
                    up = upper(up.X, up.Y);
                    if (up.Y == 0)
                    {
                        break;
                    }
                }
                return (up);*/
                return (target);
            }

            public FindPatternResult autoFill()
            {
                int moveCount = 0;
                clearMoveNum();
                //Console.WriteLine("autoFill()");
                for (int x = 0; x < NUMCELL; x++)
                {
                    dump("x=" + x);
                    for (int y = NUMCELL - 1; y >= 0; y--)
                    {
                        if (table[x, y] == null)
                        {
                            continue;
                        }
                        if (table[x, y].isEmpty())
                        {
/*                            int Y = y - 1;
                            for (; Y >= 0; Y--)
                            {
                                if (table[x, Y] == null)
                                {
                                    // check left and right
                                    continue;
                                }
                                if (table[x, Y].isEmpty())
                                {
                                    continue;
                                }
                                break;
                            }*/

                            Pos2D p = findFillCandy(x, y);
                            if (p != null)
                            {
                                //Console.WriteLine("FillCandy = " + p.ToString());
                                //bool flgSave = table[x, y].flgJelly;
                                table[x, y] = table[p.X, p.Y].clone();
                                table[p.X, p.Y] = emptyCandy.clone();
                                //table[p.X, p.Y].flgJelly = table[x, y].flgJelly;
                                //table[x, y].flgJelly = flgSave;
                                table[x, y].movenum = moveCount++;

                            }
                            else
                            {
                                break;
                            }
/*                            if (Y >= 0)
                            {
                                bool flgSave = table[x, y].flgJelly;
                                table[x, y] = table[x, Y].clone();
                                table[x, Y] = emptyCandy.clone();
                                table[x, Y].flgJelly = table[x, y].flgJelly;
                                table[x, y].flgJelly = flgSave;
                                table[x, y].movenum = moveCount++;
                            }*/
                        }

                    }
                }
                return (null);
            }

            public List<FindPatternResult> getPossibleMove()
            {
                List<String> hash = new List<String>();
                List<FindPatternResult> ret = new List<FindPatternResult>();
                for (int y = 0; y < NUMCELL; y++)
                {
                    for (int x = 0; x < NUMCELL; x++)
                    {
                        foreach (CandyPattern p in lstPattern)
                        {
/*                            if ((y == 7) && (x == 0))
                            {
                                Console.WriteLine("");
                            }*/
                            if (isTableMatch(table, p, x, y))
                            {
                                FindPatternResult f = new FindPatternResult();
                                f.from = new Point(x + p.target[0].X, y + p.target[0].Y);
                                f.to = new Point(x + p.target[1].X, y + p.target[1].Y);
                                if ((table[f.from.X, f.from.Y] != null) && (table[f.to.X, f.to.Y] != null))
                                {
                                    if (!hash.Contains(f.ToString()))
                                    {
                                        ret.Add(f);
                                        hash.Add(f.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
                return (ret);
            }
            public int clearMoveNum()
            {
                int ret = 0;
                for (int y = 0; y < NUMCELL; y++)
                    for (int x = 0; x < NUMCELL; x++)
                        if (table[x, y] != null)
                        {
                            table[x, y].movenum = 0;
                        }
                return (ret);
            }

            public bool isJelly(int x,int y)
            {
                if((cell[x,y]&(CellType.Jelly1 | CellType.Jelly2))>0){
                    return(true);
                }else{
                    return(false);
                }
            }

            public Score score()
            {
                Score ret = new Score();
                ret.clear();
                for (int y = 0; y < NUMCELL; y++)
                {
                    for (int x = 0; x < NUMCELL; x++)
                    {
                        if (table[x, y] != null)
                        {
                            if (isJelly(x, y))
                            {
                                ret.jelly += 50;
                                if (cell[x, y] == CellType.Jelly2)
                                {
                                    ret.jelly += 50;
                                }
                            }
                            if (table[x, y].isEmpty())
                            {
                                ret.empty++;
                            }
                            else
                            {
                                if (table[x, y].type == CandyType.StripH)
                                {
                                    ret.candy += 10;
                                }
                                if (table[x, y].type == CandyType.StripV)
                                {
                                    ret.candy += 10;
                                }
                                if (table[x, y].type == CandyType.Big)
                                {
                                    ret.candy += 10;
                                }
                                if (table[x, y].type == CandyType.Fruit)
                                {
                                    ret.fruit += (NUMCELL - y) * 50;
                                }

                            }
                        }
                    }
                }
                ret.possibleMove = getPossibleMove().Count;
                return (ret);
            }
            public const int defaultRecursive = 1;
            public FindPatternResult getBestMove(int count = defaultRecursive)
            {
                if (count == defaultRecursive)
                {
                    Console.WriteLine("getBestMove()");
                }
                Score maxScore = new Score();
                Board bestBoard;
                FindPatternResult bestMove = null;
                List<FindPatternResult> lstMove = getPossibleMove();
                int numMove = 0;
                foreach (FindPatternResult f in lstMove)
                {
                    numMove++;
                    Board b = process(f);
                    Score score = b.score();
                    if (count == defaultRecursive)
                    {
                        Console.WriteLine("Move_" + numMove + " " + f + " " + score);
                    }

                    if (count > 0)
                    {
                        FindPatternResult nextf = b.getBestMove(count - 1);
                        if (nextf != null)
                        {
                            score = nextf.score;
                            if (count == defaultRecursive)
                            {
                                Console.WriteLine("Move_" + numMove + " " +  count + " "  + f + " " + score);
                            }
                        }
                    }

                    //Console.WriteLine("   getBestMove()" + f + " score=" + score);
                    if (score.getValue() > maxScore.getValue())
                    {
                        maxScore = score;
                        bestBoard = b;
                        bestMove = f;
                        bestMove.score = score;
                        //Console.WriteLine("   getBestMove()" + bestMove + " score=" + maxScore);
                    }
                }
                if (count == defaultRecursive)
                {
                    Console.WriteLine("BestMove " + bestMove + "  " + maxScore);
                }
                return (bestMove);
            }

            public void dump(String data)
            {
                if (flgDebug)
                {
                    Console.WriteLine("dump()" + data);
                    for (int y = 0; y < NUMCELL; y++)
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int x = 0; x < NUMCELL; x++)
                        {
                            if (table[x, y] == null)
                            {
                                sb.Append("Nil\t");
                            }
                            else
                            {
                                if (table[x, y].movenum != 0)
                                {
                                    sb.Append(table[x, y].getName() + table[x, y].movenum + "\t");
                                }
                                else
                                {
                                    sb.Append(table[x, y].getName() + "\t");
                                }
                            }
                        }
                        Console.WriteLine(sb);
                    }
                }
            }

            public static bool test()
            {
                Board b = new Board();
                foreach (Pattern p in lstPattern)
                {
                    if (p is CandyPattern)
                    {
                        for (int x = 0; x < p.GetLength(1); x++)
                        {
                            for (int y = 0; y < p.GetLength(0); y++)
                            {
                                
                            }
                        }
                    }
                }
                return (false);
            }
        }


    }
}
