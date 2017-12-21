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
using Hykj.MeteData;
using Hykj.PubMethods;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF.Connection.Local;
using ESRI.ArcGIS.ADF;
using Hykj.Isoline.Geom;

namespace DrawLineInArcGIS
{
    public partial class Form1 : Form
    {
        private List<Hykj.PubMethods.PointInfo> tempListPntInfo = new List<Hykj.PubMethods.PointInfo>();
        private Delauney delaumey;
        //private GridClass gridClass;

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

        private void GetPntShape(string type)
        {
            DataTable valueTable = null;
            DataTable sortTable = null;

            if (type == "Triangle")
            {
                tempListPntInfo.Clear();
                string url = "http://218.28.7.251:10525/hnqxjson/QxSqlInter/findDataSetOnDataType.hd?dataType=1-3-2&cityCode=HN";
                string logMessage;
                sortTable = MeteData.GetValueFromJson(url, out logMessage);
                //valueTable = MeteData.GetValueFromJson(url, out logMessage);

                if (!string.IsNullOrEmpty(logMessage))
                {
                    MessageBox.Show(logMessage);
                    return;
                }
            }
            else if (type == "Rectangle")
            {
                string filePath = @"E:\Book1.xlsx";
                valueTable = MeteData.GetValueFromExcel(filePath);
            }

            //if (valueTable == null)
            //{
            //    MessageBox.Show("数据读取失败！");
            //    return;
            //}

            //DataView dataView = valueTable.DefaultView;
            //dataView.Sort = "longitude";
            //DataTable sortTable = dataView.ToTable();

            try
            {
                for (int i = 0; i < sortTable.Rows.Count;i++ )
                {
                    DataRow dr = sortTable.Rows[i];
                    if (dr["longitude"] == DBNull.Value || dr["longitude"] == null)
                        continue;
                    if (dr["latitude"] == DBNull.Value || dr["latitude"] == null)
                        continue;
                    if (dr["eValue"] == DBNull.Value || dr["eValue"] == null)
                        continue;
                    string stationId = dr["stationID"].ToString();
                    System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^5.*");
                    if (!reg.IsMatch(stationId))
                        continue;
                    double x = double.Parse(dr["longitude"].ToString());
                    double y = double.Parse(dr["latitude"].ToString());
                    double eValue = double.Parse(dr["eValue"].ToString());
                    if (x < 110.35 || x > 116.65)
                        continue;
                    if (y < 31.383 || y > 36.37)
                        continue;
                    if (eValue < 5)
                        continue;

                    //double height = double.Parse(dr["height"].ToString());
                    //if (height < 60)
                    //    continue;

                    for (int rowIndexAdd = 1; i + rowIndexAdd < sortTable.Rows.Count; rowIndexAdd++)
                    {
                        DataRow compareRow = sortTable.Rows[i + rowIndexAdd];
                        if (compareRow["longitude"] == DBNull.Value || compareRow["longitude"] == null)
                            continue;
                        if (compareRow["latitude"] == DBNull.Value || compareRow["latitude"] == null)
                            continue;

                        double cX = double.Parse(compareRow["longitude"].ToString());
                        double cY = double.Parse(compareRow["latitude"].ToString());
                        if (x == cX && y == cY)
                        {
                            continue;
                        }
                        else
                        {
                            i = i + rowIndexAdd - 1;
                            break;
                        }
                    }

                    Hykj.PubMethods.PointInfo pntInfo = new Hykj.PubMethods.PointInfo(x, y, eValue);
                    tempListPntInfo.Add(pntInfo);
                }
                DateTime timeStart = DateTime.Now;

                //gridClass = new GridClass(tempListPntInfo);
                //gridClass.GetSuperRectangle();
                //gridClass.GetIsobands();
                delaumey = new Delauney(tempListPntInfo);
                delaumey.GetDelauney();
                delaumey.Isoline(new double[] { 18 });


                TimeSpan timeSpan = DateTime.Now - timeStart;
                EditMap();
                //MessageBox.Show(delaumey.ListTriangles.Count.ToString() + "     " + timeSpan.TotalSeconds.ToString());
                MessageBox.Show(timeSpan.TotalSeconds.ToString());
            }
            catch (Exception ex)
            {
            }
        }

        private void EditMap()
        {
            IMap map = this.axMapControl1.Map;
            ILayer polyLayer = map.get_Layer(2);
            IFeatureLayer featLayer = polyLayer as IFeatureLayer;
            IFeatureClass featClass = featLayer.FeatureClass;

            ILayer lineLayer = map.get_Layer(1);
            IFeatureLayer featLineLayer = lineLayer as IFeatureLayer;
            IFeatureClass featLineClass = featLineLayer.FeatureClass;

            ILayer pntLayer = map.get_Layer(0);
            IFeatureLayer pntFeatLayer = pntLayer as IFeatureLayer;
            IFeatureClass pntClass = pntFeatLayer.FeatureClass;

            IFeatureDataset featDataset = featClass.FeatureDataset;
            IWorkspace workspace = featDataset.Workspace;
            IWorkspaceEdit workspaceEdit = workspace as IWorkspaceEdit;
            workspaceEdit.StartEditing(false);
            workspaceEdit.StartEditOperation();

            //using (ComReleaser comReleaser = new ComReleaser())
            //{
            //    int index = pntClass.FindField("Value");
            //    IFeatureBuffer featBuffer = pntClass.CreateFeatureBuffer();
            //    comReleaser.ManageLifetime(featBuffer);

            //    IFeatureCursor insertCursor = pntClass.Insert(true);
            //    comReleaser.ManageLifetime(insertCursor);
            //    for (int i = 0; i < gridClass.PntGrid.Length; i++)
            //    {
            //        for (int j = 0; j < gridClass.PntGrid[i].Length; j++)
            //        {
            //            PointInfo pntInfo = gridClass.PntGrid[i][j];
            //            if (pntInfo == null)
            //                continue;
            //            IPoint pnt = new PointClass();
            //            pnt.X = pntInfo.X;
            //            pnt.Y = pntInfo.Y;

            //            featBuffer.Shape = pnt;
            //            featBuffer.set_Value(index, pntInfo.EValue);
            //            insertCursor.InsertFeature(featBuffer);
            //        }
            //    }
            //    insertCursor.Flush();
            //}

            //using (ComReleaser comReleaser = new ComReleaser())
            //{
            //    int index = featClass.FindField("VALUE");

            //    IFeatureBuffer featBuffer = featClass.CreateFeatureBuffer();
            //    comReleaser.ManageLifetime(featBuffer);

            //    IFeatureCursor insertCursor = featClass.Insert(true);
            //    comReleaser.ManageLifetime(insertCursor);
            //    foreach (PolygonInfo poly in gridClass.ListPolys)
            //    {
            //        IPointCollection pntColl = new PolygonClass();
            //        for (int i = 0; i < poly.ListPolyVertrix.Count; i++)
            //        {
            //            IPoint pnt = new PointClass();
            //            pnt.X = poly.ListPolyVertrix[i].X;
            //            pnt.Y = poly.ListPolyVertrix[i].Y;
            //            pntColl.AddPoint(pnt);
            //        }
            //        IPoint pntStart = new PointClass();
            //        pntStart.X = poly.ListPolyVertrix[0].X;
            //        pntStart.Y = poly.ListPolyVertrix[0].Y;
            //        pntColl.AddPoint(pntStart);

            //        IPolygon polygon = pntColl as IPolygon;

            //        featBuffer.Shape = polygon;
            //        featBuffer.set_Value(index, poly.ColorRange);
            //        insertCursor.InsertFeature(featBuffer);
            //    }
            //    insertCursor.Flush();
            //}

            using (ComReleaser comReleaser = new ComReleaser())
            {
                int index = pntClass.FindField("Value");
                IFeatureBuffer featBuffer = pntClass.CreateFeatureBuffer();
                comReleaser.ManageLifetime(featBuffer);

                IFeatureCursor insertCursor = pntClass.Insert(true);
                comReleaser.ManageLifetime(insertCursor);
                foreach (Hykj.PubMethods.PointInfo pntInfo in tempListPntInfo)
                {
                    IPoint pnt = new PointClass();
                    pnt.X = pntInfo.X;
                    pnt.Y = pntInfo.Y;

                    featBuffer.Shape = pnt;
                    featBuffer.set_Value(index, pntInfo.EValue);
                    insertCursor.InsertFeature(featBuffer);
                }
                insertCursor.Flush();
            }

            using (ComReleaser comReleaser = new ComReleaser())
            {
                int index = featLineClass.FindField("Value");
                IFeatureBuffer featBuffer = featLineClass.CreateFeatureBuffer();
                comReleaser.ManageLifetime(featBuffer);

                IFeatureCursor insertCursor = featLineClass.Insert(true);
                comReleaser.ManageLifetime(insertCursor);
                foreach (IsolineClass isoLine in delaumey.ListIsoLines)
                {
                    IPointCollection pntColl = new PolylineClass();
                    for (int i = 0; i < isoLine.pntList.Length; i++)
                    {
                        IPoint pnt = new PointClass();
                        pnt.X = isoLine.pntList[i].X;
                        pnt.Y = isoLine.pntList[i].Y;

                        pntColl.AddPoint(pnt);
                    }

                    IPolyline line = pntColl as IPolyline;

                    featBuffer.Shape = line;
                    featBuffer.set_Value(index, isoLine.IsolineValue);
                    insertCursor.InsertFeature(featBuffer);
                }
                insertCursor.Flush();
            }

            using (ComReleaser comReleaser = new ComReleaser())
            {
                IFeatureBuffer featBuffer = featClass.CreateFeatureBuffer();
                comReleaser.ManageLifetime(featBuffer);

                IFeatureCursor insertCursor = featClass.Insert(true);
                comReleaser.ManageLifetime(insertCursor);
                foreach (TriangleInfo triangle in delaumey.ListTriangles)
                {
                    IPointCollection pntColl = new PolygonClass();
                    IPoint pntA = new PointClass();
                    pntA.X = triangle.VertexA.X;
                    pntA.Y = triangle.VertexA.Y;
                    pntColl.AddPoint(pntA);

                    IPoint pntB = new PointClass();
                    pntB.X = triangle.VertexB.X;
                    pntB.Y = triangle.VertexB.Y;
                    pntColl.AddPoint(pntB);

                    IPoint pntC = new PointClass();
                    pntC.X = triangle.VertexC.X;
                    pntC.Y = triangle.VertexC.Y;
                    pntColl.AddPoint(pntC);
                    pntColl.AddPoint(pntA);

                    IPolygon polygon = pntColl as IPolygon;

                    featBuffer.Shape = polygon;
                    insertCursor.InsertFeature(featBuffer);
                }
                insertCursor.Flush();
            }

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);
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

        private void button1_Click(object sender, EventArgs e)
        {
            GetPntShape("Triangle");
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

            List<IsoLineInfo> listLines = TestIsoline.ReadJsonFile();

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
                        pnt.X = lines.ListVertrix[j].PntCoord.X;
                        pnt.Y = lines.ListVertrix[j].PntCoord.Y;

                        pntColl.AddPoint(pnt);
                    }

                    IPolyline line = pntColl as IPolyline;

                    lineBuffer.Shape = line;
                    lineBuffer.set_Value(indexLine, lines.LineValue);
                    lineCursor.InsertFeature(lineBuffer);

                    if (!lines.FinishState)
                    {
                        Hykj.Isoline.Geom.PointInfo pntFrom = lines.GetLineFrom();
                        Hykj.Isoline.Geom.PointInfo pntEnd = lines.GetLineEnd();

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
            //return sunCount;
            MessageBox.Show(sunCount.ToString());
        }

        private void EditMapPoint()
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
            workspaceEdit.StartEditing(false);
            workspaceEdit.StartEditOperation();

            using (ComReleaser comReleaser = new ComReleaser())
            {
                int index = pntClass.FindField("Value");
                IFeatureBuffer featBuffer = pntClass.CreateFeatureBuffer();
                comReleaser.ManageLifetime(featBuffer);

                IFeatureCursor insertCursor = pntClass.Insert(true);
                comReleaser.ManageLifetime(insertCursor);
                foreach (Hykj.PubMethods.PointInfo pntInfo in tempListPntInfo)
                {
                    IPoint pnt = new PointClass();
                    pnt.X = pntInfo.X;
                    pnt.Y = pntInfo.Y;

                    featBuffer.Shape = pnt;
                    featBuffer.set_Value(index, pntInfo.EValue);
                    insertCursor.InsertFeature(featBuffer);
                }
                insertCursor.Flush();
            }

            using (ComReleaser comReleaser = new ComReleaser())
            {
                int index = featLineClass.FindField("Value");
                IFeatureBuffer featBuffer = featLineClass.CreateFeatureBuffer();
                comReleaser.ManageLifetime(featBuffer);

                IFeatureCursor insertCursor = featLineClass.Insert(true);
                comReleaser.ManageLifetime(insertCursor);
                foreach (IsolineClass isoLine in delaumey.ListIsoLines)
                {
                    IPointCollection pntColl = new PolylineClass();
                    for (int i = 0; i < isoLine.pntList.Length; i++)
                    {
                        IPoint pnt = new PointClass();
                        pnt.X = isoLine.pntList[i].X;
                        pnt.Y = isoLine.pntList[i].Y;

                        pntColl.AddPoint(pnt);
                    }

                    IPolyline line = pntColl as IPolyline;

                    featBuffer.Shape = line;
                    featBuffer.set_Value(index, isoLine.IsolineValue);
                    insertCursor.InsertFeature(featBuffer);
                }
                insertCursor.Flush();
            }

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);
        }
    }
}
