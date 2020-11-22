using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadPlugins {
    public class DrawHelper {

        //private static Database db = HostApplicationServices.WorkingDatabase;
        private static Document acDoc = Application.DocumentManager.MdiActiveDocument;
        private static Editor acEd = acDoc.Editor;
        private static Database db = acDoc.Database;




        public Line CreateLine(Point3d p1, Point3d p2, string layer) {
            Line line = new Line(p1, p2);
            line.Layer = layer;
            using (Transaction trans = db.TransactionManager.StartTransaction()) {
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

        public static DBText CreateText(string textString, Point3d position, double height, string layerName, double ang = 0) {
            DBText text = null;
            using (Transaction trans = db.TransactionManager.StartTransaction()) {
                DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                text = new DBText();
                text.Rotation = ang;
                text.TextString = textString;
                text.Position = position;
                text.Height = height;
                text.Layer = layerName;
                btr.AppendEntity(text);
                trans.AddNewlyCreatedDBObject(text, true);
                trans.Commit();
                documentLock.Dispose();
            }
            return text;
        }

        public static Arc CreateArc(Point3d cenPt, double radius, double startAng,double endAng)
        {
            Arc arc= new Arc(cenPt, radius, startAng, endAng);            
            AddToModelSpace(arc);
            return arc;
        }
        public static DBObjectCollection CrossingPolygon(Point3dCollection pc)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            DBObjectCollection EntityCollection = new DBObjectCollection();
             PromptSelectionResult ents = ed.SelectCrossingPolygon(pc);
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }
        public static DBObjectCollection SelectObjsCrossingWindow(Point3d pt1, Point3d pt2)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.SelectCrossingWindow(pt1, pt2);
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();

                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                    documentLock.Dispose();
                }
            }
            return EntityCollection;
        }
        public static ObjectIdCollection SelectIdsCrossingWindow(Point3d pt1, Point3d pt2)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            DBObjectCollection EntityCollection = new DBObjectCollection();
            ObjectIdCollection ids = new ObjectIdCollection();
            PromptSelectionResult ents = ed.SelectCrossingWindow(pt1, pt2);
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();

                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                        ids.Add(id);
                    }
                    transaction.Commit();
                    documentLock.Dispose();
                }
            }
            return ids;
        }
        public static void Remove(DBObject obj)
        {
            Database db = obj.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                obj.Erase();
                trans.Commit();
            }
        }
        /// <summary>
        /// 删除ObjectId集合中的对象
        /// </summary>
        /// <param name="ids"></param>
        public static void Remove(ObjectIdCollection ids)
        {
            if (ids.Count == 0)
            {
                return;
            }
            //获得所选对象第一个ID所在的数据库
            Database db = ids[0].OriginalDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DocumentLock documentLock = Application.DocumentManager.MdiActiveDocument.LockDocument();
                Entity ent;
                foreach (ObjectId id in ids)
                {
                    ent = trans.GetObject(id, OpenMode.ForWrite) as Entity;
                    if (ent != null)
                        ent.Erase();
                }
                trans.Commit();
                documentLock.Dispose();
            }
        }


        public static void DrawHorizontalDim(Point3d pt1, Point3d pt2, string text, double textOffset) {
            using (Transaction trans = db.TransactionManager.StartTransaction()) {

                DocumentLock documentLock = Application.DocumentManager.MdiActiveDocument.LockDocument();

                BlockTable blockTbl = trans.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord modelSpace = trans.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                RotatedDimension hdim = new RotatedDimension();
                hdim.XLine1Point = pt1;
                hdim.XLine2Point = pt2;
                hdim.Rotation = System.Math.PI;

                hdim.DimLinePoint = new Point3d(0, 0, 0);
                hdim.DimensionStyle = db.Dimstyle;
                hdim.DimensionText = text;
                hdim.Layer = "标注";

                hdim.Dimexe = 500;

                hdim.TextPosition = new Point3d((pt1.X + pt2.X) * 0.5, (pt1.Y + pt2.Y) * 0.5 + textOffset, 0);

                modelSpace.AppendEntity(hdim);
                trans.AddNewlyCreatedDBObject(hdim, true);

                trans.Commit();

                documentLock.Dispose();
            }
        }

        public static void DrawVerticalDim(Point3d pt1, Point3d pt2, string text, double textOffset) {

            using (Transaction trans = db.TransactionManager.StartTransaction()) {

                DocumentLock documentLock = Application.DocumentManager.MdiActiveDocument.LockDocument();

                BlockTable blockTbl = trans.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord modelSpace = trans.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                RotatedDimension hdim = new RotatedDimension();
                hdim.XLine1Point = pt1;
                hdim.XLine2Point = pt2;
                hdim.Rotation = System.Math.PI / 2;
                hdim.DimLinePoint = new Point3d(0, 0, 0);
                hdim.DimensionStyle = db.Dimstyle;
                hdim.DimensionText = text;
                hdim.Layer = "标注";

                hdim.Dimexe = 500;
                hdim.TextPosition = new Point3d((pt1.X + pt2.X) * 0.5 + textOffset, (pt1.Y + pt2.Y) * 0.5, 0);

                modelSpace.AppendEntity(hdim);
                trans.AddNewlyCreatedDBObject(hdim, true);

                trans.Commit();

                documentLock.Dispose();
            }
        }

        public static void SerchAndInsertBlock(string blockNamePath, bool isCover, bool isCreate)
        {
            Point3d pt = new Point3d(0, 0, 0);
            using (Database blkDb = new Database(false, true))
            {
                blkDb.ReadDwgFile(blockNamePath, System.IO.FileShare.Read, true, null);
                blkDb.CloseInput(true);
                string blockName = System.IO.Path.GetFileNameWithoutExtension(blockNamePath);
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    DocumentLock docLock = acDoc.LockDocument();
                    BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    if (bt.Has(blockName) && isCover)
                    {
                        ObjectId blkid = acDoc.Database.Insert(blockName, blkDb, false);
                        if (isCreate)
                        {
                            BlockTableRecord btr = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                            using (BlockReference br = new BlockReference(pt, blkid))
                            {
                                btr.AppendEntity(br);
                                trans.AddNewlyCreatedDBObject(br, true);
                            }
                        }
                    }
                    else if (!bt.Has(blockName))
                    {
                        ObjectId blkid = db.Insert(blockName, blkDb, false);
                        if (isCreate)
                        {
                            BlockTableRecord btr = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                            using (BlockReference br = new BlockReference(pt, blkid))
                            {
                                btr.AppendEntity(br);
                                trans.AddNewlyCreatedDBObject(br, true);
                            }
                        }
                    }
                    trans.Commit();
                    docLock.Dispose();
                }
            }
        }

        public static ObjectIdCollection AddToModelSpace(DBObjectCollection ents)
        {
            ObjectIdCollection objIds = new ObjectIdCollection();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                foreach (DBObject obj in ents)
                {
                    Entity ent = obj as Entity;
                    if (ent != null)
                    {
                        objIds.Add(btr.AppendEntity(ent));
                        trans.AddNewlyCreatedDBObject(ent, true);
                    }
                }
                trans.Commit();
            }
            return objIds;
        }

        public static ObjectId AddToModelSpace(Entity ent)
        {
            ObjectId entId;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DocumentLock documentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                entId = btr.AppendEntity(ent);
                trans.AddNewlyCreatedDBObject(ent, true);
                trans.Commit();
                documentLock.Dispose();
            }
            return entId;
        }
        public static ObjectId AddToModelSpace(BlockTableRecord block, Point3d pt, Database db)
        {
            ObjectId blkrfid = new ObjectId();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable; BlockTableRecord modelspace = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                BlockReference br = new BlockReference(pt, block.ObjectId); // 通过块定义添加块参照
                blkrfid = modelspace.AppendEntity(br); //把块参照添加到块表记录
                trans.AddNewlyCreatedDBObject(br, true); // 通过事务添加块参照到数据库
                foreach (ObjectId id in block)
                {
                    if (id.ObjectClass.Equals(RXClass.GetClass(typeof(AttributeDefinition))))
                    {
                        AttributeDefinition ad = trans.GetObject(id, OpenMode.ForRead) as AttributeDefinition;
                        AttributeReference ar = new AttributeReference(ad.Position, ad.TextString, ad.Tag, new ObjectId());
                        br.AttributeCollection.AppendAttribute(ar);
                    }
                }
                trans.Commit();
            }
            return blkrfid;
        }
        public static LayerTableRecord GetCurrentLayer(Database db)
        {
            LayerTableRecord layer = new LayerTableRecord();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                layer = tr.GetObject(db.Clayer, OpenMode.ForRead) as LayerTableRecord;
            }
            return layer;
        }

        public static void SetCurrentLayer(LayerTableRecord layer, Database db)
        {
            if (layer.ObjectId != ObjectId.Null)
                db.Clayer = layer.ObjectId;
        }
        public static ObjectId AddInLayer(string layerName, Database db)
        {
            ObjectId layerId = ObjectId.Null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite);
                if (!lt.Has(layerName))
                {
                    LayerTableRecord ltr = new LayerTableRecord();
                    ltr.Name = layerName;
                    layerId = lt.Add(ltr);
                    trans.AddNewlyCreatedDBObject(ltr, true);
                }
                trans.Commit();
            }
            return layerId;
        }
        public static ObjectId AddInLayer(string layerName, short colorIndex, Database db)
        {

            short colorIndex1 = (short)(colorIndex % 256);//防止输入的颜色超出256
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite);
                ObjectId layerId = ObjectId.Null;
                if (lt.Has(layerName) == false)
                {
                    LayerTableRecord ltr = new LayerTableRecord();
                    ltr.Name = layerName;
                    ltr.Color = Color.FromColorIndex(ColorMethod.ByColor, colorIndex1);
                    layerId = lt.Add(ltr);
                    trans.AddNewlyCreatedDBObject(ltr, true);
                }
                trans.Commit();
                return layerId;
            }
        }
        public enum FilterType
        {
            Curve, Dimension, Polyline, BlockRef, Circle, Line, Arc, Text, Mtext, Polyline3d
        }
        public static void AddBlockReference(string blockName, Point3d insertionP, double rotation)
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
                acEd.WriteMessage(blockName + "块不存在");
            }
        }

        public static AlignedDimension AlignedDimension(Point3d pt1, Point3d pt2, Point3d ptText)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId style = db.Dimstyle;
            AlignedDimension ent = new AlignedDimension(pt1, pt2, ptText, "<>", style);
            return ent;
        }

        //public static OrdinateDimension DimOrdinateY(Point3d stratPoint, Point3d ordPt, double lenght, bool bo)
        //{
        //    Database db = HostApplicationServices.WorkingDatabase;
        //    ObjectId style = db.Dimstyle;
        //    // 由ordPth点.根据方向和长度得到一终点坐标.
           
        //    Point3d pt1 =Point(ordPt, bo ? Relation.AngToRad(180) : 0, lenght);
        //    OrdinateDimension entY = new OrdinateDimension(false, ordPt, pt1, "<>", style);
        //    entY.Origin = stratPoint;
        //    return entY;
        //}


        public static  MLeader CreateMLeader(Point3d startPt, Point3d endPt,string text)
        {
           
            //const string arrowName = "_DOT";
            //ObjectId arrId = GetArrowObjectId(arrowName);                 
            MLeader mld = new MLeader();
            int ldNum = mld.AddLeader();
            int lnNum = mld.AddLeaderLine(ldNum);
            mld.AddFirstVertex(lnNum, startPt);
            mld.AddLastVertex(lnNum, endPt);
            //mld.ArrowSymbolId = arrId;
            mld.LeaderLineType = LeaderType.StraightLeader;
            MText mt = new MText();
            mt.Contents = text;
            mt.Height = 800;
            mt.Location = endPt;
            mld.ContentType = ContentType.MTextContent;
            mld.MText = mt;
            mld.ArrowSize = 800;
            mld.TextHeight = 800;
            return mld;
        }
        //static ObjectId GetArrowObjectId(string newArrName)
        //{
        //    ObjectId arrObjId = ObjectId.Null;

        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    Database db = doc.Database;

        //    // Get the current value of DIMBLK
        //    string oldArrName = Application.GetSystemVariable("DIMBLK") as string;

        //    // Set DIMBLK to the new style
        //    // (this action may create a new block)
        //    Application.SetSystemVariable("DIMBLK", newArrName);

        //    // Reset the previous value of DIMBLK
        //    if (oldArrName.Length != 0)
        //        Application.SetSystemVariable("DIMBLK", oldArrName);

        //    // Now get the objectId of the block
        //    Transaction tr = db.TransactionManager.StartTransaction();
        //    using (tr)
        //    {
        //        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
        //        arrObjId = bt[newArrName];
        //        tr.Commit();
        //    }
        //    return arrObjId;
        //}



        public static Entity CopyTo(Entity ent, Point3d sourcePt, Point3d targetPt)
         {
         Matrix3d mt = Matrix3d.Displacement(targetPt - sourcePt);
                Entity entCopy = ent.GetTransformedCopy(mt);
         return entCopy;
         }   
         /// <summary>
         /// 指定基点与旋转角度旋转实体
         /// </summary>
         /// <param name="ent">实体对象</param>
         /// <param name="basePt">基点</param>
         /// <param name="angle">旋转角度</param>
         /// <param name="Axis">旋转轴(XY平面内旋转则设为Vector3d.ZAxis)</param>
         public static void Rotate(Entity ent, Point3d basePt, double angle, Vector3d Axis)
         {
            Matrix3d mt = Matrix3d.Rotation(angle, Axis, basePt);
            ent.TransformBy(mt);
         }  
         /// <summary>
         /// 指定基点与比例缩放实体
         /// </summary>
         /// <param name="ent">实体对象</param>
         /// <param name="basePt">基点</param>
         /// <param name="scaleFactor">缩放比例</param>
         public static void Scale(Entity ent, Point3d basePt, double scaleFactor)
         {
            Matrix3d mt = Matrix3d.Scaling(scaleFactor, basePt);
            ent.TransformBy(mt);
         }

        public static Entity CopyRotateScale(Entity ent,Point3d sourcePt,Point3d targetPt,double angle,double scale)
        {
            Matrix3d mtC = Matrix3d.Displacement(targetPt - sourcePt);
            Entity entCopy = ent.GetTransformedCopy(mtC);          
            Matrix3d mtR = Matrix3d.Rotation(angle, Vector3d.ZAxis, targetPt);           
            Matrix3d mtS = Matrix3d.Scaling(scale, targetPt);
            Matrix3d mt = mtR * mtS;
            entCopy.TransformBy(mt);
            return entCopy;
            
        }









}
    

}
