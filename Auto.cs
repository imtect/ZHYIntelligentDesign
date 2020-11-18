using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Internal.Calculator;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.Interop;
using CadPlugins;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace AutoBe
{
    public class Auto
    {
        public Document acDoc;
        Editor acEd;
        Database db;
        public Auto()
        {
            acDoc = Application.DocumentManager.MdiActiveDocument;
            acEd = acDoc.Editor;
            db = acDoc.Database;
        }







        public void AddBlock(string blockName, List<Entity> ents)
        {

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                BlockTable bt = (BlockTable)db.BlockTableId.GetObject(OpenMode.ForWrite);
                BlockTableRecord btr;
                if (bt.Has(blockName))
                {
                    acEd.WriteMessage(blockName + "块已经存在，将覆盖\n");
                    btr = trans.GetObject(bt[blockName], OpenMode.ForWrite) as BlockTableRecord;
                    foreach (ObjectId id in btr)
                    {
                        //先删除既有的
                        Entity ent = trans.GetObject(id, OpenMode.ForWrite) as Entity;
                        ent.Erase();
                    }
                }
                if (!bt.Has(blockName))
                {
                    btr = new BlockTableRecord();
                    btr.Name = blockName;
                    btr.Origin = Point3d.Origin;

                    bt.Add(btr);
                    trans.AddNewlyCreatedDBObject(btr, true);

                    ObjectIdCollection objCol = new ObjectIdCollection();
                    foreach (Entity ent in ents)
                    {
                        objCol.Add(ent.Id);
                    }
                    btr.AssumeOwnershipOf(objCol);

                    
                }
                
                trans.Commit();
                documentLock.Dispose();

            }
        }
        

        public void AddBlockReference(string blockName,Point3d insertionP,double rotation)
        {
            try
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                    BlockTable bt = (BlockTable)db.BlockTableId.GetObject(OpenMode.ForWrite);
                    BlockTableRecord btr = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                    if (!bt[blockName].IsNull)
                    {
                        BlockReference br = new BlockReference(insertionP, bt[blockName]);
                        br.Rotation = rotation;
                        btr.AppendEntity(br);
                        trans.AddNewlyCreatedDBObject(br, true);
                        trans.Commit();
                        documentLock.Dispose();
                        acEd.Regen();
                    }
                    else
                        acEd.WriteMessage("块创建失败");

                }
            }
            catch
            {
                acEd.WriteMessage(blockName+"块不存在");
            }
            
        }


        public Entity CreateRotateEntity(Entity ent, double ang,Point3d oriPoint)
        {
            Entity newRotateEntity = null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                Matrix3d matrix = acEd.CurrentUserCoordinateSystem;
                CoordinateSystem3d curUCS = matrix.CoordinateSystem3d;
                Matrix3d rotationMatrix = Matrix3d.Rotation(ang, curUCS.Zaxis, oriPoint);
                newRotateEntity = ent.GetTransformedCopy(rotationMatrix);
                btr.AppendEntity(newRotateEntity);
                trans.AddNewlyCreatedDBObject(newRotateEntity, true);
                trans.Commit();
                documentLock.Dispose();
            }
            return newRotateEntity;
        }

        public Entity CreateFour(Entity entity,Line3d line)
        {
            Entity newMirrorEntity = null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                Matrix3d matrix = acEd.CurrentUserCoordinateSystem;
                CoordinateSystem3d curUCS = matrix.CoordinateSystem3d;
                Matrix3d mirror = Matrix3d.Mirroring(line);
                //Matrix3d rotationMatrix = Matrix3d.Rotation(Math.PI, curUCS.Zaxis, oriPoint);
                //Matrix3d mirrors = mirror* rotationMatrix;
                newMirrorEntity = entity.GetTransformedCopy(mirror);
                

                btr.AppendEntity(newMirrorEntity);
                trans.AddNewlyCreatedDBObject(newMirrorEntity, true);
                trans.Commit();
                documentLock.Dispose();
            }
            return newMirrorEntity;
            
        }
        public void CreateFour(Entity entity, Point3d p)
        {

            Entity newMirrorEntity3 = null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                Matrix3d matrix = acEd.CurrentUserCoordinateSystem;
                CoordinateSystem3d curUCS = matrix.CoordinateSystem3d;
                Matrix3d mirror = Matrix3d.Mirroring(p);
                Matrix3d rotationMatrix = Matrix3d.Rotation(Math.PI, curUCS.Zaxis, p);
                Matrix3d mirrors = mirror * rotationMatrix;
                newMirrorEntity3 = entity.GetTransformedCopy(mirrors);
                btr.AppendEntity(newMirrorEntity3);
                trans.AddNewlyCreatedDBObject(newMirrorEntity3, true);
                Point3d x = new Point3d(p.X + 1, p.Y, 0);
                Point3d y = new Point3d(p.X, p.Y + 1, 0);
                Line3d lx = new Line3d(p, x);
                Line3d ly = new Line3d(p, y);
                Entity newMirrorEntity2 = CreateFour(newMirrorEntity3, lx);
                Entity newMirrorEntity4 = CreateFour(entity, lx);

                trans.Commit();
                documentLock.Dispose();
            }

        }
        public Entity GetEntMirrorByXline(Entity ent,Point3d center)
        {
            Entity entResult = null;
            Line3d lineX = new Line3d(new Point3d(center.X + 1, center.Y, 0), center);
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                Matrix3d matrix = acEd.CurrentUserCoordinateSystem;
                CoordinateSystem3d curUCS = matrix.CoordinateSystem3d;
                Matrix3d mirror = Matrix3d.Mirroring(lineX);
                //Matrix3d rotationMatrix = Matrix3d.Rotation(ang, curUCS.Zaxis, oriPoint);
                entResult = ent.GetTransformedCopy(mirror);
                btr.AppendEntity(entResult);
                trans.AddNewlyCreatedDBObject(entResult, true);                
                trans.Commit();
                documentLock.Dispose();
            }
            return entResult;
        }
        public Entity GetEntMirrorByYline(Entity ent, Point3d center)
        {
            Entity entResult = null;
            Line3d lineY = new Line3d(new Point3d(center.X , center.Y+1, 0), center);
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                Matrix3d matrix = acEd.CurrentUserCoordinateSystem;
                CoordinateSystem3d curUCS = matrix.CoordinateSystem3d;
                Matrix3d mirror = Matrix3d.Mirroring(lineY);
                //Matrix3d rotationMatrix = Matrix3d.Rotation(ang, curUCS.Zaxis, oriPoint);
                entResult = ent.GetTransformedCopy(mirror);
                btr.AppendEntity(entResult);
                trans.AddNewlyCreatedDBObject(entResult, true);
                trans.Commit();
                documentLock.Dispose();
            }
            return entResult;
        }
        public Entity GetEntMirrorByCenter(Entity ent, Point3d center)
        {
            Entity entResult = null;
            
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                Matrix3d matrix = acEd.CurrentUserCoordinateSystem;
                CoordinateSystem3d curUCS = matrix.CoordinateSystem3d;
                Matrix3d mirror = Matrix3d.Mirroring(center);
                //Matrix3d rotationMatrix = Matrix3d.Rotation(ang, curUCS.Zaxis, oriPoint);
                entResult = ent.GetTransformedCopy(mirror);
                btr.AppendEntity(entResult);
                trans.AddNewlyCreatedDBObject(entResult, true);
                trans.Commit();
                documentLock.Dispose();
            }
            return entResult;
        }

        public Point3d GetPointMirrorByXline(Point3d p1,Point3d center)
        {
            
            Point3d p4 = new Point3d(p1.X, 2 * center.Y - p1.Y-500, 0);
            return p4;
        }
        public Point3d GetPointMirrorByYline(Point3d p1, Point3d center)
        {
            Point3d p2 = new Point3d(2*center.X-p1.X-500, p1.Y, 0);
            return p2;
        }
        public Point3d GetPointMirrorByCenter(Point3d p1, Point3d center)
        {
            Point3d p3 = new Point3d(2 * center.X - p1.X,2*center.Y- p1.Y-500, 0);
            return p3;
        }
        public List<Point3d> GetThreePoint(Point3d p1,Point3d center)
        {
            List<Point3d> ps = new List<Point3d>();
            Point3d p2 = GetPointMirrorByYline(p1, center);
            Point3d p4 = GetPointMirrorByXline(p1, center);
            Point3d p3 = GetPointMirrorByCenter(p1, center);
            ps.Add(p2);
            ps.Add(p3);
            ps.Add(p4);
            return ps;

        }



        public void CreateFourText(string str,Point3d p1,Point3d center)
        {
            List<Point3d> ps = GetThreePoint(p1, center);
            
           foreach(var p in ps)
            {
                CreateText(str, p, 500, "0",0);
            }
        }


        public Line CreateLine(Point3d p1,Point3d p2,string layer)
        {
            Line line = new Line(p1, p2);
            line.Layer = layer;
            using(Transaction trans = db.TransactionManager.StartTransaction())
            {
                DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                btr.AppendEntity(line);
                trans.AddNewlyCreatedDBObject(line, true);
                trans.Commit();
                documentLock.Dispose();
            }
            return line;
        }
        public Polyline CreatePLine(List< Point2d> ps , string layer)
        {
            Polyline pline = new Polyline();
            for(int i=0;i<ps.Count; i++)
            {
                pline.AddVertexAt(i, ps[i], 0,0,0);
            }
            pline.Layer = layer;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                btr.AppendEntity(pline);
                trans.AddNewlyCreatedDBObject(pline, true);
                trans.Commit();
                documentLock.Dispose();
            }
            return pline;
        }

        public DBText CreateText(string textString,Point3d position,double height, string layerName,double ang=0)
        {
            DBText text = null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                text = new DBText();
                text.Rotation = ang;
                text.TextString = textString;
                text.Position = position;
                text.Height = height;
                btr.AppendEntity(text);
                trans.AddNewlyCreatedDBObject(text, true);
                trans.Commit();
                documentLock.Dispose();
            }
            return text;
        }

        public void GetAllText()
        {
            List<DBText> allText = new List<DBText>();
            TypedValue[] typeValue = new TypedValue[1];
            SelectionSet texts = null;
            typeValue.SetValue(new TypedValue(0, "Text"), 0);
            SelectionFilter textFilter = new SelectionFilter(typeValue);
            PromptSelectionResult ssPromp;
            ssPromp = acEd.SelectAll(textFilter);
            if (ssPromp.Status == PromptStatus.OK)
            {
                texts = ssPromp.Value;
            }
            if (texts == null) return;
            else
            {
                using(Transaction trans= db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        foreach (SelectedObject text in texts)
                        {
                            DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                            DBText te = trans.GetObject(text.ObjectId, OpenMode.ForWrite) as DBText;
                            te.Rotation = 0;
                            te.Oblique = 0;
                            te.IsMirroredInX = true;
                            te.IsMirroredInY = false;
                            allText.Add(te);
                            documentLock.Dispose();


                        }
                    }
                    catch (System.Exception e)
                    {
                        acEd.WriteMessage(e.Message + "," + e.Source + "," + "," + e.HelpLink + "," + e.HResult + "," + e.StackTrace);

                    }
                }
               

            }
           
        }
        public void CreateBlockByCircle(string blockName, double r, double stang, double deltaAng, Point3d center,double strheight, bool str = false)
        {
            int num = (int)(360 / deltaAng);
            for (int i = 0; i < num; i++)
            {
                double ang = (90-stang - i * deltaAng)*Math.PI/180;
                Point3d insertionP = new Point3d(center.X + r * Math.Sin(ang), center.Y + r * Math.Cos(ang), 0);
                Point3d textP= new Point3d(center.X + (r+ strheight) * Math.Sin(ang), center.Y + (r + strheight) * Math.Cos(ang), 0);
                AddBlockReference(blockName, insertionP, -ang);
                double angStr = (Math.Round(ang * 180 / Math.PI, 1) < 0)? (360 + (Math.Round(ang * 180 / Math.PI, 1))): Math.Round(ang * 180 / Math.PI, 1);
                if (str)
                CreateText(angStr.ToString(), textP, 500, "0",-ang);
            }



        }
        public Entity GetAnEntity(string message, Type entType)
        {
            Entity entity = null;
            PromptEntityOptions options = new PromptEntityOptions(message);
            if (entType != null)
            {
                options.SetRejectMessage("选择的图元类型不对，请重新选择" + entType.ToString() + "类型图元\n");
                options.AddAllowedClass(entType, true);
            }
            PromptEntityResult res = acEd.GetEntity(options);
            ObjectId objectId = res.ObjectId;
            if (objectId != null)
            {
                using(Transaction tran = db.TransactionManager.StartTransaction())
                {
                    entity = tran.GetObject(objectId, OpenMode.ForRead) as Entity;
                }
            }
            return entity;

        }
        public string Getstring(string message)
        {
            PromptStringOptions Opts = new PromptStringOptions(message);
            Opts.AllowSpaces = true;
            PromptResult Res = acDoc.Editor.GetString(Opts);
            return Res.StringResult;
        }
        public int Getint(string message, bool defaultvalueflag, int defaultvalue)
        {
            PromptIntegerOptions Opts = new PromptIntegerOptions(message);
            if (defaultvalueflag)
            {
                Opts.UseDefaultValue = defaultvalueflag;
                Opts.DefaultValue = defaultvalue;
            }
            PromptIntegerResult Res = acDoc.Editor.GetInteger(Opts);
            return Res.Value;
        }
        public double GetDouble(string message)
        {
            PromptDoubleOptions Opts = new PromptDoubleOptions(message);
            
            PromptDoubleResult Res = acDoc.Editor.GetDouble(Opts);
            return Res.Value;
        }

        public Circle CreateCircle(Point3d center, double r,string layer)
        {

            Circle circle = new Circle();            
            circle.Center = center;
            circle.Radius = r;
            circle.Layer = layer;            
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                btr.AppendEntity(circle);
                trans.AddNewlyCreatedDBObject(circle, true);
                trans.Commit();
                documentLock.Dispose();
            }
            return circle;
        }

        public void CreateLayer(string name)
        {

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                LayerTable lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite);
                ObjectId layerId = ObjectId.Null;
                if (!lt.Has(name))
                {

                    LayerTableRecord ltr = new LayerTableRecord();
                    ltr.Name = name;
                    layerId = lt.Add(ltr);
                    //ltr.Color=Color.FromKnownColor(KnownColor.)
                    trans.AddNewlyCreatedDBObject(ltr, true);
                    trans.Commit();
                }                 
                documentLock.Dispose();
            }
        }



        [CommandMethod("ClearLayer")]
        public void ClearLayer()
        {
           
            Transaction tr = db.TransactionManager.StartTransaction();
            try
            {
                using (tr)

                {
                    string selectLayerName = GetLayerName();
                    LayerTable layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                    foreach (var layerId in layerTable)
                    {
                        LayerTableRecord layer = tr.GetObject(layerId, OpenMode.ForWrite) as LayerTableRecord;
                        string layerName = layer.Name;
                        if (layerName == selectLayerName)
                        {
                            layer.IsOff = true;
                            break;
                        }
                    }
                    tr.Commit();
                }
            }
            catch(Autodesk.AutoCAD.Runtime.Exception e)
            {
                acEd.WriteMessage(e.Message+","+e.Source+"," + e.ErrorStatus + "," + e.HelpLink + "," + e.HResult+","+ e.StackTrace);
            }
            
        }

        [CommandMethod("AddMenu")]

        public void AddMenu()
        {
            AcadApplication acadApp = Application.AcadApplication as AcadApplication;
            AcadPopupMenu SBaddMenu = null;
            if (SBaddMenu == null)
            {
                SBaddMenu = acadApp.MenuGroups.Item(0).Menus.Add("提取坐标");
                SBaddMenu.AddMenuItem(SBaddMenu.Count, "画一根优雅的直线", "line ");
                AcadPopupMenu subMenu = SBaddMenu.AddSubMenu(SBaddMenu.Count, "子菜单对象");
                subMenu.AddMenuItem(SBaddMenu.Count, "画个圆", "circle ");
                SBaddMenu.AddSeparator(SBaddMenu.Count);

            }
            bool isShowed = false;
            foreach(AcadPopupMenu menu in acadApp.MenuBar)
            {
                if (menu == SBaddMenu)
                {
                    isShowed = true;
                    break;
                }
            }
            if (!isShowed)
            {
                SBaddMenu.InsertInMenuBar(acadApp.MenuBar.Count);
            }            
        }
        //menubar(0不显示，1显示)

        [CommandMethod("SB")]
        public void SB()
        {
            SBForm myForm = new SBForm();
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(myForm);
        }

        

        [CommandMethod("zhy")]
        public void ZHY()
        {
            ZHYForm zhyForm = new ZHYForm();
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(zhyForm);
        }



        public class Wall
        {
            public int ID { get; set; }
            public Point3d StartPoint { get; set; }

            public Point3d EndPoint { set; get; }

            public string Level { get; set; }

            public double Width { get; set; }

            public double Offset { get; set; }

            
        }


        public class Floor
        {
            public int ID { get; set; }
            public int CurveSize { set; get; }
            public IList<Curve> Curves{ get; set; }
            public  double Width { get; set; }
            public string Level { get; set; }
            public string Code { get; set; }
            public string UsageName { get; set; }
        }


        public class Column
        {
            public int ID { get; set; }
            public Point3d InsertionP { get; set; }
            public double length { get; set; }
            public double Width { get; set; }
            public string Level { get; set; }
            public string Code { get; set; }
            public bool IsStructure { get; set; }
        }

        public class Beam
        {
            public int ID { get; set; }
            public Point3d StartPoint { get; set; }

            public Point3d EndPoint { set; get; }

            public string Level { get; set; }

            public double Width { get; set; }

            public double Offset { get; set; }
            public double Height { set; get; }

        }


        public class Pipe
        {
            public int ID { get; set; }
            public Point3d StartPoint { get; set; }
            public Point3d EndPoint { set; get; }
            public string Level { get; set; }
            public double Diameter { get; set; }
            public double Offset { get; set; }
            public string System { get; set; }
        }

        [CommandMethod("tt")]

        public void showLayerName()
        {
            string a = GetLayerName();
            acEd.WriteMessage(a);
        }

        public string GetLayerName()
        {
            try
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Entity selEntity = null;
                    PromptEntityResult entityResult = acEd.GetEntity("选择需要隐藏的图元");
                    if (entityResult.Status == PromptStatus.OK)
                    {
                        selEntity = (Entity)tr.GetObject(entityResult.ObjectId, OpenMode.ForWrite);
                        acEd.WriteMessage(selEntity.Layer);
                        return selEntity.Layer;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch(System.Exception e)
            {
                acEd.WriteMessage(e.Message);
                return null;
            }            
            
        }

     

        public Point3d? Getpoint(string message)
        {
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // 提示起点
            pPtOpts.Message = message;
            pPtRes = acDoc.Editor.GetPoint(pPtOpts);
            Point3d ptStart = pPtRes.Value;            
            // 如果用户按 ESC 键或取消命令，就退出
            if (pPtRes.Status == PromptStatus.Cancel) return null;
            return ptStart;
        }

        public double GetLineRotation()
        {
            Point3d? st = Getpoint("拾取起始点");
            Point3d? ed = Getpoint("拾取终点点");
            //if ((ed.Value.X == st.Value.X)) return 90;
            double k = Math.Atan((ed.Value.Y - st.Value.Y) / (ed.Value.X - st.Value.X));
            return k;
        }   

        


        public SelectionSet SelectSSget(string Selectstr, Point3dCollection ptCollection, TypedValue[] acTypValAr)
        {
            //获取当前文档编辑器

            // 将过滤条件赋值给SelectionFilter对象
            SelectionFilter acSelFtr = null;
            if (acTypValAr != null) { acSelFtr = new SelectionFilter(acTypValAr); }
            // 请求在图形区域选择对象
            PromptSelectionResult acSSPrompt;
            if (Selectstr == "GetSelection")
            { acSSPrompt = acEd.GetSelection(acSelFtr); }
            else if (Selectstr == "SelectAll")
            { acSSPrompt = acEd.SelectAll(acSelFtr); }
            else if (Selectstr == "SelectCrossingPolygon")
            { acSSPrompt = acEd.SelectCrossingPolygon(ptCollection, acSelFtr); }
            else if (Selectstr == "SelectFence")
            { acSSPrompt = acEd.SelectCrossingPolygon(ptCollection, acSelFtr); }
            else if (Selectstr == "SelectWindowPolygon")
            { acSSPrompt = acEd.SelectWindowPolygon(ptCollection, acSelFtr); }
            else if (Selectstr == "SelectCrossingWindow")
            {
                Point3d pt1 = ptCollection[0];
                Point3d pt2 = ptCollection[1];
                acSSPrompt = acEd.SelectCrossingWindow(pt1, pt2, acSelFtr);
            }
            else if (Selectstr == "SelectWindow")
            {
                Point3d pt1 = ptCollection[0];
                Point3d pt2 = ptCollection[1];
                acSSPrompt = acEd.SelectWindow(pt1, pt2, acSelFtr);
            }
            else if (Selectstr == "SelectPrevious")
            {

                acSSPrompt = acEd.SelectPrevious();
            }
            else { return null; }

            // 如果提示状态OK，表示对象已选
            if (acSSPrompt.Status == PromptStatus.OK)
            {
                SelectionSet acSSet = acSSPrompt.Value;
                //caddisplay("Number of objects selected: " + acSSet.Count.ToString() + "\n");
                return acSSet;
            }
            else
            {
                //caddisplay("Number of objects selected 0 \n");
                return null;
            }
        }

    }

     
            
}
