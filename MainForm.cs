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
//using Autodesk.AutoCAD.Interop;
using CadPlugins;
using System.Windows.Media.Media3D;
using AutoBe;

namespace CadPlugins {
    public partial class MainForm : Form {
        #region 参数
        # region 衬里方案参数
        Auto auto = new Auto();
        public double R,r,a,D,d,ringWidth,ringLap,aMin,aMax,step,b,lap,xlineLengths,ylineLengths, anga, angb, angc, delDe,stAng;
        public int ringNum,four,three,two,one, numa, numb, numc, numx, numy, numBo;
        public string fourName,threeName,twoName,oneName;        
        public Point3d center;            
        public List<double> angs;        
        public List<BanData> bans;
        public Dictionary<int, double> banUpLength;
        #endregion
        #region 顶拱方案参数
        public double ring1,ring2,ring3,ring4,ring5,ring6,ring7,ring8;
        public int numA,numB,numC,numAll;
        #endregion
        #endregion        
        #region 衬里方案
        public void Init() {
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
            stAng= Convert.ToDouble(StartAng.Text)*Math.PI/180;
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
            DrawHelper.SerchAndInsertBlock("D:\\deskTop\\Test1.dwg", true, false);
            
        }
        public void DrawingLining()
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
                //auto.CreateText(four.ToString(), cc, 800, "0", 0);
                //auto.CreateText(three.ToString(), cc, 800, "0", 0);
                string x = null;
                foreach (var ban in bans)
                {
                    x += ban.code + ":" + ban.shortEdge + "," + ban.longEdge + "   ,  " + ban.count + ".     ";
                }

                CreateCircleLines();
            }
        }
        public void LiningPlan()
        {


            Arc c = auto.GetAnEntity("请选择一个圆", typeof(Arc)) as Arc;
            double R = c.Radius;
            auto.CreateBlockByCircle("af", R - 500, 1, 2.5, c.Center, 0);
            auto.CreateCircle(c.Center, R - 500, "0");
            auto.CreateBlockByCircle("bf", R - 300, 1, 7.5, c.Center, 1800, true);
            auto.CreateCircle(c.Center, R - 350, "0");
            auto.CreateBlockByCircle("cf", R, 7.75, 10, c.Center, 1000, true);

            List<Entity> ents = GetAFengBlock(R);

            auto.AddBlock("aaaa", ents);
            auto.AddBlockReference("aaaa", Point3d.Origin, 0);

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

            Polyline pl1 = auto.CreatePLine(ps1, "0");
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
            DrawHelper.CreateArc(center, cr, 0, Math.PI);
            DrawHelper.CreateArc(center, cr, Math.PI, Math.PI * 2);
            //auto.CreateCircle(center, cr, "F");
            double cR = cr + ringWidth - ringLap;
            //auto.CreateCircle(center, cR, "F");
            DrawHelper.CreateArc(center, cR, 0, Math.PI);
            DrawHelper.CreateArc(center, cR, Math.PI, Math.PI * 2);
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
                    auto.CreateText(name, pt1, 500, "0", 0);
                    num = num + 1;
                }
                else
                {
                    pt1 = new Point3d(x - SWAB(del, a, b) / 2, 0, 0);
                    auto.CreateText(name, pt1, 500, "0", 0);
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
                    xh += 3 * a / 4;

                }
                else if (switch_ab == b && xh < x - 500 - a / 2)
                {
                    Line yline1 = CreateYLine(xh + a / 2, y, switch_ab, twoName, ref two);//当水平增量为a时 判断是否以一般/2区分  绘制纵线
                    xh += a / 2;
                }
                if (switch_ab == a)
                {
                    CreateXLineShort(xh, y);//水平增量为b 时   画水平短线
                }
                CreateSideboardName(xh, x, y, switch_ab, center);


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
        public void CreateSideboardName(double xh, double x, double y, double del, Point3d center)
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
                    ban.longEdge = GetPoint_X(r, y - b) - xh;
                    ban.shortEdge = x - xh;
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
                        ban.shortEdge = GetPoint_X(r, y - (4 - i) * b) - xh;
                        ban.width = b;
                        ban.type = BanType.AbnormalBan;
                        ban.count = 4;
                        bans.Add(ban);
                    }
                    if (i == 2 || i == 3)
                    {
                        ban.code = numx;
                        ban.longEdge = GetPoint_X(r, y - (5 - i) * b) - xh;
                        ban.shortEdge = GetPoint_X(r, y - (4 - i) * b) - xh;
                        ban.width = b;
                        ban.type = BanType.AbnormalBan;
                        ban.count = 4;
                        bans.Add(ban);
                    }
                    if (i == 4)
                    {
                        ban.code = numx;
                        ban.longEdge = GetPoint_X(r, yp - b / 2) - xh; ;
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
                    if (i == 1)
                    {
                        string text = threeName;
                        Point3d insertP = new Point3d(xh + b / 2, y - b - (4 - i) / 2 * b, 0);
                        auto.CreateText(text, insertP, 500, "0");
                        three += 1;
                    }
                    if (i == 2)
                    {
                        string text = twoName;
                        Point3d insertP = new Point3d(xh + b / 2, y - b - (4 - i) / 2 * b, 0);
                        auto.CreateText(text, insertP, 500, "0");
                        two += 1;
                    }

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
                    ban.longEdge = GetPoint_X(r, (xs - b / 2)) - y;
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
                    ban.longEdge = GetPoint_X(r, (xs - b / 2)) - y;
                    ban.shortEdge = GetPoint_X(r, (xs + b / 2)) - y;
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
            ban1.longEdge = GetPoint_X(r, (xs - b / 2)) - y;
            ban1.shortEdge = GetPoint_X(r, (xs + b / 2)) - y;
            ban1.width = b;
            ban1.type = BanType.AbnormalBan;
            ban1.count = 1;
            bans.Add(ban1);
        }

        public void Tk()
        {
            Point3d tkOriPoint = new Point3d(-61902, -63911, 0);
            auto.AddBlockReference("tk", tkOriPoint, 0);
            Point3d p1 = new Point3d(79648, 17050, 0);
            DrawHelper.CreateText(anga.ToString(), p1, 800, "0");
            Point3d p2 = new Point3d(88640, 17100, 0);
            DrawHelper.CreateText(angb.ToString(), p2, 800, "0");
            Point3d p3 = new Point3d(94075, 32360, 0);
            DrawHelper.CreateText(angc.ToString(), p3, 800, "0");
            Point3d p4 = new Point3d(71755 ,15520 , 0);
            DrawHelper.CreateText(numa.ToString(), p4, 800, "0");
            Point3d p5 = new Point3d(97090, 17135, 0);
            DrawHelper.CreateText(numb.ToString(), p5, 800, "0");
            Point3d p6 = new Point3d(91125, 30588, 0);
            DrawHelper.CreateText(numc.ToString(), p6, 800, "0");



        }
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
        #region 顶梁框架
        public void RingInit() {
            ring1 = Convert.ToDouble(Beam1.Text);
            ring2 = Convert.ToDouble(Beam2.Text);
            ring3 = Convert.ToDouble(Beam3.Text);
            ring4 = Convert.ToDouble(Beam4.Text);
            ring5 = Convert.ToDouble(Beam5.Text);
            ring6 = Convert.ToDouble(Beam6.Text);
            ring7 = Convert.ToDouble(Beam7.Text);
            ring8= Convert.ToDouble(Beam8.Text);
            numA = Convert.ToInt32(NumberA.Text);
            numB = Convert.ToInt32(NumberB.Text);
            numC = Convert.ToInt32(NumberC.Text);
            numAll = numA + numB + numC;
            DrawHelper.SerchAndInsertBlock("D:\\deskTop\\Test1.dwg", true, false);

        }
        public void CreateABeam() {
            double delAng = Math.PI * 2 / numAll;
            for(int i = 0; i < numA; i++)
            {
                double ang = i * (2 * Math.PI / numA);
                CreateOneBeam(ring1, ring8, center, ang);
            }
            for(int i = 0; i < numB; i++)
            {
                double ang = i * (2 * Math.PI / numB)+delAng;
                CreateOneBeam(ring3, ring8, center, ang);
            }
            for (int i = 0; i < numC; i++)
            {
                double ang = i * (2 * Math.PI / numC) +2* delAng;
                CreateOneBeam(ring4, ring8, center, ang);
            }


        }
        public void CreateOneBeam(double st, double ed, Point3d center, double ang) {
            Point3d stP = new Point3d(st * Math.Cos(ang)+center.X, st * Math.Sin(ang) + center.Y, 0);
            Point3d edP = new Point3d(ed * Math.Cos(ang) + center.X, ed * Math.Sin(ang) + center.Y, 0);
            Line l= auto.CreateLine(stP, edP, "0");
            CreateBounder(l, 150, ang);

        }
        public void CreateBounder(Line line,double width,double ang)
        {
            Point2d p1 = new Point2d(line.StartPoint.X + width / 2 * Math.Sin(ang), line.StartPoint.Y - width / 2 * Math.Cos(ang));
            Point2d p2=new Point2d(line.EndPoint.X + width / 2 * Math.Sin(ang), line.EndPoint.Y - width / 2 * Math.Cos(ang));
            Point2d p3 = new Point2d(line.StartPoint.X -width / 2 * Math.Sin(ang), line.StartPoint.Y + width / 2 * Math.Cos(ang));
            Point2d p4 = new Point2d(line.EndPoint.X - width / 2 * Math.Sin(ang), line.EndPoint.Y + width / 2 * Math.Cos(ang));
            List<Point2d> point2s = new List<Point2d>();
            point2s.Add(p1);
            point2s.Add(p2);
            point2s.Add(p4);
            point2s.Add(p3);
            point2s.Add(p1);
            auto.CreatePLine(point2s,"0");
        }

        public void CreateOneCircleRing(Point3d center, double r,double width)
        {
            auto.CreateCircle(center, r, "0");
            auto.CreateCircle(center, r - width / 2, "0");
            auto.CreateCircle(center, r + width / 2, "0");
        }
        public void CreateCircleRings(Point3d center,double width)
        {
            CreateOneCircleRing(center, ring1, 150);
            CreateOneCircleRing(center, ring2, 150);
            CreateOneCircleRing(center, ring3, 150);
            CreateOneCircleRing(center, ring4, 150);
            CreateOneCircleRing(center, ring5, 150);
            CreateOneCircleRing(center, ring6, 150);
            CreateOneCircleRing(center, ring7, 150);
            CreateOneCircleRing(center, ring8, 150);
            
        }
        private void CreateNotes()
        {
            Point3d pst = new Point3d(center.X + ring6 * Math.Cos(Math.PI * 60 / 180), center.Y + ring6 * Math.Sin(Math.PI * 60 / 180), 0);
            Point3d ped=new Point3d(center.X+5000 + ring8 * Math.Cos(Math.PI * 60 / 180), center.Y +5000+ ring8 * Math.Sin(Math.PI * 60 / 180), 0);
            MLeader m= DrawHelper.CreateMLeader(pst, ped, "环梁");
            DrawHelper.AddToModelSpace(m);
        }
        #endregion
        #region Calculate
        //例子
        public List<BanData> m_BanDatas = new List<BanData>() {
            new BanData(){code = 1, shortEdge = 3213,longEdge = 3230,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 2, shortEdge = 1867,longEdge = 2008,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 3, shortEdge = 1585,longEdge = 1870,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 4, shortEdge = 1160,longEdge = 1591,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 5, shortEdge = 2997,longEdge = 3579,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 6, shortEdge = 5884,longEdge = 6623,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 7, shortEdge = 3831,longEdge = 4840,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 8, shortEdge = 2722,longEdge = 3804,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 9, shortEdge = 1466,longEdge = 2741,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 10,shortEdge = 2408,longEdge = 3898,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 11,shortEdge = 1907,longEdge = 3639,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 12,shortEdge = 3538,longEdge = 5553,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 13,shortEdge = 1219,longEdge = 3574,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 14,shortEdge = 3296,longEdge = 6081,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 15,shortEdge = 2386,longEdge = 5755,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 16,shortEdge = 4211,longEdge = 8472,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 17,shortEdge = 0   ,longEdge = 3085,width = 1394,type = BanType.AbnormalBan },
            new BanData(){code = 18,shortEdge = 1378,longEdge = 2283,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 19,shortEdge = 2269,longEdge = 3008,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 20,shortEdge = 2997,longEdge = 3579,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 21,shortEdge = 3570,longEdge = 4001,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 22,shortEdge = 3995,longEdge = 4280,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 23,shortEdge = 4277,longEdge = 4417,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 97,shortEdge = 9680,longEdge = 9680,width = 2450,type = BanType.BanLining },
        };
        //计算一个板子可能的组合情况
        private List<List<BanData>> tempResult = new List<List<BanData>>();
        //最终确定的组合
        private List<List<BanData>> allResults = new List<List<BanData>>();
        //最终确定的组合
        private Dictionary<BanData, List<List<BanData>>> results = new Dictionary<BanData, List<List<BanData>>>();
        //已经处理的数据
        private List<int> handledBanData = new List<int>();
        //中幅板的数据，以中幅板作为模板进行切割下料
        //private BanData curComparer;

        List<BanData> tempBanDatas = new List<BanData>();


        void Calculate()
        {
            //以中幅板计算
            CalculateCuttingStyle(bans, GetComparar(BanType.BanLining));

            //以四分之三计算
            if (tempBanDatas != null && tempBanDatas.Count != 0)
            {
                CalculateCuttingStyle(tempBanDatas, GetComparar(BanType.ThreeQuartersBan));
            }
            //以二分之一计算
            if (tempBanDatas != null && tempBanDatas.Count != 0)
            {
                CalculateCuttingStyle(tempBanDatas, GetComparar(BanType.HalfBan));
            }
        }


        public void CalculateCuttingStyle(List<BanData> banDatas, BanData comparar)
        {
            if (banDatas == null || banDatas.Count == 0 || comparar == null) return;

            // curComparer = comparar;

            allResults.Clear();

            //排序，按长边进行排序
            banDatas = banDatas.Where(k => k.type != BanType.BanLining).OrderByDescending(k => k.longEdge).ToList();

            while (banDatas.Count != 0)
            {

                //计算出所有可能性（剔除同类型中面积较小的）

                tempResult.Clear();

                BanData item1 = banDatas[0];

                var banData1 = IsMatchTwoBansConditon(item1, comparar, banDatas);

                if (banData1 == null)
                {

                    allResults.Add(new List<BanData>() { item1 }); //单板

                    banDatas.Remove(item1);

                }
                else
                {
                    for (int j = 0; j < banDatas.Count; j++)
                    {

                        BanData item2 = banDatas[j];

                        double p1Lp2S = item1.longEdge + item2.shortEdge;
                        double p1Sp2L = item1.shortEdge + item2.longEdge;

                        double maxLength = p1Lp2S >= p1Sp2L ? p1Lp2S : p1Sp2L; //获取最大值

                        if (maxLength > comparar.longEdge) continue;

                        var item3 = IsExistThreeBans(banDatas, comparar.longEdge - maxLength);

                        if (item3 == null)
                        { //两板组合
                            tempResult.Add(new List<BanData>() { item1, item2 });
                        }
                        else
                        { //三板组合
                            tempResult.Add(new List<BanData>() { item1, item2, item3 });
                        }
                    }
                    //同一类中选出最大面积的可能
                    List<BanData> optimal = GetMaxBanDatas(tempResult);
                    if (optimal != null)
                    {
                        allResults.Add(optimal);

                        optimal.ForEach(k => { banDatas.Remove(k); });
                    }
                }
            }

            if (!results.ContainsKey(comparar))
                results.Add(comparar, new List<List<BanData>>());

            tempBanDatas.Clear();

            //判断是否有三个中出现相同的情况
            for (int i = 0; i < allResults.Count; i++)
            {
                var items = allResults[i];
                if (items.Count == 3 && comparar.type == BanType.BanLining)
                {
                    if (isAllDiff(items))
                    {
                        results[comparar].Add(items);
                    }
                    else
                    {
                        items.ForEach(k => {
                            if (!tempBanDatas.Contains(k))
                                tempBanDatas.Add(k);
                        });
                    }
                }
                else if (items.Count == 3 && comparar.type == BanType.ThreeQuartersBan)
                {
                    if (isAllDiff(items))
                    {
                        results[comparar].Add(items);
                    }
                    else
                    {
                        items.ForEach(k => {
                            if (!tempBanDatas.Contains(k))
                                tempBanDatas.Add(k);
                        });
                    }
                }
                else
                {
                    results[comparar].Add(items);
                }
            }
        }

        bool isAllDiff(List<BanData> banDatas)
        {
            if (banDatas == null || banDatas.Count == 0) return false;
            var code = banDatas[0].code;
            for (int i = 1; i < banDatas.Count; i++)
            {
                if (code == banDatas[i].code)
                    return false;
            }
            return true;
        }


        BanData GetComparar(BanType banType)
        {
            BanData banData = null;
            switch (banType)
            {
                case BanType.BanLining:
                    banData = new BanData() { code = 10000, shortEdge = a, longEdge = a, width = b, type = BanType.BanLining };
                    break;
                case BanType.ThreeQuartersBan:
                    banData = new BanData() { code = 10000, shortEdge = a * 3 / 4, longEdge = a * 3 / 4, width = b, type = BanType.ThreeQuartersBan };
                    break;
                case BanType.HalfBan:
                    banData = new BanData() { code = 10000, shortEdge = a * 0.5, longEdge = a * 0.5, width = b, type = BanType.HalfBan };
                    break;
            }
            return banData;
        }

        //[Obsolete]
        //public void CalculateCuttingStyle(List<BanData> banDatas) {
        //    if (banDatas == null || banDatas.Count == 0) return;
        //    ClearDatas();

        //    InitCutParamter();

        //    comparer = banDatas.Where(k => k.type == BanType.BanLining).FirstOrDefault();
        //    if (comparer == null) {
        //        comparer = new BanData() { code = 10000, shortEdge = a, longEdge = a, width = b, type = BanType.BanLining };
        //    }
        //    //排序，按长边进行排序
        //    banDatas = banDatas.Where(k => k.type != BanType.BanLining).OrderByDescending(k => k.longEdge).ToList();

        //    //计算出所有可能性（剔除同类型中面积较小的）
        //    for (int i = 0; i < banDatas.Count; i++) {

        //        tempResult.Clear();

        //        BanData item1 = banDatas[i];

        //        var banData1 = IsMatchTwoBansConditon(item1, comparer, banDatas);

        //        if (banData1 == null) {

        //            results.Add(new List<BanData>() { item1 }); //单板

        //        } else {
        //            for (int j = i; j < banDatas.Count; j++) {

        //                BanData item2 = banDatas[j];

        //                double p1Lp2S = item1.longEdge + item2.shortEdge;
        //                double p1Sp2L = item1.shortEdge + item2.longEdge;

        //                double maxLength = p1Lp2S >= p1Sp2L ? p1Lp2S : p1Sp2L; //获取最大值

        //                if (maxLength > comparer.longEdge) continue;

        //                var item3 = IsExistThreeBans(banDatas, comparer.longEdge - maxLength);

        //                if (item3 == null) { //两板组合
        //                    tempResult.Add(new List<BanData>() { item1, item2 });
        //                } else { //三板组合
        //                    tempResult.Add(new List<BanData>() { item1, item2, item3 });
        //                }

        //            }
        //            //同一类中选出最大面积的可能
        //            List<BanData> optimal = GetMaxBanDatas(tempResult);
        //            if (optimal != null) {
        //                allResults.Add(optimal);
        //            }
        //        }
        //    }

        //    //计算最终涵盖所有板子的可能性
        //    allResults = allResults.OrderByDescending(k => CalculateArea(k)).ToList(); //按面积排序

        //    for (int i = 0; i < allResults.Count; i++) {

        //        var item = allResults[i];

        //        if (!IsContained(item)) {

        //            results.Add(allResults[i]);

        //            item.ForEach(k => {
        //                if (!handledBanData.Contains(k.code))
        //                    handledBanData.Add(k.code);
        //            });
        //        }
        //    }
        //}

        /// <summary>
        /// 已经处理的是否包含该组合的所有元素
        /// </summary>
        /// <param name="banDatas"></param>
        /// <returns></returns>
        //bool IsContained(List<BanData> banDatas) {
        //    if (banDatas == null || banDatas.Count == 0) return false;
        //    bool b = true;

        //    for (int i = 0; i < banDatas.Count; i++) {
        //        if (!handledBanData.Contains(banDatas[i].code))
        //            return false;
        //    }

        //    return b;
        //}

        /// <summary>
        /// 获取面积最大的那个组合
        /// </summary>
        /// <param name="banDatas"></param>
        /// <returns></returns>
        List<BanData> GetMaxBanDatas(List<List<BanData>> banDatas)
        {
            if (banDatas == null || banDatas.Count == 0) return null;
            double maxArea = 0;
            List<BanData> maxBanDatas = null;
            for (int i = 0; i < banDatas.Count; i++)
            {
                var area = CalculateArea(banDatas[i]);
                if (area > maxArea)
                {
                    maxBanDatas = banDatas[i];
                    maxArea = area;
                }
            }
            return maxBanDatas;
        }
        /// <summary>
        /// 计算面积
        /// </summary>
        /// <param name="banDatas"></param>
        /// <returns></returns>
        double CalculateArea(List<BanData> banDatas)
        {
            if (banDatas == null || banDatas.Count == 0) return 0;

            double area = 0;
            banDatas.ForEach(k => {
                area += (k.shortEdge + k.longEdge) * k.width / 2;
            });
            return area;
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        void ClearDatas()
        {
            tempResult.Clear();
            allResults.Clear();
            results.Clear();
            handledBanData.Clear();
            tempBanDatas.Clear();
            //curComparer = null;
        }


        /// <summary>
        /// 判断是否满足两个板的组合
        /// </summary>
        /// <param name="banData">待比较的板子</param>
        /// <param name="comparar">中幅板</param>
        /// <param name="banDatas">所有异形板</param>
        /// <returns></returns>
        BanData IsMatchTwoBansConditon(BanData banData, BanData comparar, List<BanData> banDatas)
        {
            if (banData == null || comparar == null || banDatas.Count == 0) return null;
            double longDis = comparar.longEdge - banData.longEdge; //减去长边剩余的长度
            double shortDis = comparar.longEdge - banData.shortEdge; //减去短边剩余的长度
            var data = banDatas.Where(k => k.longEdge <= shortDis && k.shortEdge <= longDis);
            if (data != null)
                return data.FirstOrDefault();
            else
                return null;
        }

        /// <summary>
        /// 判断是否满足三个板的组合
        /// </summary>
        /// <param name="BanDatas"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        BanData IsExistThreeBans(List<BanData> BanDatas, double length)
        {
            if (BanDatas == null || BanDatas.Count == 0) return null;
            var result = BanDatas.Where(k => k.longEdge <= length).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 判断是否已经包含两个
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <returns></returns>
        bool IsValidate(BanData item1, BanData item2)
        {
            if (item1 != null
                && item2 != null
                && !handledBanData.Contains(item1.code)
                && !handledBanData.Contains(item2.code))
                return true;
            return false;
        }
        /// <summary>
        /// 判断是否已经包含三个
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <param name="item3"></param>
        /// <returns></returns>
        bool IsValidate(BanData item1, BanData item2, BanData item3)
        {
            if (item1 != null
                && item2 != null
                && item3 != null
                && !handledBanData.Contains(item1.code)
                && !handledBanData.Contains(item2.code)
                && !handledBanData.Contains(item3.code))
                return true;
            return false;
        }

       
        #endregion

        #region Draw

        private double initPositionX; //切割下料图的初始水平位置
        private double initPositionY; //切割下料图的初始水平位置

        private void BeamPart_Click(object sender, EventArgs e)
        {
            DrawHelper.AddBlockReference("zlaz", Point3d.Origin, 0);
        }

        private double horizontalOffset; //下料图之间的水平偏移量
        private double verticalOffset; //下料图之间的垂直偏移量
        private double columeCount; //每行下料图的个数

        private double horizontalLabelOffset; //水平标注偏移量
        private double verticalLabelOffset; //垂直标注偏移量

        private Point3d initPos; //下料图的初始位置

        private double textHeight = 500;

        public void DrawCuttingStyles(Dictionary<BanData, List<List<BanData>>> BanDatas)
        {

            int rowCount = 0;

            foreach (var item in BanDatas)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    List<BanData> items = item.Value[i];

                    if (i % columeCount == 0) rowCount++;

                    var horPos = initPositionX + (i % columeCount) * horizontalOffset;
                    var verPos = initPositionY + (rowCount - 1) * -verticalOffset;

                    initPos = new Point3d(horPos, verPos, 0);

                    //绘制板
                    if (items.Count == 1)
                    {
                        CreateOneLining(items);
                    }
                    else if (items.Count == 2)
                    {
                        CreateTwoLining(items, item.Key);
                    }
                    else if (items.Count == 3)
                    {
                        CreateThreeLining(items, item.Key);
                    }

                    CreateCountText(items, initPos, item.Key);
                }
            }

            rowCount++;

            initPos = new Point3d(initPositionX, initPositionY + (rowCount - 1) * -verticalOffset, 0);
            CreateNormalLining(BanType.BanLining, initPos, four);
            initPos = new Point3d(initPositionX + horizontalOffset, initPositionY + (rowCount - 1) * -verticalOffset, 0);
            CreateNormalLining(BanType.ThreeQuartersBan, initPos, three);
            initPos = new Point3d(initPositionX + 2 * horizontalOffset, initPositionY + (rowCount - 1) * -verticalOffset, 0);
            CreateNormalLining(BanType.HalfBan, initPos, two);
            initPos = new Point3d(initPositionX + 3 * horizontalOffset, initPositionY + (rowCount - 1) * -verticalOffset, 0);
            CreateNormalLining(BanType.QuarterBane, initPos, one);

        }

        void CreateCountText(List<BanData> items, Point3d initPos, BanData comparar)
        {
            //标注需要多少块板
            int count = 0;
            if (IsAllSame(items) && items.Count != 1)
            {
                count = 2;
            }
            else
            {
                count = 4;
            }
            var x = initPos.X + comparar.longEdge * 0.5 - 1000;
            var y = initPos.Y - 3000;
            DrawHelper.CreateText($"{count}板  {count}Plates", new Point3d(x, y, 0), textHeight, "标注1");
            auto.CreateLine(new Point3d(x, y, 0), new Point3d(x + 5000, y, 0), "0");
        }

        void CreateNormalLining(BanType banType, Point3d initPos, int count)
        {
            if (count == 0) return;

            var length = a;
            var width = b;

            switch (banType)
            {
                case BanType.ThreeQuartersBan:
                    length = a * 3 / 4;
                    break;
                case BanType.HalfBan:
                    length = a * 0.5;
                    break;
            }

            Point3d point0 = initPos;
            Point3d point1 = new Point3d(initPos.X + length, initPos.Y, 0);
            Point3d point2 = new Point3d(initPos.X + length, initPos.Y + width, 0);
            Point3d point3 = new Point3d(initPos.X, initPos.Y + width, 0);

            auto.CreateLine(point0, point1, "0");
            auto.CreateLine(point1, point2, "0");
            auto.CreateLine(point2, point3, "0");
            auto.CreateLine(point3, point0, "0");

            DrawHelper.DrawHorizontalDim(point0, point1, length.ToString("N0"), -verticalLabelOffset);//长边
            DrawHelper.DrawVerticalDim(point0, point3, width.ToString("N0"), -horizontalLabelOffset);//高度
            DrawHelper.DrawHorizontalDim(point2, point3, length.ToString("N0"), horizontalLabelOffset);//短边

            var x = initPos.X + length * 0.5 - 1000;
            var y = initPos.Y - 3000;
            DrawHelper.CreateText($"{count}板  {count}Plates", new Point3d(x, y, 0), textHeight, "标注1");
            auto.CreateLine(new Point3d(x, y, 0), new Point3d(x + 5000, y, 0), "0");
        }

        bool IsAllSame(List<BanData> banDatas)
        {
            if (banDatas == null || banDatas.Count == 0) return false;
            var temp = banDatas[0];
            for (int i = 1; i < banDatas.Count; i++)
            {
                if (temp.code != banDatas[i].code) return false;
            }
            return true;
        }

        /// <summary>
        /// 单板切割
        /// </summary>
        /// <param name="banDatas"></param>
        void CreateOneLining(List<BanData> banDatas)
        {
            if (banDatas == null || banDatas.Count == 0) return;

            BanData BanData0 = banDatas[0];

            CreateLiningLine(new List<Point3d>() {
                new Point3d( initPos.X,initPos.Y,0),   //左下角
                new Point3d(initPos.X + BanData0.longEdge, initPos.Y, 0), //右下角
                new Point3d(initPos.X + BanData0.shortEdge, initPos.Y + BanData0.width, 0), //右上角
                new Point3d(initPos.X, initPos.Y + BanData0.width, 0), //左上角
            }, banDatas[0], BanPosType.Left);
        }

        /// <summary>
        /// 双板切割
        /// </summary>
        /// <param name="BanDatas"></param>
        void CreateTwoLining(List<BanData> banDatas, BanData comparar)
        {
            if (banDatas == null || banDatas.Count == 0) return;

            var length = comparar.longEdge;

            BanData BanData0 = banDatas[0];
            CreateLiningLine(new List<Point3d>() {
                new Point3d( initPos.X,initPos.Y,0),  //左下角
                new Point3d(initPos.X + BanData0.longEdge, initPos.Y, 0), //右下角
                new Point3d(initPos.X + BanData0.shortEdge, initPos.Y + BanData0.width, 0), //右上角
                new Point3d(initPos.X, initPos.Y + BanData0.width, 0), //左上角
            }, banDatas[0], BanPosType.Left);

            BanData BanData1 = banDatas[1];
            CreateLiningLine(new List<Point3d>() {
                new Point3d(initPos.X + length - BanData1.shortEdge, initPos.Y, 0), //左下角
                new Point3d(initPos.X + length, initPos.Y, 0), //右下角
                new Point3d(initPos.X + length, initPos.Y + BanData1.width, 0), //右上角
                new Point3d(initPos.X + length - BanData1.longEdge, initPos.Y + BanData1.width, 0) //左上角
            }, BanData1, BanPosType.ShortRight);


            DrawHelper.DrawHorizontalDim(
                new Point3d(initPos.X, initPos.Y + BanData0.width, 0),
                new Point3d(initPos.X + length, initPos.Y + BanData1.width, 0),
                length.ToString("N0"), 2 * horizontalLabelOffset);  //总长
        }

        /// <summary>
        /// 三板切割
        /// </summary>
        /// <param name="BanDatas"></param>
        void CreateThreeLining(List<BanData> banDatas, BanData comparar)
        {
            if (banDatas == null || banDatas.Count == 0) return;

            var length = comparar.longEdge;

            BanData BanData0 = banDatas[0];
            CreateLiningLine(new List<Point3d>() {
                new Point3d(initPos.X,initPos.Y,0),    //左下角
                new Point3d(initPos.X + BanData0.longEdge, initPos.Y, 0), //右下角
                new Point3d(initPos.X + BanData0.shortEdge, initPos.Y + BanData0.width, 0), //右上角
                new Point3d(initPos.X, initPos.Y + BanData0.width, 0), //左上角
            }, banDatas[0], BanPosType.Left);

            BanData BanData2 = banDatas[2];
            CreateLiningLine(new List<Point3d>() {
                new Point3d(initPos.X + length - BanData2.longEdge, initPos.Y, 0), //左下角
                new Point3d(initPos.X + length, initPos.Y, 0), //右下角
                new Point3d(initPos.X + length, initPos.Y + BanData2.width, 0), //右上角
                new Point3d(initPos.X + length - BanData2.shortEdge, initPos.Y + BanData2.width, 0) //左上角
            }, BanData2, BanPosType.ShortRight);

            DrawHelper.DrawHorizontalDim(
              new Point3d(initPos.X, initPos.Y + BanData0.width, 0),
              new Point3d(initPos.X + length, initPos.Y + BanData2.width, 0),
              length.ToString("N0"), 2 * horizontalLabelOffset);  //总长

            BanData BanData1 = banDatas[1];
            //计算按那个边进行分割
            double upOffset = length - BanData0.shortEdge - BanData1.longEdge - BanData2.shortEdge;
            double downOffset = length - BanData0.longEdge - BanData1.shortEdge - BanData2.longEdge;
            double offset = upOffset < downOffset ? upOffset * 0.5 : downOffset * 0.5;

            double middleLeftPointX = initPos.X + BanData0.longEdge + offset;

            //避免出现左中上交叉情况
            double downLenght = BanData0.longEdge + offset + BanData1.shortEdge;
            double upLenght = BanData0.shortEdge + BanData1.longEdge;
            if (downLenght < upLenght)
            {
                double offset2 = upLenght - downLenght; //去掉交叉的部分
                middleLeftPointX = initPos.X + BanData0.longEdge + offset + offset2 + (downOffset * 0.5 - offset) * 0.5;
            }

            //避免出现中右下交叉
            double middleLeftOffset = initPos.X + length - middleLeftPointX - BanData1.shortEdge - BanData2.longEdge;
            if (middleLeftOffset < 0)
            {
                var ss = middleLeftPointX + BanData1.shortEdge - BanData1.longEdge - BanData0.shortEdge + middleLeftOffset - initPos.X;
                middleLeftPointX = middleLeftPointX + middleLeftOffset - ss * 0.5;
            }

            CreateLiningLine(new List<Point3d>() {
                    new Point3d(middleLeftPointX, initPos.Y , 0), //左下角
                    new Point3d(middleLeftPointX + BanData1.shortEdge, initPos.Y, 0), //右下角
                    new Point3d(middleLeftPointX + BanData1.shortEdge, initPos.Y + BanData1.width, 0), // 右上角
                    new Point3d(middleLeftPointX + BanData1.shortEdge- BanData1.longEdge, initPos.Y + BanData1.width, 0) //左上角
                }, BanData1, BanPosType.ShortMiddle);
        }

        /// <summary>
        /// 逆时针排列点进行绘制
        /// </summary>
        /// <param name="points"></param>
        /// <param name="banData"></param>
        /// <param name="type"></param>
        void CreateLiningLine(List<Point3d> points, BanData banData, BanPosType type)
        { //逆时针绘制，从左下角开始画
            if (points == null || points.Count != 4) return;

            auto.CreateLine(points[0], points[1], "0");
            auto.CreateLine(points[1], points[2], "0");
            auto.CreateLine(points[2], points[3], "0");
            auto.CreateLine(points[3], points[0], "0");

            //绘制标注
            if (type == BanPosType.Left)
            {
                DrawHelper.DrawVerticalDim(points[0], points[3], banData.width.ToString("N0"), -verticalLabelOffset);//高度
                DrawHelper.DrawHorizontalDim(points[0], points[1], banData.longEdge.ToString("N0"), -horizontalLabelOffset);//长边
                DrawHelper.DrawHorizontalDim(points[2], points[3], banData.shortEdge.ToString("N0"), horizontalLabelOffset);//短边
            }
            else if (type == BanPosType.ShortMiddle)
            {
                DrawHelper.DrawHorizontalDim(points[0], points[1], banData.shortEdge.ToString("N0"), -horizontalLabelOffset);//长边
                DrawHelper.DrawHorizontalDim(points[2], points[3], banData.longEdge.ToString("N0"), horizontalLabelOffset);//短边
            }
            else if (type == BanPosType.LongMiddle)
            {
                DrawHelper.DrawHorizontalDim(points[0], points[1], banData.longEdge.ToString("N0"), -horizontalLabelOffset);//长边
                DrawHelper.DrawHorizontalDim(points[2], points[3], banData.shortEdge.ToString("N0"), horizontalLabelOffset);//短边
            }
            else if (type == BanPosType.ShortRight)
            {
                DrawHelper.DrawHorizontalDim(points[0], points[1], banData.shortEdge.ToString("N0"), -horizontalLabelOffset);//短边
                DrawHelper.DrawVerticalDim(points[1], points[2], banData.width.ToString("N0"), verticalLabelOffset);//高度
                DrawHelper.DrawHorizontalDim(points[2], points[3], banData.longEdge.ToString("N0"), horizontalLabelOffset);//长边
            }
            else if (type == BanPosType.LongRight)
            {
                DrawHelper.DrawHorizontalDim(points[0], points[1], banData.longEdge.ToString("N0"), -horizontalLabelOffset);//长边
                DrawHelper.DrawVerticalDim(points[1], points[2], banData.width.ToString("N0"), verticalLabelOffset);//高度
                DrawHelper.DrawHorizontalDim(points[2], points[3], banData.shortEdge.ToString("N0"), horizontalLabelOffset);//短边
            }
            //创建数字标识
            CreateText(banData, points[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="banData"></param>
        /// <param name="leftDownPos"></param>
        /// <param name="isNoraml">true表示长边在下边，短边在上边</param>
        void CreateText(BanData banData, Point3d leftDownPos, bool isNormal = true)
        {
            if (banData == null) return;
            DrawHelper.CreateText(banData.code.ToString() + "L", GetCenter(banData, leftDownPos, textHeight, isNormal), textHeight, "标注1");
        }

        /// <summary>
        /// 计算文字标注的中心点位置
        /// </summary>
        /// <param name="banData"></param>
        /// <param name="initPos"></param>
        /// <param name="textHeight"></param>
        /// <param name="isNormal"></param>
        /// <returns></returns>
        Point3d GetCenter(BanData banData, Point3d initPos, double textHeight, bool isNormal)
        {
            if (isNormal)
            {
                var x = initPos.X + (banData.longEdge + banData.shortEdge) * 0.25 - textHeight * 0.5;
                var y = initPos.Y + banData.width * 0.5 - textHeight * 0.5;
                return new Point3d(x, y, 0);
            }
            else
            {
                var x = initPos.X - (banData.longEdge + banData.shortEdge) * 0.25 - textHeight * 0.5;
                var y = initPos.Y - banData.width * 0.5 - textHeight * 0.5;
                return new Point3d(x, y, 0);
            }
        }

        void DreateDiffBans()
        {

        }

        #endregion
        #region Event
        public MainForm()
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
        private void BottomAndRing_Click(object sender, EventArgs e)
        {
            Point3d p3 = new Point3d(-(r+10000), -(r+10000), 0);
            Point3d p4 = new Point3d(r+10000, r+10000, 0);
            DBObjectCollection objs = DrawHelper.SelectObjsCrossingWindow(p3, p4);
            foreach(Entity obj in objs)
            {               
                Entity ent= DrawHelper.CopyRotateScale(obj,new Point3d(0,0,0),new Point3d(81287.6820,17801.3108 , 0.0000),stAng,0.5);
                DrawHelper.AddToModelSpace(ent);
            }
            Point3d p1 = new Point3d(-(R+5000), -1000, 0);
            Point3d p2 = new Point3d(r+5000, -(R+10000), 0);
            ObjectIdCollection delObjs = DrawHelper.SelectIdsCrossingWindow(p1, p2);



            DrawHelper.Remove(delObjs);            
            DrawHelper.AddBlockReference("tk2", Point3d.Origin, 0);

        }
        private void button1_Click(object sender, EventArgs e)
        {
            DrawingLining();
        }
        private void Lining_Click(object sender, EventArgs e)
        {
            Init();
            LiningPlan();
            Tk();
        }
        private void button3_Click(object sender, EventArgs e)
        {

        }
        private void RingCircle_Click(object sender, EventArgs e)
        {
            RingInit();
            CreateABeam();
            CreateCircleRings(center, 150);
            CreateNotes();
            Point3d p2 = new Point3d(center.X + 75000, center.Y, 0);
            CreateLittleCircle(p2, 150);
            CreateLittleBeam(p2);
            DrawHelper.AddBlockReference("tkgd", Point3d.Origin, 0);


        }
        public void CreateLittleBeam(Point3d p2)
        {
            double delAng = Math.PI * 2 / numAll;
            for (int i = 0; i < numA; i++)
            {
                double ang = i * (2 * Math.PI / numA);
                CreateOneBeam(ring1, 4000, p2, ang);
            }
            for (int i = 0; i < numB; i++)
            {
                double ang = i * (2 * Math.PI / numB) + delAng;
                CreateOneBeam(ring2, 4000, p2, ang);
            }
          


        }
        private void TopPositin_Click(object sender, EventArgs e)
        {
            Point3d p = new Point3d(0, -ring8 - 25000, 0);
            DrawHelper.AddBlockReference("kj", p, 0);
            AddPositionDim(p);

        }
        public void AddPositionDim(Point3d point)
        {
            Point3d p11 = new Point3d(0, point.Y - 3000, 0);
            Point3d p12 = new Point3d(ring1, point.Y - 3000, 0);
            DrawHelper.DrawHorizontalDim(p11, p12, ring1.ToString(), -1000);
            Point3d p21 = new Point3d(0, point.Y - 4500, 0);
            Point3d p22 = new Point3d(ring2, point.Y - 4500, 0);
            DrawHelper.DrawHorizontalDim(p21, p22, ring2.ToString(), -1000);
            
            Point3d p31 = new Point3d(0, point.Y - 6000, 0);
            Point3d p32 = new Point3d(ring3, point.Y - 6000, 0);
            DrawHelper.DrawHorizontalDim(p31, p32, ring3.ToString(), -1000);
            Point3d p41 = new Point3d(0, point.Y - 7500, 0);
            Point3d p42 = new Point3d(ring4, point.Y - 7500, 0);
            DrawHelper.DrawHorizontalDim(p41, p42, ring4.ToString(), -1000);

            Point3d p51 = new Point3d(0, point.Y - 9000, 0);
            Point3d p52 = new Point3d(ring5, point.Y - 9000, 0);
            DrawHelper.DrawHorizontalDim(p51, p52, ring5.ToString(), -1000);
            Point3d p61 = new Point3d(0, point.Y - 10500, 0);
            Point3d p62 = new Point3d(ring6, point.Y - 10500, 0);
            DrawHelper.DrawHorizontalDim(p61, p62, ring6.ToString(), -1000);

            Point3d p71 = new Point3d(0, point.Y - 12000, 0);
            Point3d p72 = new Point3d(ring7, point.Y - 12000, 0);
            DrawHelper.DrawHorizontalDim(p71, p72, ring7.ToString(), -1000);
            Point3d p81 = new Point3d(0, point.Y - 13500, 0);
            Point3d p82 = new Point3d(ring8, point.Y - 13500, 0);
            DrawHelper.DrawHorizontalDim(p81, p82, ring8.ToString(), -1000);
        }





        private void CreateLittleCircle(Point3d p2, double width)
        {
            CreateOneCircleRing(p2, ring1, 150);
            CreateOneCircleRing(p2, ring2, 150);
    
        }

        private void GenerateCutTypeBtn_Click(object sender, EventArgs e)
        {
            ClearDatas();
            InitCutParamter();
            //CalculateCuttingStyle(bans, GetComparar(BanType.BanLining));
            Calculate();
            DrawCuttingStyles(results);
            DrawHelper.AddBlockReference("tk0", Point3d.Origin, 0);
        }

        private void GenerateTestCutTypes_Click(object sender, EventArgs e)
        {
            CalculateCuttingStyle(m_BanDatas, GetComparar(BanType.BanLining));
            DrawCuttingStyles(results);
        }

        private void GenerateAllTestCutTypes_Click(object sender, EventArgs e)
        {
            CalculateCuttingStyle(m_BanDatas, GetComparar(BanType.BanLining));
            DrawCuttingStyles(results);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            CalculateCuttingStyle(bans, GetComparar(BanType.BanLining));
            DrawCuttingStyles(results);
        }

        private void InitCutParamter()
        {
            initPositionX = Convert.ToDouble(textBox7.Text);
            initPositionY = Convert.ToDouble(textBox13.Text);
            columeCount = Convert.ToDouble(textBox10.Text);
            horizontalOffset = Convert.ToDouble(textBox9.Text);
            verticalOffset = Convert.ToDouble(textBox11.Text);
            horizontalLabelOffset = Convert.ToDouble(textBox2.Text);
            verticalLabelOffset = Convert.ToDouble(textBox4.Text);
        }
        private void textBox7_TextChanged(object sender, EventArgs e) {
            if (textBox7.Text != null)
                initPositionX = Convert.ToDouble(textBox7.Text);
        }
        private void textBox13_TextChanged(object sender, EventArgs e) {
            if (textBox13.Text != null)
                initPositionY = Convert.ToDouble(textBox13.Text);
        }
        private void MainForm_Load(object sender, EventArgs e) {
            initPositionX = Convert.ToDouble(textBox7.Text);
            initPositionY = Convert.ToDouble(textBox13.Text);
            columeCount = Convert.ToDouble(textBox10.Text);
            horizontalOffset = Convert.ToDouble(textBox9.Text);
            verticalOffset = Convert.ToDouble(textBox11.Text);
            horizontalLabelOffset = Convert.ToDouble(textBox2.Text);
            verticalLabelOffset = Convert.ToDouble(textBox4.Text);
        }
        private void textBox10_TextChanged(object sender, EventArgs e) {
            if (textBox10.Text != null)
                columeCount = Convert.ToDouble(textBox10.Text);
        }
        private void textBox9_TextChanged(object sender, EventArgs e) {
            if (textBox9.Text != null)
                horizontalOffset = Convert.ToDouble(textBox9.Text);
        }
        private void textBox11_TextChanged(object sender, EventArgs e) {
            if (textBox11.Text != null)
                verticalOffset = Convert.ToDouble(textBox11.Text);
        }
        private void textBox2_TextChanged(object sender, EventArgs e) {
            if (textBox2.Text != null)
                horizontalLabelOffset = Convert.ToDouble(textBox2.Text);
        }
        private void textBox4_TextChanged(object sender, EventArgs e) {
            if (textBox4.Text != null)
                verticalLabelOffset = Convert.ToDouble(textBox4.Text);
        }
        #endregion        
    }
   
}
