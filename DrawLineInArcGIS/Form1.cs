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
using Hykj.Isoline.Geom;
using Hykj.Isoline.Isobands;

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


            //IFeatureCursor pFeatureCursor = featClass.Update(pQueryFilter, false);
            //IFeature pFeature = pFeatureCursor.NextFeature();
            //while (pFeature != null)
            //{
            //    pFeatureCursor.DeleteFeature();
            //    pFeature = pFeatureCursor.NextFeature();
            //}
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(pQueryFilter);

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
            List<IsoLineInfo> listLines = gridIsoline.ListIsolines;
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
                        Hykj.Isoline.Geom.PointInfo pntFrom = lines.FromPoint;//.GetLineFrom();
                        Hykj.Isoline.Geom.PointInfo pntEnd = lines.ToPoint;//.GetLineEnd();

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
    }
}
