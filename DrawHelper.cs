using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadPlugins {
    public class DrawHelper {

        private static Database db = HostApplicationServices.WorkingDatabase;

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


        public static void DrawHorizontalDim(Point3d pt1, Point3d pt2, string text, double length) {
            using (Transaction trans = db.TransactionManager.StartTransaction()) {

                DocumentLock documentLock = Application.DocumentManager.MdiActiveDocument.LockDocument();

                BlockTable blockTbl = trans.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord modelSpace = trans.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                RotatedDimension hdim = new RotatedDimension();
                hdim.XLine1Point = pt1;
                hdim.XLine2Point = pt2;
                hdim.Rotation = System.Math.PI;

                hdim.DimLinePoint = new Point3d(0, length, 0);
                hdim.DimensionStyle = db.Dimstyle;
                hdim.DimensionText = text;
                hdim.Layer = "标注";

                modelSpace.AppendEntity(hdim);
                trans.AddNewlyCreatedDBObject(hdim, true);

                trans.Commit();

                documentLock.Dispose();
            }
        }

        public static void DrawVerticalDim(Point3d pt1, Point3d pt2, string text, double length) {
            using (Transaction trans = db.TransactionManager.StartTransaction()) {

                DocumentLock documentLock = Application.DocumentManager.MdiActiveDocument.LockDocument();

                BlockTable blockTbl = trans.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord modelSpace = trans.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                RotatedDimension hdim = new RotatedDimension();
                hdim.XLine1Point = pt1;
                hdim.XLine2Point = pt2;
                hdim.Rotation = System.Math.PI / 2;
                hdim.DimLinePoint = new Point3d(0, 5, 0);
                hdim.DimensionStyle = db.Dimstyle;
                hdim.DimensionText = text;
                hdim.Layer = "标注";

                modelSpace.AppendEntity(hdim);
                trans.AddNewlyCreatedDBObject(hdim, true);

                trans.Commit();

                documentLock.Dispose();
            }
        }

    }
}
