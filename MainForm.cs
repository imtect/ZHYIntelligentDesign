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
        #region 顶拱方案参数

        public double ring1;
        public double ring2;
        public double ring3;
        public double ring4;
        public double ring5;
        public double ring6;
        public double ring7;

        public int numA;
        public int numB;
        public int numC;


        #endregion




        #endregion
        public MainForm() {
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
        private void button1_Click(object sender, EventArgs e) {
            Run();
        }

        public void Run() {
            Init();
            int num = (int)((aMax - aMin) / step);
            int i;
            for (i = 0; i < num; i++) {
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
                foreach (var ban in bans) {
                    x += ban.code + ":" + ban.shortEdge + "," + ban.longEdge + "   ,  " + ban.count + ".     ";
                }

                CreateCircleLines();
            }
        }
        private void Lining_Click(object sender, EventArgs e) {
            LiningPlan();
            Tk();
        }
        public void Tk() {
            Point3d tkOriPoint = new Point3d(-61902, -63911, 0);
            auto.AddBlockReference("tk", tkOriPoint, 0);
        }





        public void LiningPlan() {


            Circle c = auto.GetAnEntity("请选择一个圆", typeof(Circle)) as Circle;
            double R = c.Radius;
            auto.CreateBlockByCircle("af", R - 500, 1, 2.5, c.Center, 0);
            auto.CreateCircle(c.Center, R - 500, "0");
            auto.CreateBlockByCircle("bf", R - 300, 1, 7.5, c.Center, 1800, true);
            auto.CreateCircle(c.Center, R - 350, "0");
            auto.CreateBlockByCircle("cf", R, 7.75, 10, c.Center, 1000, true);
            //auto.CreateCircle(c.Center, R - 350, "0");

            List<Entity> ents = GetAFengBlock(R);

            auto.AddBlock("aaaa", ents);
            auto.AddBlockReference("aaaa", Point3d.Origin, 0);







        }








        #endregion

        #region SubMethod
        public void AddAF(double ang, double ra) {

        }

        public List<Entity> GetAFengBlock(double R) {
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



        public Point3d GetCiccleCenter(int i) {
            double x = i % 10 * 250000;
            double y = (int)(i / 10) * 250000;
            Point3d center = new Point3d(x, y, 0);
            return center;
        }
        public void BasicCircle(Point3d center, double cr) {
            auto.CreateLayer("F");
            auto.CreateCircle(center, cr, "F");
            double cR = cr + ringWidth - ringLap;
            auto.CreateCircle(center, cR, "F");
        }
        public Line CreateCircleLine(double R, double r, double ang) {
            Point3d pr = new Point3d(r * Math.Cos(ang), r * Math.Sin(ang), 0);
            Point3d pR = new Point3d(R * Math.Cos(ang), R * Math.Sin(ang), 0);
            Line l = auto.CreateLine(pr, pR, "0");
            return l;
        }
        public void CreateCircleLines() {
            for (int i = 0; i < numBo; i++) {
                double ang = i * (delDe * Math.PI / 180);
                CreateCircleLine(R, r, ang);
                angs.Add(ang);
            }
        }
        public Line CreateXline(double x, double y) {
            Point3d p1 = new Point3d(-x, y, 0);
            Point3d p2 = new Point3d(x, y, 0);
            Point3d p3 = new Point3d(-x, -y, 0);
            Point3d p4 = new Point3d(x, -y, 0);


            Line xLine = auto.CreateLine(p1, p2, "0");
            Line xlinep = auto.CreateLine(p3, p4, "0");
            xlineLengths += 2 * xLine.Length;
            return xLine;
        }//改动

        public Line CreateYLine(double x, double y, double del, string name, ref int num) {
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

            if (ym == 0) {

                if (x < SWAB(del, a, b)) {
                    pt1 = new Point3d(x - SWAB(del, a, b) / 2, 0, 0);
                    auto.CreateText(name, pt1, 500, "0", 0);
                    num = num + 1;
                } else {
                    pt1 = new Point3d(x - SWAB(del, a, b) / 2, 0, 0);
                    auto.CreateText(name, pt1, 500, "0", 0);
                    pt2 = new Point3d(-x + SWAB(del, a, b) / 2, 0, 0);
                    auto.CreateText(name, pt2, 500, "0", 0);
                    num += 2;
                }



            } else {
                if (x < SWAB(del, a, b)) {
                    pt1 = new Point3d(x - SWAB(del, a, b) / 2, y - del / 2, 0);
                    pt3 = new Point3d(-x + SWAB(del, a, b) / 2, -y + del / 2, 0);
                    auto.CreateText(name, pt1, 500, "0", 0);
                    auto.CreateText(name, pt3, 500, "0", 0);
                    num += 2;
                } else {
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

        }//改动
        public void AddLines() {
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
                while (xh < x - 500) {
                    if (xh > 0) {
                        Line yline = CreateYLine(xh, y, switch_ab, fourName, ref four);//第一次b //绘制纵线
                    }
                    xh += SWAB(switch_ab, a, b);//a 
                }
                xh -= SWAB(switch_ab, a, b);
                if (switch_ab == b && xh < x - 500 - a / 4) {
                    Line yline1 = CreateYLine(xh + 3 * a / 4, y, switch_ab, threeName, ref three);//当水平增量为a时 判断是否以一般3/4区分  绘制纵线
                    xh += 3 * a / 4;

                } else if (switch_ab == b && xh < x - 500 - a / 2) {
                    Line yline1 = CreateYLine(xh + a / 2, y, switch_ab, twoName, ref two);//当水平增量为a时 判断是否以一般/2区分  绘制纵线
                    xh += a / 2;
                }
                if (switch_ab == a) {
                    CreateXLineShort(xh, y);//水平增量为b 时   画水平短线
                }
                CreateSideboardName(xh, x, y, switch_ab, center);


                switch_ab = SWAB(switch_ab, a, b);//a

                y += switch_ab;//第一次+a；                
            }
            if (switch_ab == a) {
                y -= a;
                CreateYLineShort(x, y);//绘制最上侧和最下侧的纵线
            }
            double totalLength = xlineLengths + ylineLengths;
            Point3d p = new Point3d(20000, 6000, 0);
            //auto.WriteText("此方案的板长为：" + totalLength.ToString(), p, 3000, "0");
        }//改动


        public void CreateSideboardName(double xh, double x, double y, double del, Point3d center) {
            if (del == b) {
                if (y < b) {
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

                } else {
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
            } else {
                for (int i = 1; i < 5; i++) {
                    double yp = y - (4 - i) * b - b / 2;
                    double xp = GetPoint_X(r, yp) - 1000;
                    Point3d p = new Point3d(xp - 200, yp, 0);
                    numx += 1;
                    DBText text = auto.CreateText(numx.ToString(), p, 500, "0");
                    auto.CreateFourText(numx.ToString(), p, center);
                    BanData ban = new BanData();
                    if (i == 1) {
                        ban.code = numx;
                        ban.longEdge = GetPoint_X(r, y - a) - xh;
                        ban.shortEdge = GetPoint_X(r, y - (4 - i) * b) - xh;
                        ban.width = b;
                        ban.type = BanType.AbnormalBan;
                        ban.count = 4;
                        bans.Add(ban);
                    }
                    if (i == 2 || i == 3) {
                        ban.code = numx;
                        ban.longEdge = GetPoint_X(r, y - (5 - i) * b) - xh;
                        ban.shortEdge = GetPoint_X(r, y - (4 - i) * b) - xh;
                        ban.width = b;
                        ban.type = BanType.AbnormalBan;
                        ban.count = 4;
                        bans.Add(ban);
                    }
                    if (i == 4) {
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
        }//改动

        public Line CreateXLineShort(double xh, double y)//改动
        {
            int i;
            Line l = null;
            for (i = 1; i < 4; i++) {
                double x1 = GetPoint_X(r, (y - i * a / 4));
                if ((Math.Abs(x1 - xh) > b + 500) & i < 3) {
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
                    if (i == 1) {
                        string text = threeName;
                        Point3d insertP = new Point3d(xh + b / 2, y - b - (4 - i) / 2 * b, 0);
                        auto.CreateText(text, insertP, 500, "0");
                        three += 1;
                    }
                    if (i == 2) {
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
                if ((Math.Abs(x1 - xh) > b + 500) & i < 3) {
                    xh += b;
                }

            }
            return l;

        }



        public void CreateYLineShort(double x, double y) {
            Point3d p;
            double xs = b / 2 - lap / 2;
            while (xs < x - 500) {
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
            while (xs >= 0) {

                topNum += 1;

                numx += 1;
                p = new Point3d(xs, y + 500, 0);
                auto.CreateText(numx.ToString(), p, 500, "0", 0);
                auto.CreateFourText(numx.ToString(), p, center);
                if (topNum == 1) {
                    BanData ban = new BanData();
                    ban.code = numx;
                    ban.longEdge = GetPoint_X(r, (xs - b / 2)) - y;
                    ban.shortEdge = 0;
                    ban.width = b;
                    ban.type = BanType.AbnormalBan;
                    ban.count = 4;
                    bans.Add(ban);
                } else {
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
        }//改动

        #endregion

        #region BasicMethod
        public double GetPoint_X(double r, double y) {
            return Math.Sqrt(r * r - y * y);
        }

        public double SWAB(double m, double a, double b) {
            if (m == a) return b;
            else return a;
        }


        #endregion
        #region 顶梁框架
        public void RingInit() {
            ring1 = Convert.ToDouble(Ring1.Text);
            ring2 = Convert.ToDouble(Ring2.Text);
            ring3 = Convert.ToDouble(Ring3.Text);
            ring4 = Convert.ToDouble(Ring4.Text);
            ring5 = Convert.ToDouble(Ring5.Text);
            ring6 = Convert.ToDouble(Ring6.Text);
            ring7 = Convert.ToDouble(Ring7.Text);
            numA = Convert.ToInt32(NumberA.Text);
            numB = Convert.ToInt32(NumberB.Text);
            numC = Convert.ToInt32(NumberC.Text);


        }
        public void CreateABeam() {

        }

        public void CreateOneBeam(double st, double ed, Point3d center, double ang) {
            Point3d stP = new Point3d(st * Math.Cos(ang), st * Math.Sin(ang), 0);
            Point3d edP = new Point3d(ed * Math.Cos(ang), ed * Math.Sin(ang), 0);
            auto.CreateLine(stP, edP, "0");



        }



        #endregion


        #region Calculate
        //例子
        public List<BanData> BanDatas = new List<BanData>() {
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
            new BanData(){code = 17,shortEdge = 0   ,longEdge = 3085,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 18,shortEdge = 1378,longEdge = 2283,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 19,shortEdge = 2269,longEdge = 3008,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 20,shortEdge = 2997,longEdge = 3579,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 21,shortEdge = 3570,longEdge = 4001,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 22,shortEdge = 3995,longEdge = 4280,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 23,shortEdge = 4277,longEdge = 4417,width = 2450,type = BanType.AbnormalBan },
            new BanData(){code = 97,shortEdge = 9680,longEdge = 9680,width = 2450,type = BanType.BanLining },
        };

        //已经处理的数据
        private List<int> handledBanData = new List<int>();

        //可能的组合
        private List<List<BanData>> results = new List<List<BanData>>();

        //中幅板的数据，以中幅板作为模板进行切割下料
        BanData comparer;

        public void CalculateCuttingStyle(List<BanData> BanDatas) {
            if (BanDatas == null || BanDatas.Count == 0) return;

            ClearDatas();

            //中幅板宽度
            comparer = BanDatas.Where(k => k.type == BanType.BanLining).FirstOrDefault();

            if (comparer == null) {
                comparer = new BanData() { code = 10000, shortEdge = a, longEdge = a, width = b, type = BanType.BanLining };
            }

            //排序
            BanDatas = BanDatas.Where(k => k.type != BanType.BanLining).OrderByDescending(k => k.longEdge).ToList();

            //比较
            for (int i = 0; i < BanDatas.Count; i++) {

                BanData item1 = BanDatas[i];

                if (item1.width < comparer.width) continue; //暂时去除宽度不是正常宽度的部分，后期需要考虑斜率去处理

                for (int j = i + 1; j < BanDatas.Count; j++) {

                    BanData item2 = BanDatas[j];

                    if (item2.width < comparer.width) continue;

                    double p1Lp2S = item1.longEdge + item2.shortEdge;
                    double p1Sp2L = item1.shortEdge + item2.longEdge;

                    double maxLength = p1Lp2S >= p1Sp2L ? p1Lp2S : p1Sp2L; //获取最大值

                    //最大值是否超过中幅版的长度，超过不满足
                    if (maxLength > comparer.longEdge) {
                        continue;
                    } else {
                        //不超过计算还剩多少
                        double offset = comparer.longEdge - maxLength;

                        //判断剩余的长边是否有小于该长度的，可以实现三个板子的拼接
                        BanData isThreeLining = IsExitLining(BanDatas, offset);

                        //满足两个排列组合
                        if (isThreeLining == null) {

                            if (IsValidate(item1, item2)) {
                                results.Add(new List<BanData>() { item1, item2 });

                                handledBanData.Add(item1.code);
                                handledBanData.Add(item2.code);
                            }
                        } else { //满足三个组合

                            if (IsValidate(item1, item2, isThreeLining)) {
                                results.Add(new List<BanData>() { item1, item2, isThreeLining });

                                handledBanData.Add(item1.code);
                                handledBanData.Add(item2.code);
                                handledBanData.Add(isThreeLining.code);
                            }
                        }
                    }
                }
            }
        }

        public void CalculateCuttingStyle2(List<BanData> BanDatas) {
            if (BanDatas == null || BanDatas.Count == 0) return;

            ClearDatas();

            //中幅板宽度
            comparer = BanDatas.Where(k => k.type == BanType.BanLining).FirstOrDefault();
            if (comparer == null) {
                comparer = new BanData() { code = 10000, shortEdge = a, longEdge = a, width = b, type = BanType.BanLining };
            }

            //排序
            BanDatas = BanDatas.Where(k => k.type != BanType.BanLining).OrderByDescending(k => k.longEdge).ToList();

            //比较
            for (int i = 0; i < BanDatas.Count; i++) {

                BanData item1 = BanDatas[i];

                if (item1.width < comparer.width) continue;

                for (int j = i + 1; j < BanDatas.Count; j++) {

                    BanData item2 = BanDatas[j];

                    if (item2.width < comparer.width) continue;

                    double p1Lp2S = item1.longEdge + item2.shortEdge;
                    double p1Sp2L = item1.shortEdge + item2.longEdge;

                    double maxLength = p1Lp2S >= p1Sp2L ? p1Lp2S : p1Sp2L; //获取最大值

                    //最大值是否超过中幅版的长度，超过不满足
                    if (maxLength > comparer.longEdge) {
                        continue;
                    } else {
                        //不超过计算还剩多少
                        double offset = comparer.longEdge - maxLength;

                        //判断剩余的长边是否有小于该长度的，可以实现三个板子的拼接
                        BanData isThreeLining = IsExitLining(BanDatas, offset);

                        //满足两个排列组合
                        if (isThreeLining == null) {

                            if (!IsValidate2(item1, item2))
                                results.Add(new List<BanData>() { item1, item2 });

                        } else { //满足三个组合

                            if (!IsValidate2(item1, item2, isThreeLining))
                                results.Add(new List<BanData>() { item1, item2, isThreeLining });

                        }
                    }
                }
            }

            List<int> tempData = new List<int>();
            for (int i = 0; i < results.Count; i++) {

                List<BanData> data = results[i];

                if (isExit(data, tempData)) continue;

                string str = string.Empty;
                data.ForEach(k => {
                    str += $"{k.code} - ";
                    tempData.Add(k.code);
                });
                Console.WriteLine($"可能存在的组合为：{str}");
            }
        }

        void ClearDatas() {
            if (results != null)
                results.Clear();
            if (handledBanData != null)
                handledBanData.Clear();
        }

        bool isExit(List<BanData> datas, List<int> tempData) {
            foreach (var item in datas) {
                if (tempData.Contains(item.code))
                    return true;
            }
            return false;
        }

        BanData IsExitLining(List<BanData> BanDatas, double length) {
            return BanDatas.Where(k => k.longEdge <= length).FirstOrDefault();
        }

        bool IsValidate(BanData item1, BanData item2) {
            if (item1 != null
                && item2 != null
                && handledBanData != null
                && !handledBanData.Contains(item1.code)
                && !handledBanData.Contains(item2.code))
                return true;
            return false;
        }

        bool IsValidate(BanData item1, BanData item2, BanData item3) {
            if (item1 != null
                && item2 != null
                && item3 != null
                && handledBanData != null
                && !handledBanData.Contains(item1.code)
                && !handledBanData.Contains(item2.code)
                && !handledBanData.Contains(item3.code))
                return true;
            return false;
        }

        bool IsValidate2(BanData item1, BanData item2) {
            bool isExit = false;
            results.ForEach(k => {
                if (k.Contains(item1) && k.Contains(item2))
                    isExit = true;
            });
            return isExit;
        }

        bool IsValidate2(BanData item1, BanData item2, BanData item3) {
            bool isExit = false;
            results.ForEach(k => {
                if (k.Contains(item1) && k.Contains(item2) && k.Contains(item3))
                    isExit = true;
            });
            return isExit;
        }

        #endregion

        #region Draw

        private double initPositionX; //切割下料图的初始水平位置
        private double initPositionY; //切割下料图的初始水平位置
        private double horizontalOffset; //下料图之间的水平偏移量
        private double verticalOffset; //下料图之间的垂直偏移量
        private double columeCount; //每行下料图的个数

        private double horizontalLabelOffset; //水平标注偏移量
        private double verticalLabelOffset; //垂直标注偏移量

        Point3d initPos; //下料图的初始位置

        double length = 9680;
        double width = 2450;
        public void DrawCuttingStyles(List<List<BanData>> BanDatas) {

            length = comparer.longEdge;
            width = comparer.width;

            int rowCount = 0;

            for (int i = 0; i < BanDatas.Count; i++) {
                List<BanData> items = BanDatas[i];

                if (i % columeCount == 0) rowCount++;

                var horPos = initPositionX + (i % columeCount) * horizontalOffset;
                var verPos = initPositionY + (rowCount - 1) * -verticalOffset;

                initPos = new Point3d(horPos, verPos, 0);

                if (items.Count == 2) {
                    CreateTwoLining(items, initPos);
                } else if (items.Count == 3) {
                    CreateThreeLining(items, initPos);
                }
            }
        }

        void CreateTwoLining(List<BanData> BanDatas, Point3d initPos) {
            if (BanDatas == null || BanDatas.Count == 0) return;

            BanData BanData0 = BanDatas[0];

            Point3d point00 = initPos; //左下角点
            Point3d point01 = new Point3d(initPos.X + BanData0.longEdge, initPos.Y, 0); //长边点
            Point3d point02 = new Point3d(initPos.X, initPos.Y + width, 0); //高度
            Point3d point03 = new Point3d(initPos.X + BanData0.shortEdge, initPos.Y + width, 0); //短边

            CreateLiningLine(new List<Point3d>() { point00, point01, point03, point02 }, BanData0);

            //绘制标注
            DrawHelper.DrawHorizontalDim(point00, point01, BanData0.longEdge.ToString("N0"), -horizontalLabelOffset);//长边
            DrawHelper.DrawVerticalDim(point00, point02, BanData0.width.ToString("N0"), -verticalLabelOffset);//高度
            DrawHelper.DrawHorizontalDim(point02, point03, BanData0.shortEdge.ToString("N0"), horizontalLabelOffset);//短边

            //创建数字标识
            CreateText(BanData0, point00);

            BanData BanData1 = BanDatas[1];

            Point3d point10 = new Point3d(initPos.X + length - BanData1.shortEdge, initPos.Y, 0); //短边左下角点
            Point3d point11 = new Point3d(initPos.X + length, initPos.Y, 0); //端边点；
            Point3d point12 = new Point3d(initPos.X + length, initPos.Y + width, 0); //高度
            Point3d point13 = new Point3d(initPos.X + length - BanData1.longEdge, initPos.Y + width, 0); //长边点

            CreateLiningLine(new List<Point3d>() { point10, point11, point12, point13 }, BanData1);

            //绘制标注
            DrawHelper.DrawHorizontalDim(point11, point10, BanData1.shortEdge.ToString("N0"), -horizontalLabelOffset);//短边
            DrawHelper.DrawVerticalDim(point11, point12, BanData1.width.ToString("N0"), verticalLabelOffset);//高度
            DrawHelper.DrawHorizontalDim(point12, point13, BanData1.longEdge.ToString("N0"), horizontalLabelOffset);//长边

            //创建数字标识
            CreateText(BanData1, point12, false);

            DrawHelper.DrawHorizontalDim(point02, point12, length.ToString("N0"), 2 * horizontalLabelOffset);//总长
        }

        void CreateThreeLining(List<BanData> BanDatas, Point3d initPos) {
            if (BanDatas == null || BanDatas.Count == 0) return;
            //最左边
            BanData BanData0 = BanDatas[0];

            Point3d point00 = initPos; //左下角点
            Point3d point01 = new Point3d(initPos.X + BanData0.longEdge, initPos.Y, 0); //长边点；
            Point3d point02 = new Point3d(initPos.X, initPos.Y + width, 0); //高度
            Point3d point03 = new Point3d(initPos.X + BanData0.shortEdge, initPos.Y + width, 0); //短边

            CreateLiningLine(new List<Point3d>() { point00, point01, point03, point02 }, BanData0);

            //绘制标注
            DrawHelper.DrawHorizontalDim(point00, point01, BanData0.longEdge.ToString("N0"), -horizontalLabelOffset);//长边
            DrawHelper.DrawVerticalDim(point00, point02, BanData0.longEdge.ToString("N0"), -verticalLabelOffset);//高度
            DrawHelper.DrawHorizontalDim(point02, point03, BanData0.shortEdge.ToString("N0"), horizontalLabelOffset);//短边

            //创建数字标识
            CreateText(BanData0, point00);


            //最右边
            BanData BanData2 = BanDatas[2];

            Point3d point20 = new Point3d(initPos.X + length - BanData2.longEdge, initPos.Y, 0); //短边左下角点
            Point3d point21 = new Point3d(initPos.X + length, initPos.Y, 0); //端边点；
            Point3d point22 = new Point3d(initPos.X + length, initPos.Y + width, 0); //高度
            Point3d point23 = new Point3d(initPos.X + length - BanData2.shortEdge, initPos.Y + width, 0); //长边点

            CreateLiningLine(new List<Point3d>() { point20, point21, point22, point23 }, BanData2);


            //绘制标注
            DrawHelper.DrawHorizontalDim(point21, point20, BanData2.longEdge.ToString("N0"), -horizontalLabelOffset);//短边
            DrawHelper.DrawVerticalDim(point21, point22, BanData2.width.ToString("N0"), verticalLabelOffset);//高度
            DrawHelper.DrawHorizontalDim(point22, point23, BanData2.shortEdge.ToString("N0"), horizontalLabelOffset);//长边

            //创建数字标识
            CreateText(BanData2, point22, false);

            DrawHelper.DrawHorizontalDim(point02, point22, length.ToString("N0"), verticalLabelOffset * 2);//总长

            //中间
            BanData BanData1 = BanDatas[1];

            double upOffset = length - BanData0.shortEdge - BanData1.longEdge - BanData2.shortEdge;
            double downOffset = length - BanData0.longEdge - BanData1.shortEdge - BanData2.longEdge;

            if (upOffset < downOffset) {
                double middleLeftPointX = initPos.X + BanData0.shortEdge + upOffset / 2;

                Point3d point10 = new Point3d(middleLeftPointX, initPos.Y + width, 0); //左上角
                Point3d point11 = new Point3d(middleLeftPointX + BanData1.longEdge, initPos.Y + width, 0); //右上角；
                Point3d point12 = new Point3d(point11.X, initPos.Y, 0); //右下角
                Point3d point13 = new Point3d(point11.X - BanData1.shortEdge, initPos.Y, 0); //左下角

                CreateLiningLine(new List<Point3d>() { point10, point11, point12, point13 }, BanData1);

                //绘制标注
                DrawHelper.DrawHorizontalDim(point10, point11, BanData1.longEdge.ToString("N0"), horizontalLabelOffset);//长边
                DrawHelper.DrawHorizontalDim(point13, point12, BanData1.shortEdge.ToString("N0"), -horizontalLabelOffset);//短边

                //创建数字标识
                CreateText(BanData1, point13);

            } else {
                double middleLeftPointX = initPos.X + BanData0.longEdge + downOffset / 2;

                Point3d point10 = new Point3d(middleLeftPointX, initPos.Y, 0); //短边左下角点
                Point3d point11 = new Point3d(point10.X + BanData1.shortEdge, initPos.Y, 0); //端边点；
                Point3d point12 = new Point3d(point10.X + BanData1.shortEdge, initPos.Y + width, 0); //高度
                Point3d point13 = new Point3d(point10.X + BanData1.shortEdge - BanData1.longEdge, initPos.Y + width, 0); //长边点

                CreateLiningLine(new List<Point3d>() { point10, point11, point12, point13 }, BanData1);

                //绘制标注
                DrawHelper.DrawHorizontalDim(point10, point11, BanData1.longEdge.ToString("N0"), -horizontalLabelOffset);//短边
                DrawHelper.DrawHorizontalDim(point12, point13, BanData1.shortEdge.ToString("N0"), horizontalLabelOffset);//长边

                //创建数字标识
                CreateText(BanData1, point10);
            }
        }

        void CreateLiningLine(List<Point3d> points, BanData banData) { //逆时针绘制，从左下角开始画
            if (points == null || points.Count != 4) return;
            auto.CreateLine(points[0], points[1], "0"); //长边
            auto.CreateLine(points[1], points[2], "0"); //高
            auto.CreateLine(points[2], points[3], "0"); //短边
            auto.CreateLine(points[3], points[0], "0"); //斜边
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="banData"></param>
        /// <param name="leftDownPos"></param>
        /// <param name="isNoraml">true表示长边在下边，短边在上边</param>
        void CreateText(BanData banData, Point3d leftDownPos, bool isNormal = true) {
            if (banData == null) return;
            double textHeight = 500;
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
        Point3d GetCenter(BanData banData, Point3d initPos, double textHeight, bool isNormal) {
            if (isNormal) {
                var x = initPos.X + (banData.longEdge + banData.shortEdge) * 0.25 - textHeight * 0.5;
                var y = initPos.Y + banData.width * 0.5 - textHeight * 0.5;
                return new Point3d(x, y, 0);
            } else {
                var x = initPos.X - (banData.longEdge + banData.shortEdge) * 0.25 - textHeight * 0.5;
                var y = initPos.Y - banData.width * 0.5 - textHeight * 0.5;
                return new Point3d(x, y, 0);
            }
        }

        #endregion

        #region Event

        private void GenerateCutTypeBtn_Click(object sender, EventArgs e) {
            CalculateCuttingStyle(bans);
            DrawCuttingStyles(results);
        }

        private void GenerateTestCutTypes_Click(object sender, EventArgs e) {
            CalculateCuttingStyle(BanDatas);
            DrawCuttingStyles(results);
        }

        private void GenerateAllTestCutTypes_Click(object sender, EventArgs e) {
            CalculateCuttingStyle2(BanDatas);
            DrawCuttingStyles(results);
        }

        private void button2_Click(object sender, EventArgs e) {
            CalculateCuttingStyle2(bans);
            DrawCuttingStyles(results);
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

        private void button3_Click(object sender, EventArgs e) {

        }

        private void RingCircle_Click(object sender, EventArgs e) {

        }


    }
    public class BanData {
        public int code; //幅板的ID
        public double shortEdge; //短边的长度
        public double longEdge; //长边的长度
        public double width; //宽度
        public BanType type;
        public int count;
    }

    public enum BanType {
        BanLining, //中幅板
        ThreeQuartersBan, //四分之三板
        HalfBan, //二分之一板
        AbnormalBan //异形板
    }

}
