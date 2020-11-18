using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Internal.Calculator;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.Interop;
using CadPlugins;
using System.Windows.Media.Media3D;
using AutoBe;

namespace CadPlugins
{
    public partial class ZHYForm : Form
    {


        #region field
        Auto auto = new Auto();
        public double R;
        public double r;
        public double a;
        public double D;
        public double d;
        public double ringWidth;
        public double ringLap;
        public int ringNum;
        public double aMin;
        public double aMax;
        public double step;
        public double b;
        public double lap;
        public double xlineLengths;
        public double ylineLengths;
        public int four;
        public int three;
        public int two;
        public int one;
        public string fourName;
        public string threeName;
        public string twoName;
        public string oneName;
        public int numx;
        public int numy;
        public Point3d center;
        public double delDe;
        public int numBo;
        public List<double> angs;
        public double anga;
        public int numa;
        public double angb;
        public int numb;
        public double angc;
        public int numc;
        public List<BanData> bans;
        public Dictionary<int, double> banUpLength;
        #endregion
        public ZHYForm()
        {
            InitializeComponent();
            RingNumCombox.Items.Add("1");
            RingNumCombox.Items.Add("2");


            System.Data.DataTable data = new System.Data.DataTable();
            System.Data.DataColumn dc1 = new System.Data.DataColumn("id");
            System.Data.DataColumn dc2 = new System.Data.DataColumn("name");
            data.Columns.Add(dc1);
            data.Columns.Add(dc2);

            DataRow dr1 = data.NewRow();
            dr1["id"] = "1";
            dr1["name"] = "圆";
            DataRow dr2 = data.NewRow();
            dr1["id"] = "2";
            dr1["name"] = "直线";
            data.Rows.Add(dr1);
            data.Rows.Add(dr2);
            

        }

        public void Init()
        {
            D = Convert.ToDouble(Diameter.Text);
            R = D / 2;
            ringWidth = Convert.ToDouble(RingWidth.Text);
            ringLap = Convert.ToDouble(RingLap.Text);
            ringNum = 1; //Convert.ToInt32(RingNumCombox);
            lap = Convert.ToDouble(Lap.Text);
            r = D / 2 - ringWidth * ringNum + ringLap * ringNum;
            aMin = Convert.ToDouble(AMin.Text);
            aMax = Convert.ToDouble(AMax.Text);
            step = Convert.ToDouble(StepL.Text);
            xlineLengths = 0;
            ylineLengths = 0;
            four = 0;
            three = 0;
            two = 0;
            one = 0;
            numx = 0;
            numy = 0;
            delDe = Convert.ToDouble(DeltaDegree.Text);
            numBo = (int)(360 / delDe);
            angs = new List<double>();
            anga = Convert.ToDouble(angleA.Text);
            numa = (int)(360 / anga);
            angb = Convert.ToDouble(angleB.Text);
            numb = (int)(360 / angb);
            angc = Convert.ToDouble(angleC.Text);
            numc = (int)(360 / angc);
            bans = new List<BanData>();
        }
        #region MainMethod
        private void button1_Click(object sender, EventArgs e)
        {
            Run();
        }

        public void Run()
        {
            Init();
            int num = (int)((aMax - aMin) / step);
            int i;
            for (i = 0; i < num; i++)
            {
                angs.Clear();
                a = aMin + i * step - lap;
                b = a / 4;
                fourName = Math.Round((a + lap) / 100, 0).ToString();
                threeName = Math.Round((a * 3 / 4 + lap) / 100, 0).ToString();
                twoName = Math.Round((a / 2 + lap) / 100, 0).ToString();
                center = new Point3d(0, 0, 0);
                Point3d cc = GetCiccleCenter(i);
                AddLines();
                BasicCircle(cc, r);
                auto.CreateText(four.ToString(), cc, 800, "0",0);
                auto.CreateText(three.ToString(), cc, 800, "0",0);
                CreateCircleLines();
            }
        }
        private void Lining_Click(object sender, EventArgs e)
        {
            LiningPlan();
            Tk();
        }
        public void Tk()
        {
            Point3d tkOriPoint = new Point3d(-61902, -63911, 0);
            auto.AddBlockReference("tk", tkOriPoint, 0);
        }

        



        public void LiningPlan()
        {
            Circle c = auto.GetAnEntity("请选择一个圆", typeof(Circle)) as Circle;
            double R = c.Radius;
            auto.CreateBlockByCircle("af", R - 500, 1, 2.5, c.Center,0);
            auto.CreateCircle(c.Center, R - 500, "0");
            auto.CreateBlockByCircle("bf", R - 300, 1, 7.5, c.Center,1800,true);
            auto.CreateCircle(c.Center, R - 350, "0");
            auto.CreateBlockByCircle("cf", R , 7.75, 10, c.Center,1000,true);
            //auto.CreateCircle(c.Center, R - 350, "0");

            List<Entity> ents = GetAFengBlock(R);




            auto.AddBlock("aaaa", ents);
            auto.AddBlockReference("aaaa", Point3d.Origin, 0);
            ////double af = auto.GetDouble("请输入一类缝的间隔");
            ////int numa = (int)(360 / af);
            //for(int i = 0; i < numa; i++)
            //{
            //    double angA = (1 + i * anga) * Math.PI / 180;
            //    auto.AddBlockReference("af", Point3d.Origin, angA);

            //}
            //for (int i = 0; i < numb; i++)
            //{
            //    double angB = (4.75 + i * angb) * Math.PI / 180;
            //    auto.AddBlockReference("bf", Point3d.Origin, angB);

            //}
            //for (int i = 0; i < numc; i++)
            //{
            //    double angC = (2.25 + i * angc) * Math.PI / 180;
            //    auto.AddBlockReference("cf", Point3d.Origin, angC);

            //}


        }








        #endregion

        #region SubMethod
        public void AddAF(double ang,double ra)
        {

        }

        public List<Entity> GetAFengBlock(double R)
        {
            List<Point2d> ps1 = new List<Point2d>();
            List<Point2d> ps2 = new List<Point2d>();
            Point2d p1 = new Point2d(-300, R - 600 + 20);
            Point2d p2 = new Point2d(300, R - 600 + 20);
            Point2d p3 = new Point2d(300, R - 600 - 20);
            Point2d p4 = new Point2d(-300, R - 600 - 20);

            
            Point2d p5 = new Point2d(-100, R - 600 + 7);
            Point2d p6 = new Point2d(100, R - 600 + 7);
            Point2d p7 = new Point2d(100, R - 600 - 7);
            Point2d p8 = new Point2d(-100, R - 600 - 7);

            ps1.Add(p1);
            ps1.Add(p2);
            ps1.Add(p3);
            ps1.Add(p4);
            ps1.Add(p1);
            ps2.Add(p5);
            ps2.Add(p6);
            ps2.Add(p7);
            ps2.Add(p8);
            ps2.Add(p5);

            Polyline pl1 = auto.CreatePLine(ps1,"0");
            Polyline pl2 = auto.CreatePLine(ps2, "0");
            List<Entity> ents = new List<Entity>();
            ents.Add(pl1);
            ents.Add(pl2);
            
            return ents;
           
        }


        
        public Point3d GetCiccleCenter(int i)
        {
            double x = i % 10 * 250000;
            double y = (int)(i / 10) * 250000;
            Point3d center = new Point3d(x, y, 0);
            return center;
        }
        public void BasicCircle(Point3d center, double cr)
        {
            auto.CreateLayer("F");
            auto.CreateCircle(center, cr, "F");
            double cR = cr + ringWidth - ringLap;
            auto.CreateCircle(center, cR, "F");
        }
        public Line CreateCircleLine(double R, double r, double ang)
        {
            Point3d pr = new Point3d(r * Math.Cos(ang), r * Math.Sin(ang), 0);
            Point3d pR = new Point3d(R * Math.Cos(ang), R * Math.Sin(ang), 0);
            Line l = auto.CreateLine(pr, pR, "0");
            return l;
        }
        public void CreateCircleLines()
        {
            for (int i = 0; i < numBo; i++)
            {
                double ang = i * (delDe * Math.PI / 180);
                CreateCircleLine(R, r, ang);
                angs.Add(ang);
            }
        }
        public Line CreateXline(double x, double y)
        {
            Point3d p1 = new Point3d(-x, y, 0);
            Point3d p2 = new Point3d(x, y, 0);
            Point3d p3 = new Point3d(-x, -y, 0);
            Point3d p4 = new Point3d(x, -y, 0);


            Line xLine = auto.CreateLine(p1, p2, "0");
            Line xlinep = auto.CreateLine(p3, p4, "0");
            xlineLengths += 2 * xLine.Length;
            return xLine;
        }

        public Line CreateYLine(double x, double y, double del, string name, ref int num)
        {
            Point3d pt1, pt2, pt3, pt4;
            double ym = y - del;
            if (ym < 0) ym = 0;

            Point3d p1 = new Point3d(x, y, 0);
            Point3d p2 = new Point3d(x, ym, 0);

            Point3d p3 = new Point3d(-x, y, 0);
            Point3d p4 = new Point3d(-x, ym, 0);

            Point3d p5 = new Point3d(x, -y, 0);
            Point3d p6 = new Point3d(x, -ym, 0);

            Point3d p7 = new Point3d(-x, -y, 0);
            Point3d p8 = new Point3d(-x, -ym, 0);

            if (ym == 0)
            {

                if (x < SWAB(del, a, b))
                {
                    pt1 = new Point3d(x - SWAB(del, a, b) / 2, 0, 0);
                    auto.CreateText(name, pt1, 500, "0",0);                                        
                    num = num + 1;
                }
                else
                {
                    pt1 = new Point3d(x - SWAB(del, a, b) / 2, 0, 0);
                    auto.CreateText(name, pt1, 500, "0",0);
                    pt2 = new Point3d(-x + SWAB(del, a, b) / 2, 0, 0);
                    auto.CreateText(name, pt2, 500, "0", 0);
                    num += 2;
                }



            }
            else
            {
                if (x < SWAB(del, a, b))
                {
                    pt1 = new Point3d(x - SWAB(del, a, b) / 2, y - del / 2, 0);
                    pt3 = new Point3d(-x + SWAB(del, a, b) / 2, -y + del / 2, 0);
                    auto.CreateText(name, pt1, 500, "0", 0);
                    auto.CreateText(name, pt3, 500, "0", 0);
                    num += 2;
                }
                else
                {
                    pt1 = new Point3d(x - SWAB(del, a, b) / 2, y - del / 2, 0);
                    pt2 = new Point3d(-x + SWAB(del, a, b) / 2, y - del / 2, 0);
                    pt3 = new Point3d(-x + SWAB(del, a, b) / 2, -y + del / 2, 0);
                    pt4 = new Point3d(x - SWAB(del, a, b) / 2, -y + del / 2, 0);
                    auto.CreateText(name, pt1, 500, "0", 0);
                    auto.CreateText(name, pt2, 500, "0", 0);
                    auto.CreateText(name, pt3, 500, "0", 0);
                    auto.CreateText(name, pt4, 500, "0", 0);
                    num += 4;
                }

            }

            Line yLine = auto.CreateLine(p1, p2, "0");

            Line yline1 = auto.CreateLine(p3, p4, "0");
            Line yLinep = auto.CreateLine(p5, p6, "0");
            Line yLine1p = auto.CreateLine(p7, p8, "0");
            ylineLengths += 4 * yLine.Length;
            return yLine;

        }
        public void AddLines()
        {
            auto.CreateLayer("JF");
            double x = 0;
            double y = 0.5 * b - lap / 2;
            double switch_ab = b;//b
            while (y < r)//y以a、b为增量递增
            {
                x = GetPoint_X(r, y);
                double x_length = 2 * x;
                Line xline = CreateXline(x, y);//绘制横线
                double xh = -0.5 * SWAB(switch_ab, a, b) - lap / 2;//a
                while (xh < x - 500)
                {
                    if (xh > 0)
                    {
                        Line yline = CreateYLine(xh, y, switch_ab, fourName, ref four);//第一次b //绘制纵线
                    }
                    xh += SWAB(switch_ab, a, b);//a 

                }
                xh -= SWAB(switch_ab, a, b);
                if (switch_ab == b && xh < x - 500 - a / 4)
                {
                    Line yline1 = CreateYLine(xh + 3 * a / 4, y, switch_ab, threeName, ref three);//当水平增量为a时 判断是否以一般3/4区分  绘制纵线
                   
                    
                   
                }
                else if (switch_ab == b && xh < x - 500 - a / 2)
                {
                    Line yline1 = CreateYLine(xh + a / 2, y, switch_ab, twoName, ref two);//当水平增量为a时 判断是否以一般/2区分  绘制纵线
                }
                if (switch_ab == a)
                {
                    CreateXLineShort(xh, y);//水平增量为b 时   画水平短线
                }
                CreateSideboardName(xh,x, y, switch_ab, center);


                switch_ab = SWAB(switch_ab, a, b);//a

                y += switch_ab;//第一次+a；                
            }
            if (switch_ab == a)
            {
                y -= a;
                CreateYLineShort(x, y);//绘制最上侧和最下侧的纵线
            }
            double totalLength = xlineLengths + ylineLengths;
            Point3d p = new Point3d(20000, 6000, 0);
            //auto.WriteText("此方案的板长为：" + totalLength.ToString(), p, 3000, "0");
        }


        public void CreateSideboardName(double xh,double x, double y, double del, Point3d center)
        {
            if (del == b)
            {
                if (y < b)
                {
                    Point3d p = new Point3d(x - 1000, y - (del / 2), 0);
                    numx += 1;
                    DBText text = auto.CreateText(numx.ToString(), p, 500, "0");
                    Point3d p2 = auto.GetPointMirrorByYline(p, center);
                    auto.CreateText(numx.ToString(), p2, 500, "0");
                    BanData ban = new BanData();                    
                    ban.code = numx;
                    ban.longEdge = x - xh;
                    ban.shortEdge = x - xh;
                    ban.width = b;
                    ban.type = BanType.AbnormalBan;
                    ban.count = 2;
                    bans.Add(ban);

                }
                else
                {
                    Point3d p = new Point3d(x - 1000, y - (del / 2), 0);
                    numx += 1;
                    DBText text = auto.CreateText(numx.ToString(), p, 500, "0");
                    auto.CreateFourText(numx.ToString(), p, center);
                    BanData ban = new BanData();
                    ban.code = numx;
                    ban.shortEdge = GetPoint_X(r, y - b) - xh;
                    ban.longEdge = x - xh;
                    ban.width = b;
                    ban.type = BanType.AbnormalBan;
                    ban.count = 4;
                    bans.Add(ban);
                }
            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    double yp = y - (4 - i) * b - b / 2;
                    double xp = GetPoint_X(r, yp) - 1000;
                    Point3d p = new Point3d(xp - 200, yp, 0);
                    numx += 1;
                    DBText text = auto.CreateText(numx.ToString(), p, 500, "0");
                    auto.CreateFourText(numx.ToString(), p, center);
                    BanData ban = new BanData();
                    if (i == 1)
                    {
                        ban.code = numx;
                        ban.longEdge = GetPoint_X(r, y - a) - xh;
                        ban.shortEdge = xp - xh;
                        ban.width = b;
                        ban.type = BanType.AbnormalBan;
                        ban.count = 4;
                        bans.Add(ban);
                    }
                    if (i == 2 || i == 3)
                    {
                        ban.code = numx;
                        ban.longEdge = GetPoint_X(r, yp - b)-xh;
                        ban.shortEdge = xp - xh;
                        ban.width = b;
                        ban.type = BanType.AbnormalBan;
                        ban.count = 4;
                        bans.Add(ban);
                    }
                    if (i == 4)
                    {
                        ban.code = numx;
                        ban.longEdge = GetPoint_X(r, yp - b)-xh; ;
                        ban.shortEdge = x - xh;
                        ban.width = b;
                        ban.type = BanType.AbnormalBan;
                        ban.count = 4;
                        bans.Add(ban);
                    }
                    

                }
            }
        }

        public Line CreateXLineShort(double xh, double y)
        {
            int i;
            Line l = null;
            for (i = 1; i < 4; i++)
            {
                double x1 = GetPoint_X(r, (y - i * a / 4));
                if ((Math.Abs(x1 - xh) > b + 500) & i < 3)
                {
                    Point3d pUp1 = new Point3d(xh + b, y - i * b, 0);
                    Point3d pDown1 = new Point3d(xh + b, y - a, 0);
                    Point3d pUp2 = new Point3d(-xh - b, y - i * b, 0);
                    Point3d pDown2 = new Point3d(-xh - b, y - a, 0);
                    Point3d pUp3 = new Point3d(-xh - b, -y + i * b, 0);
                    Point3d pDown3 = new Point3d(-xh - b, -y + a, 0);
                    Point3d pUp4 = new Point3d(xh + b, -y + i * b, 0);
                    Point3d pDown4 = new Point3d(xh + b, -y + a, 0);
                    Line ly = auto.CreateLine(pUp1, pDown1, "0");
                    auto.CreateLine(pUp2, pDown2, "0");
                    auto.CreateLine(pUp3, pDown3, "0");
                    auto.CreateLine(pUp4, pDown4, "0");
                    ylineLengths += ly.Length;
                }
                Point3d pl11 = new Point3d(xh, y - i * a / 4, 0);
                Point3d pr11 = new Point3d(x1, y - i * a / 4, 0);

                Point3d pl12 = new Point3d(-xh, y - i * a / 4, 0);
                Point3d pr12 = new Point3d(-x1, y - i * a / 4, 0);

                Point3d pl13 = new Point3d(-xh, -y + i * a / 4, 0);
                Point3d pr13 = new Point3d(-x1, -y + i * a / 4, 0);

                Point3d pl14 = new Point3d(xh, -y + i * a / 4, 0);
                Point3d pr14 = new Point3d(x1, -y + i * a / 4, 0);

                l = auto.CreateLine(pl11, pr11, "0");
                auto.CreateLine(pl12, pr12, "0");
                auto.CreateLine(pl13, pr13, "0");
                auto.CreateLine(pl14, pr14, "0");
                xlineLengths += 4 * l.Length;
                if ((Math.Abs(x1 - xh) > b + 500) & i < 3)
                {
                    xh += b;
                }
            }
            return l;

        }



        public void CreateYLineShort(double x, double y)
        {
            Point3d p;
            double xs = b / 2 - lap / 2;
            while (xs < x - 500)
            {
                double ys = GetPoint_X(r, xs);
                Point3d p1 = new Point3d(xs, y, 0);
                Point3d p2 = new Point3d(xs, ys, 0);

                Point3d p3 = new Point3d(-xs, y, 0);
                Point3d p4 = new Point3d(-xs, ys, 0);

                Point3d p5 = new Point3d(-xs, -y, 0);
                Point3d p6 = new Point3d(-xs, -ys, 0);

                Point3d p7 = new Point3d(xs, -y, 0);
                Point3d p8 = new Point3d(xs, -ys, 0);

                Line l1 = auto.CreateLine(p1, p2, "0");
                Line l2 = auto.CreateLine(p3, p4, "0");
                Line l3 = auto.CreateLine(p5, p6, "0");
                Line l4 = auto.CreateLine(p7, p8, "0");

                ylineLengths += l1.Length * 4;
                xs += b;
            }
            xs -= b / 2;
            int topNum = 0;
            while (xs >= 0)
            {
                
                topNum += 1;
                
                numx += 1;
                p = new Point3d(xs, y + 500, 0);
                auto.CreateText(numx.ToString(), p, 500, "0", 0);
                auto.CreateFourText(numx.ToString(), p, center);
                if (topNum == 1)
                {
                    BanData ban = new BanData();
                    ban.code = numx;
                    ban.longEdge = GetPoint_X(r,(xs-b/2) - y);
                    ban.shortEdge = 0;
                    ban.width = b;
                    ban.type = BanType.AbnormalBan;
                    ban.count = 4;
                    bans.Add(ban);
                }
                else
                {
                    BanData ban = new BanData();
                    ban.code = numx;
                    ban.longEdge = GetPoint_X(r, (xs - b / 2) - y);
                    ban.shortEdge = GetPoint_X(r, (xs + b / 2) - y);
                    ban.width = b;
                    ban.type = BanType.AbnormalBan;
                    ban.count = 4;
                    bans.Add(ban);
                }
                xs -= b;
            }
            numx += 1;
            p = new Point3d(xs, y + 500, 0);
            auto.CreateText(numx.ToString(), p, 500, "0", 0);
            Point3d pp = auto.GetPointMirrorByXline(p, center);
            auto.CreateText(numx.ToString(), pp, 500, "0", 0);
            BanData ban1 = new BanData();
            ban1.code = numx;
            ban1.longEdge = GetPoint_X(r, (xs - b / 2) - y);
            ban1.shortEdge = GetPoint_X(r, (xs + b / 2) - y);
            ban1.width = b;
            ban1.type = BanType.AbnormalBan;
            ban1.count = 1;
            bans.Add(ban1);
        }

        #endregion

        #region BasicMethod
        public double GetPoint_X(double r, double y)
        {
            return Math.Sqrt(r * r - y * y);
        }

        public double SWAB(double m, double a, double b)
        {
            if (m == a) return b;
            else return a;
        }


        #endregion

        





        private void ZHYForm_Load(object sender, EventArgs e)
        {
            
            

        }





        private void Diameter_TextChanged(object sender, EventArgs e)
        {

        }

        private void RingNumCombox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        
    }
    public class BanData
    {
        public int code; //幅板的ID
        public double shortEdge; //短边的长度
        public double longEdge; //长边的长度
        public double width; //宽度
        public BanType type;
        public int count;
    }

    public enum BanType
    {
        BanLining, //中幅板
        ThreeQuartersBan, //四分之三板
        HalfBan, //二分之一板
        AbnormalBan //异形板
    }

}
