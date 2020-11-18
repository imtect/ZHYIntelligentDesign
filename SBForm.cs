using AutoBe;
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
using System.Data.SQLite;
using System.Runtime.InteropServices;

namespace CadPlugins
{
    public partial class SBForm : Form
    {
        [DllImport("user32.dll", EntryPoint = "SetFocus")]
        public static extern int SetFocus(IntPtr hWnd);
        public SBForm()
        {
            InitializeComponent();
        }

        public void SBForm_Load(object sender, EventArgs e)
        {
            FloorcomboBox.Items.Add("F001");
            FloorcomboBox.Items.Add("F002");
            FloorcomboBox.Items.Add("F003");
            FloorcomboBox.Items.Add("F004");
            FloorcomboBox.Items.Add("F005");
            FloorcomboBox.Items.Add("F006");
            FloorcomboBox.Items.Add("F007");
            FloorcomboBox.Items.Add("F008");
            FloorcomboBox.Items.Add("F009");
            FloorcomboBox.Items.Add("F010");
            FloorcomboBox.Items.Add("B001");
            FloorcomboBox.Items.Add("B002");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Auto myCad = new Auto();
            string text1 = DB_pathTextBox.Text;


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void OpenDBBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "强选择DB文件";
            fileDialog.Filter = "db文件|*.db";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;
                DB_pathTextBox.Text = file;
            }
            
        }

        private void WritePositionInfoBtn_Click(object sender, EventArgs e)
        {
            Auto myCad = new Auto();
            Document acDoc = myCad.acDoc;
            SetFocus(acDoc.Window.Handle);
            Point3d? offSet = myCad.Getpoint("请选择偏移点");
            if (offSet == null)
            {
                MessageBox.Show("选择的点错误");
                return;
            }
            SetFocus(acDoc.Window.Handle);
            double rotation = myCad.GetLineRotation();        
            MessageBox.Show(offSet.Value.X.ToString());
            WritePositionToDb(DB_pathTextBox.Text, offSet.Value.X,offSet.Value.Y, rotation);
        }

        public void WritePositionToDb(string path,double x,double y,double rotation)
        {
            using (SQLiteConnection cn=new SQLiteConnection("Data Source="+path+ ";Pooling=true;FailIfMissing=false"))
            {
                cn.Open();
                using(SQLiteCommand cmd=new SQLiteCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = "DROP TABLE IF EXISTS Position";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText= "CREATE TABLE Position(ID INTEGER PRIMARY KEY, X FLOAT,Y FLOAT,Rotation FLOAT)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO Position values (null,"+x.ToString()+","+y.ToString()+","+rotation.ToString()+")";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void ReadBlockToDB_Click(object sender, EventArgs e)
        {
            Auto myCad = new Auto();
            Document acDoc = myCad.acDoc;
            Database acDb = acDoc.Database;
            TypedValue[] filter = new TypedValue[3];
            filter.SetValue(new TypedValue((int)DxfCode.Start, "insert"), 0);
            filter.SetValue(new TypedValue((int)8, "摄像头"), 1);
            filter.SetValue(new TypedValue((int)100, "AcDbBlockReference"), 2);
            SetFocus(acDoc.Window.Handle);
            SelectionSet blockSet = myCad.SelectSSget("GetSelection", null, filter);
            MessageBox.Show(blockSet.Count.ToString());
            List<BlockReference> cameraSets = new List<BlockReference>();

            using(Transaction acTrans = acDb.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in blockSet.GetObjectIds())

                {
                    Entity ca = acTrans.GetObject(id, OpenMode.ForRead) as Entity;
                    BlockReference care = ca as BlockReference;
                    cameraSets.Add(care);
                }              
            }
            MessageBox.Show(cameraSets.Count.ToString());

            WriteBlockPositionToDb(DB_pathTextBox.Text, cameraSets, FloorcomboBox.Text);
        }

        public void WriteBlockPositionToDb(string path,List<BlockReference> CaSet,string floors)
        {
            Auto myCad = new Auto();
            Document acDoc = myCad.acDoc;
            Database acDb = acDoc.Database;
            using (SQLiteConnection cn = new SQLiteConnection("Data Source=" + path + ";Pooling=true;FailIfMissing=false"))
            {
                cn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = cn;
                    //cmd.CommandText = "DROP TABLE IF EXISTS CameraPosition"+floors;
                    //cmd.ExecuteNonQuery();
                    //cmd.CommandText = "CREATE TABLE CameraPosition" + floors+"(ID INTEGER PRIMARY KEY, PositionX double,PositionY double,PositionZ double,RotationAng double,FloorCode string,Code string)";
                    //cmd.ExecuteNonQuery();
                    StringBuilder sb = new StringBuilder();
                    using (Transaction acTrans = acDb.TransactionManager.StartTransaction())
                    {
                        foreach (BlockReference camera in CaSet)
                        {
                            string x = (camera.Position.X / 1000).ToString();
                            string y = (camera.Position.Y / 1000).ToString();
                            string rotation = camera.Rotation.ToString();
                            string floor = "'"+floors.ToString()+"'";
                            string name = "'"+camera.Name+"'";
                            

                            BlockTableRecord btr = (BlockTableRecord)acTrans.GetObject(camera.BlockTableRecord, OpenMode.ForRead);
                            Autodesk.AutoCAD.DatabaseServices.AttributeCollection attCol = camera.AttributeCollection;

                            string strCode = null;
                            string huilu = "'null'";
                            foreach (ObjectId attId in attCol)
                            {
                                AttributeReference attRef = (AttributeReference)acTrans.GetObject(attId, OpenMode.ForRead);                             
                                if (attRef.Tag == "序号")
                                {
                                    strCode = "'" + attRef.TextString + "'";
                                }
                                
                            }

                            if (string.IsNullOrEmpty(strCode))
                            {
                                //cmd.CommandText = "INSERT INTO CameraPosition" + floors + " values (null," + x + ",null," + y + "," + rotation + ","+floor+","+name+")";
                                //cmd.ExecuteNonQuery();
                                cmd.CommandText = "INSERT INTO CameraPosition values (null," + x + ",null," + y + "," + rotation + "," + floor + "," + "'null'"  +","+name + ")";
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                //cmd.CommandText = "INSERT INTO CameraPosition" + floors + " values (null," + x + ",null," + y + "," + rotation + ",null," + strCode + ")";
                                //cmd.ExecuteNonQuery();
                                cmd.CommandText = "INSERT INTO CameraPosition values (null," + x + ",null," + y + "," + rotation + ","+floor+"," + strCode + "," + name + ")";
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                        
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();

                }
            }
        }

        public void WriteCameraLayer(string layar)
        {

        }








    }
}
