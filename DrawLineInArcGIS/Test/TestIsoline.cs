using Hykj.BaseMethods;
using Hykj.GISModule;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Hykj.GISModule.Isobands;
using Hykj.EagleClass;
using System.Data.OleDb;

namespace DrawLineInArcGIS
{
    /// <summary>
    /// 
    /// </summary>
    public class TestIsoline
    {
        public static GridIsoline ReadJsonFile()
        {
            string fieldName = "pm25";
            List<PointInfo> listPntInfo = new List<PointInfo>();
            string jsonValue = File.ReadAllText(@"E:\工作内容\Q气象局项目\等值线程序\DrawLineInArcGIS\DrawLineInArcGIS\bin\Debug\Data\test.json");
            if (!string.IsNullOrEmpty(jsonValue))
            {
                DataTable student4 = JsonHelper.DeserializeJsonToObject<DataTable>(jsonValue);

                foreach (DataRow dataRow in student4.Rows)
                {
                    double x = double.Parse(dataRow["longitude"].ToString());
                    double y = double.Parse(dataRow["latitude"].ToString());
                    double z = double.Parse(dataRow[fieldName].ToString());
                    PointInfo pntInfo = new PointInfo(x, y, z);
                    listPntInfo.Add(pntInfo);
                }
            }

            GridClass gridClass = new Hykj.GISModule.GridClass(listPntInfo);
            gridClass.GetGrid();
            GridIsoline gridIsoline = new GridIsoline(gridClass);
            double[] lineValue = new double[]{5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 105};
            gridIsoline.WikiIsoLineToBands(lineValue);

            return gridIsoline;
            
        }

        private DataTable GetValueFromExcel(string filePath)
        {
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + filePath + ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1';";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = "select * from [sheet2$]";
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds, "table1");
            return ds.Tables[0];
        }

        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static DataTable OpenCSV(string fileName)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;

            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                aryLine = strLine.Split(',');
                if (IsFirst == true)
                {
                    IsFirst = false;
                    columnCount = aryLine.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(aryLine[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }

            sr.Close();
            fs.Close();
            return dt;
        }

        public static void ReadJsonFile1()
        {
            //string jsonValue = File.ReadAllText(@"E:\工作内容\Q气象局项目\等值线程序\DrawLineInArcGIS\DrawLineInArcGIS\bin\Debug\Data\test.json");
            //if (!string.IsNullOrEmpty(jsonValue))
            //{
                //DataTable student4 = JsonHelper.DeserializeJsonToObject<DataTable>(jsonValue);
            DataTable student4 = OpenCSV(@"E:\01Project Manager\02环保项目\06数据\20180116测试数据\鸿鹄板卡\程序测试最终版\Part1\AirPart1.csv");

                EagleCls eagle = new EagleCls(student4);
                string isoLinesStr = null;
                //string isoBandsStr = eagle.calculator_PM25(out isoLinesStr);
                string isoBandsStr = eagle.calculator_NO2(out isoLinesStr);
                //using (StreamWriter sw = new StreamWriter(@"C:\Users\admin\Desktop\test\isoLines.txt", false, Encoding.Default))
                //{
                //    sw.Write(isoLinesStr);
                //} 
                using (StreamWriter sw = new StreamWriter(@"C:\Users\admin\Desktop\test\isoBands.txt", false, Encoding.Default))
                {
                    sw.Write(isoBandsStr);
                }
            //}
        }
    }
}
