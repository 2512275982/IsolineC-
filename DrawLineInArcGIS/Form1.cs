using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF.Connection.Local;
using ESRI.ArcGIS.ADF;
using Hykj.GISModule;
using Hykj.GISModule.Isobands;

namespace DrawLineInArcGIS
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            IAoInitialize lisence = new AoInitializeClass();
            lisence.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.axMapControl1.LoadMxFile(Application.StartupPath + "\\map.mxd");
        }

        private void Delete2()
        {
            IMap map = this.axMapControl1.Map;
            ILayer pntLayer = map.get_Layer(0);
            IFeatureLayer featLayer = pntLayer as IFeatureLayer;
            IFeatureClass featClass = featLayer.FeatureClass;

            ILayer lineLayer = map.get_Layer(1);
            IFeatureLayer featLineLayer = lineLayer as IFeatureLayer;
            IFeatureClass featLineClass = featLineLayer.FeatureClass;

            ILayer polygonLayer = map.get_Layer(2);
            IFeatureClass polygonClass = (polygonLayer as IFeatureLayer).FeatureClass;

            IFeatureDataset featDataset = featClass.FeatureDataset;
            IWorkspace workspace = featDataset.Workspace;
            IWorkspaceEdit workspaceEdit = workspace as IWorkspaceEdit;
            workspaceEdit.StartEditing(false);
            workspaceEdit.StartEditOperation();

            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = "";

            ITable table = (ITable)featClass;
            table.DeleteSearchedRows(pQueryFilter);

            ITable lineTable = (ITable)featLineClass;
            lineTable.DeleteSearchedRows(pQueryFilter);

            ITable polygonTable = (ITable)polygonClass;
            polygonTable.DeleteSearchedRows(pQueryFilter);

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Delete2();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            IMap map = this.axMapControl1.Map;
            ILayer lineLayer = map.get_Layer(1);
            IFeatureLayer featLineLayer = lineLayer as IFeatureLayer;
            IFeatureClass featLineClass = featLineLayer.FeatureClass;

            ILayer pntLayer = map.get_Layer(0);
            IFeatureLayer pntFeatLayer = pntLayer as IFeatureLayer;
            IFeatureClass pntClass = pntFeatLayer.FeatureClass;

            IFeatureDataset featDataset = pntClass.FeatureDataset;
            IWorkspace workspace = featDataset.Workspace;
            IWorkspaceEdit workspaceEdit = workspace as IWorkspaceEdit;

            GridIsoline gridIsoline = TestIsoline.ReadJsonFile();
            List<IsoLineInfo> listLines = gridIsoline.Isolines;
            List<IsoPolygonInfo> listPolys = gridIsoline.IsoBands;

            workspaceEdit.StartEditing(false);
            workspaceEdit.StartEditOperation();

            int sunCount = 0;

            using (ComReleaser comReleaser = new ComReleaser())
            {
                int index = pntClass.FindField("Value");
                IFeatureBuffer featBuffer = pntClass.CreateFeatureBuffer();
                comReleaser.ManageLifetime(featBuffer);

                int indexLine = featLineClass.FindField("Value");
                IFeatureBuffer lineBuffer = featLineClass.CreateFeatureBuffer();
                comReleaser.ManageLifetime(lineBuffer);

                IFeatureCursor lineCursor = featLineClass.Insert(true);
                comReleaser.ManageLifetime(lineCursor);

                IFeatureCursor insertCursor = pntClass.Insert(true);
                comReleaser.ManageLifetime(insertCursor);
                for (int i = 0; i < listLines.Count; i++)
                {
                    IsoLineInfo lines = listLines[i];

                    IPointCollection pntColl = new PolylineClass();
                    for (int j = 0; j < lines.ListVertrix.Count; j++)
                    {
                        IPoint pnt = new PointClass();
                        pnt.X = lines.ListVertrix[j].X;
                        pnt.Y = lines.ListVertrix[j].Y;

                        pntColl.AddPoint(pnt);
                    }

                    IPolyline line = pntColl as IPolyline;

                    lineBuffer.Shape = line;
                    lineBuffer.set_Value(indexLine, lines.LineValue);
                    lineCursor.InsertFeature(lineBuffer);

                    if (!lines.FinishState)
                    {
                        Hykj.GISModule.PointInfo pntFrom = lines.FromPoint;//.GetLineFrom();
                        Hykj.GISModule.PointInfo pntEnd = lines.ToPoint;//.GetLineEnd();

                        IPoint pntF = new PointClass();
                        pntF.X = pntFrom.PntCoord.X;
                        pntF.Y = pntFrom.PntCoord.Y;

                        featBuffer.Shape = pntF;
                        featBuffer.set_Value(index, pntFrom.Z);
                        insertCursor.InsertFeature(featBuffer);

                        IPoint pntE = new PointClass();
                        pntE.X = pntEnd.PntCoord.X;
                        pntE.Y = pntEnd.PntCoord.Y;

                        featBuffer.Shape = pntE;
                        featBuffer.set_Value(index, pntEnd.Z);
                        insertCursor.InsertFeature(featBuffer);

                        sunCount++;
                    }
                }
                insertCursor.Flush();
                lineCursor.Flush();
            }

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);
            MessageBox.Show(sunCount.ToString());
        }

        private void btnTestIsoband_Click(object sender, EventArgs e)
        {
            IMap map = this.axMapControl1.Map;
            ILayer polygonLayer = map.get_Layer(2);
            IFeatureLayer featPolyLayer = polygonLayer as IFeatureLayer;
            IFeatureClass featPolyClass = featPolyLayer.FeatureClass;

            GridIsoline gridIsoline = TestIsoline.ReadJsonFile();
            List<IsoPolygonInfo> listPolys = gridIsoline.IsoBands;

            IFeatureDataset featDataset = featPolyClass.FeatureDataset;
            IWorkspace workspace = featDataset.Workspace;
            IWorkspaceEdit workspaceEdit = workspace as IWorkspaceEdit;

            workspaceEdit.StartEditing(false);
            workspaceEdit.StartEditOperation();

            int sunCount = 0;

            using (ComReleaser comReleaser = new ComReleaser())
            {
                int indexLine = featPolyClass.FindField("Value");
                IFeatureBuffer polygonBuffer = featPolyClass.CreateFeatureBuffer();
                comReleaser.ManageLifetime(polygonBuffer);

                IFeatureCursor polyCursor = featPolyClass.Insert(true);
                comReleaser.ManageLifetime(polyCursor);

                for (int i = 0; i < listPolys.Count; i++)
                {
                    IsoPolygonInfo poly = listPolys[i];

                    IPolygon polygon = new PolygonClass();
                    IGeometryCollection geomCol = polygon as IGeometryCollection;

                    IPointCollection pntColl = new RingClass();
                    for (int j = 0; j < poly.OuterRing.Vertries.Count; j++)
                    {
                        IPoint pnt = new PointClass();
                        pnt.X = poly.OuterRing.Vertries[j].X;
                        pnt.Y = poly.OuterRing.Vertries[j].Y;

                        pntColl.AddPoint(pnt);
                    }
                    IRing outerRing = pntColl as IRing;
                    geomCol.AddGeometry(outerRing);

                    for (int ij = 0; ij < poly.InterRings.Count; ij++)
                    {
                        IPointCollection pntColIn = new RingClass();

                        List<PointCoord> listVertrix = poly.InterRings[ij].Vertries;
                        for (int j = 0; j < listVertrix.Count; j++)
                        {
                            IPoint pnt = new PointClass();
                            pnt.X = listVertrix[j].X;
                            pnt.Y = listVertrix[j].Y;

                            pntColIn.AddPoint(pnt);
                        }
                        IRing inerRing = pntColIn as IRing;
                        geomCol.AddGeometry(inerRing);
                    }

                    polygonBuffer.Shape = polygon;
                    polygonBuffer.set_Value(indexLine, poly.MaxValue);
                    polyCursor.InsertFeature(polygonBuffer);
                }
                polyCursor.Flush();
            }

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);
            MessageBox.Show(sunCount.ToString());
        }

        private void btnRing_Click(object sender, EventArgs e)
        {
            //IMap map = this.axMapControl1.Map;
            //ILayer polygonLayer = map.get_Layer(2);
            //IFeatureLayer featPolyLayer = polygonLayer as IFeatureLayer;
            //IFeatureClass featPolyClass = featPolyLayer.FeatureClass;

            //List<IsoRingInfo> listPolys = TestIsoline.ReadJsonFile1();

            //IFeatureDataset featDataset = featPolyClass.FeatureDataset;
            //IWorkspace workspace = featDataset.Workspace;
            //IWorkspaceEdit workspaceEdit = workspace as IWorkspaceEdit;

            //workspaceEdit.StartEditing(false);
            //workspaceEdit.StartEditOperation();

            //using (ComReleaser comReleaser = new ComReleaser())
            //{
            //    int indexLine = featPolyClass.FindField("Value");
            //    int indexType = featPolyClass.FindField("TYPE");
            //    IFeatureBuffer polygonBuffer = featPolyClass.CreateFeatureBuffer();
            //    comReleaser.ManageLifetime(polygonBuffer);

            //    IFeatureCursor polyCursor = featPolyClass.Insert(true);
            //    comReleaser.ManageLifetime(polyCursor);

            //    for (int i = 0; i < listPolys.Count; i++)
            //    {
            //        IsoRingInfo poly = listPolys[i];

            //        IPolygon polygon = new PolygonClass();
            //        IGeometryCollection geomCol = polygon as IGeometryCollection;

            //        IPointCollection pntColl = new RingClass();
            //        for (int j = 0; j < poly.IsoRing.Vertries.Count; j++)
            //        {
            //            IPoint pnt = new PointClass();
            //            pnt.X = poly.IsoRing.Vertries[j].X;
            //            pnt.Y = poly.IsoRing.Vertries[j].Y;

            //            pntColl.AddPoint(pnt);
            //        }
            //        IRing outerRing = pntColl as IRing;
            //        geomCol.AddGeometry(outerRing);

            //        polygonBuffer.Shape = polygon;
            //        polygonBuffer.set_Value(indexLine, poly.Value);
            //        polygonBuffer.set_Value(indexType, poly.ID);

            //        polyCursor.InsertFeature(polygonBuffer);
            //    }
            //    polyCursor.Flush();
            //}

            //workspaceEdit.StopEditOperation();
            //workspaceEdit.StopEditing(true);
            //MessageBox.Show("OVER");
        }
    }
}
