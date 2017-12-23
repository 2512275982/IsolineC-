using Hykj.Isoline.Geom;
using System;
using System.Collections.Generic;

namespace Hykj.Isoline.Isobands
{
    /// <summary>
    /// 
    /// </summary>
    public class GridIsoline
    {
        private List<IsoLineInfo> tempIsolines = new List<IsoLineInfo>();
        private List<IsoLineInfo> listIsolines = new List<IsoLineInfo>();

        public List<IsoLineInfo> ListIsolines
        {
            get { return listIsolines; }
        }
        private List<IsoPolygonInfo> isoBands;

        public List<IsoPolygonInfo> IsoBands
        {
            get { return isoBands; }
        }
        private PointInfo[,] pntGrid;

        public GridIsoline(PointInfo[,] gridPnts)
        {
            this.pntGrid = gridPnts;
        }

        public List<IsoLineInfo> WikiIsoline(double[] listContourValues)
        {
            listIsolines.Clear();
            for (int i = 0; i < listContourValues.Length; i++)
            {
                GetIsolines(listContourValues[i]);
                MergeIsolines();

                for (int j = 0; j < tempIsolines.Count; j++)
                {
                    IsoLineInfo tempLine = tempIsolines[j];
                    tempLine.Label = GetLabelInfo(tempLine);
                    //tempLine.ListVertrix = BsLine(tempLine.ListVertrix, 10);
                    listIsolines.Add(tempLine);
                }
                tempIsolines.Clear();
            }
            return listIsolines;
        }

        public List<IsoPolygonInfo> WikiIsolineBand(List<IsoLineInfo> isolines,GridCoord superGrid){
            List<IsoRingInfo> rings = GetIsoRings(isolines, superGrid);
			isoBands = GetIsoBands(rings);
			return isoBands;
        }

        /*
		 * Step1
		 * 将等值线转换成简单的面（IsoRing），并对等值线进行分类和排序（由大到小）
		 * 该步骤为生成等值面的第一步
		 * yMax,yMin,xMax,xMin：grid矩形范围值
		 * listIsolines：该类的全局变量
		 * 返回值：listIsoRings，排序后的IsoRingInfo列表
		 * edit by maxiaoling at 2017.12.14
		 */
        private List<IsoRingInfo> GetIsoRings(List<IsoLineInfo> isolines,GridCoord superGrid)
        {
            List<IsoRingInfo> listClass1 = new List<IsoRingInfo>();
			List<IsoRingInfo> listClass2 = new List<IsoRingInfo>();
			List<IsoRingInfo> listClass3 = new List<IsoRingInfo>();
			List<IsoRingInfo> listClass4 = new List<IsoRingInfo>();
			List<IsoRingInfo> listClass5 = new List<IsoRingInfo>();
			List<IsoRingInfo> listClass6 = new List<IsoRingInfo>();
			List<IsoRingInfo> listClass7 = new List<IsoRingInfo>();
			List<IsoRingInfo> listClass8 = new List<IsoRingInfo>();
			List<IsoRingInfo> listClass9 = new List<IsoRingInfo>();
			List<IsoRingInfo> listClass10 = new List<IsoRingInfo>();
			List<IsoRingInfo> listClass11 = new List<IsoRingInfo>();

            double yMax = superGrid.yMax;
            double yMin = superGrid.yMin;
            double xMax = superGrid.xMax;
            double xMin = superGrid.xMin;
			
			IsoRing isoRing = null;
            IsoRingInfo isoRingInfo = null;
			string ringId = string.Empty;
            bool needAdd = false;

            IsoRingInfo ringCompare = null;
			for(int i=0;i<isolines.Count;i++){
			    IsoLineInfo line = isolines[i];
				needAdd = true;
				if(line.LineType){  //开放型
					PointInfo pntFrom = line.FromPoint;//.GetLineFrom();
					PointInfo pntEnd = line.ToPoint;//.GetLineEnd();
					string type1 = string.Empty,type2 = string.Empty;

                    if (Math.Abs(pntFrom.PntCoord.X - xMin) < 0.0000001)
                    {
						type1 = "1";
					}
                    else if (Math.Abs(pntFrom.PntCoord.X - xMax) < 0.0000001)
                    {
						type1 = "3";
					}
                    else if (Math.Abs(pntFrom.PntCoord.Y - yMin) < 0.0000001)
                    {
						type1 = "4";
					}
                    else if (Math.Abs(pntFrom.PntCoord.Y - yMax) < 0.0000001)
                    {
						type1 = "2";
					}
                    if (Math.Abs(pntEnd.PntCoord.X - xMin) < 0.0000001)
                    {
						type2 = "1";
					}
                    else if (Math.Abs(pntEnd.PntCoord.X - xMax) < 0.0000001)
                    {
						type2 = "3";
					}
                    else if (Math.Abs(pntEnd.PntCoord.Y - yMin) < 0.0000001)
                    {
						type2 = "4";
					}
                    else if (Math.Abs(pntEnd.PntCoord.Y - yMax) < 0.0000001)
                    {
						type2 = "2";
					}
					string type = type1 + type2;
					
					int j;  //JavaScript不存在块作用域，所以此处声明和在for语句外声明效果一致
					switch(type){
						case "33":   //第2类
							ringId = "02" + listClass2.Count.ToString();
							isoRing = new IsoRing(line.ListVertrix);
							isoRingInfo = new IsoRingInfo(ringId,isoRing,line.LineValue);
							
							//以线的起始点判断是否包含关系，替换为以下判断是否包含的方法，更好理解
							for(j = 0;j<listClass2.Count; j++){
								ringCompare = listClass2[j];
								if(isoRing.JudgePntInRing(ringCompare.IsoRing.Vertries[1])){ //将大的放在前面
									listClass2.Insert(j,isoRingInfo);//.splice(j,0,isoRingInfo);  
									needAdd = false;
									break;
								}
							}
							if(needAdd){
								listClass2.Add(isoRingInfo);
							}
							break;
						case "11":  //第3类
							ringId = "03" + listClass3.Count.ToString();
							isoRing = new IsoRing(line.ListVertrix);
							isoRingInfo = new IsoRingInfo(ringId,isoRing,line.LineValue);
							
							for(j = 0;j<listClass3.Count;j++){
								ringCompare = listClass3[j];
								if(isoRing.JudgePntInRing(ringCompare.IsoRing.Vertries[1])){
									listClass3.Insert(j,isoRingInfo);//.splice(j,0,isoRingInfo);
									needAdd = false;
									break;
								}
							}
							if(needAdd){
								listClass3.Add(isoRingInfo);
							}
							break;
						case "44": //第4类
							ringId = "04" + listClass4.Count.ToString();
							isoRing = new IsoRing(line.ListVertrix);
							isoRingInfo = new IsoRingInfo(ringId,isoRing,line.LineValue);
						
							for(j = 0;j<listClass4.Count;j++){
								ringCompare = listClass4[j];
								if(isoRing.JudgePntInRing(ringCompare.IsoRing.Vertries[1])){
									listClass4.Insert(j,isoRingInfo);//.splice(j,0,isoRingInfo);
									needAdd = false;
									break;
								}
							}
							if(needAdd){
								listClass4.Add(isoRingInfo);
							}
							break;
						case "22":  //第5类
							ringId = "05" + listClass5.Count.ToString();
							isoRing = new IsoRing(line.ListVertrix);
							isoRingInfo = new IsoRingInfo(ringId,isoRing,line.LineValue);
						
							for(j = 0;j<listClass5.Count;j++){
								ringCompare = listClass5[j];
								if(isoRing.JudgePntInRing(ringCompare.IsoRing.Vertries[1])){
									listClass5.Insert(j,isoRingInfo);//.splice(j,0,isoRingInfo);
									needAdd = false;
									break;
								}
							}
							if(needAdd){
								listClass5.Add(isoRingInfo);
							}
							break;
						case "12":  //第6类
						case "21":
							ringId = "06" + listClass6.Count.ToString();
							isoRing = new IsoRing(line.ListVertrix);
							isoRing.PushPoint(new PointCoord(xMin,yMax));  //第6类需要加上一个角点（左上角）
							isoRingInfo = new IsoRingInfo(ringId,isoRing,line.LineValue);
							
							for(j = 0;j<listClass6.Count;j++){
								ringCompare = listClass6[j];
								if(isoRing.JudgePntInRing(ringCompare.IsoRing.Vertries[1])){
									listClass6.Insert(j,isoRingInfo);//.splice(j,0,isoRingInfo);
									needAdd = false;
									break;
								}
							}
							if(needAdd){
								listClass6.Add(isoRingInfo);
							}
							break;
						case "14":  //第7类
						case "41":
							ringId = "07" + listClass7.Count.ToString();
							isoRing = new IsoRing(line.ListVertrix);
							isoRing.PushPoint(new PointCoord(xMin,yMin));   //第7类需要加上一个角点（左下角）
							isoRingInfo = new IsoRingInfo(ringId,isoRing,line.LineValue);
							
							for(j = 0;j<listClass7.Count;j++){
								ringCompare = listClass7[j];
								if(isoRing.JudgePntInRing(ringCompare.IsoRing.Vertries[1])){
									listClass7.Insert(j,isoRingInfo);//.splice(j,0,isoRingInfo);
									needAdd = false;
									break;
								}
							}
							if(needAdd){
								listClass7.Add(isoRingInfo);
							}
							break;
						case "34":  //第8类
						case "43":
							ringId = "08" + listClass8.Count.ToString();
							isoRing = new IsoRing(line.ListVertrix);
							isoRing.PushPoint(new PointCoord(xMax,yMin));   //第8类需要加上一个角点（右下角）
							isoRingInfo = new IsoRingInfo(ringId,isoRing,line.LineValue);
							
							for(j = 0;j<listClass8.Count;j++){
								ringCompare = listClass8[j];
								if(isoRing.JudgePntInRing(ringCompare.IsoRing.Vertries[1])){
									listClass8.Insert(j,isoRingInfo);//.splice(j,0,isoRingInfo);
									needAdd = false;
									break;
								}
							}
							if(needAdd){
								listClass8.Add(isoRingInfo);
							}
							break;
						case "23":   //第9类
						case "32":
							ringId = "09" + listClass9.Count.ToString();
							isoRing = new IsoRing(line.ListVertrix);
							isoRing.PushPoint(new PointCoord(xMax,yMax));   //第9类需要加上一个角点（右上角）
							isoRingInfo = new IsoRingInfo(ringId,isoRing,line.LineValue);
							
							for(j = 0;j<listClass9.Count;j++){
								ringCompare = listClass9[j];
								if(isoRing.JudgePntInRing(ringCompare.IsoRing.Vertries[1])){
									listClass9.Insert(j,isoRingInfo);//.splice(j,0,isoRingInfo);
									needAdd = false;
									break;
								}
							}
							if(needAdd){
								listClass9.Add(isoRingInfo);
							}
							break;
						case "13":  //第10类
						case "31":
							ringId = "10" + listClass10.Count.ToString();
							isoRing = new IsoRing(line.ListVertrix);
                            if (Math.Abs(line.ToPoint.PntCoord.Y - xMin) < 0.000001)
                            {  //第10类，差两个点，需要考虑添加的顺序
								isoRing.PushPoint(new PointCoord(xMin,yMin));
								isoRing.PushPoint(new PointCoord(xMax,yMin));
							}
							else{
								isoRing.PushPoint(new PointCoord(xMax,yMin));
								isoRing.PushPoint(new PointCoord(xMin,yMin));
							}
							isoRingInfo = new IsoRingInfo(ringId,isoRing,line.LineValue);
							
							for(j = 0;j<listClass10.Count;j++){
								ringCompare = listClass10[j];
								if(isoRing.JudgePntInRing(ringCompare.IsoRing.Vertries[1])){
									listClass10.Insert(j,isoRingInfo);//.splice(j,0,isoRingInfo);
									needAdd = false;
									break;
								}
							}
							if(needAdd){
								listClass10.Add(isoRingInfo);
							}
							break;
						case "24":  //第11类
						case "42":
							ringId = "11" + listClass11.Count.ToString();
							isoRing = new IsoRing(line.ListVertrix);
                            if (Math.Abs(line.ToPoint.PntCoord.Y - yMin) < 0.000001)
                            {  //第11类，差两个点，需要考虑添加的顺序   GetLineEnd()
								isoRing.PushPoint(new PointCoord(xMin,yMin));
								isoRing.PushPoint(new PointCoord(xMin,yMax));
							}
							else{
								isoRing.PushPoint(new PointCoord(xMin,yMax));
								isoRing.PushPoint(new PointCoord(xMin,yMin));
							}
							isoRingInfo = new IsoRingInfo(ringId,isoRing,line.LineValue);
							
							for(j = 0;j<listClass11.Count;j++){
								ringCompare = listClass11[j];
								if(isoRing.JudgePntInRing(ringCompare.IsoRing.Vertries[1])){
									listClass11.Insert(j,isoRingInfo);//.splice(j,0,isoRingInfo);
									needAdd = false;
									break;
								}
							}
							if(needAdd){
								listClass11.Add(isoRingInfo);
							}
							break;
					}
				}
				else{   //闭合型，反向遍历，第1类
					ringId = "01" + listClass1.Count.ToString();
					isoRing = new IsoRing(line.ListVertrix);
					isoRingInfo = new IsoRingInfo(ringId,isoRing,line.LineValue);
					
					for(int j = 0; j<listClass1.Count; j++){
						ringCompare = listClass1[j];
						if(isoRing.JudgePntInRing(ringCompare.IsoRing.Vertries[0])){
							listClass1.Insert(j,isoRingInfo);//.splice(j,0,isoRingInfo);
							needAdd = false;
							break;
						}
					}
					if(needAdd){
						listClass1.Add(isoRingInfo);
					}
				}
				ringCompare = null;
			}
			
			ringId = "0000";  //添加最外圈矩形
            List<PointCoord> outerEnvelop = new List<PointCoord>();
            outerEnvelop.Add(new PointCoord(xMax,yMax));
            outerEnvelop.Add(new PointCoord(xMax,yMin));
            outerEnvelop.Add(new PointCoord(xMin,yMin));
            outerEnvelop.Add(new PointCoord(xMin,yMax));
			isoRing = new IsoRing(outerEnvelop);
			isoRingInfo = new IsoRingInfo(ringId,isoRing);
			
			List<IsoRingInfo> listIsoRings = new List<IsoRingInfo>();
            listIsoRings.Add(isoRingInfo);
            listIsoRings.AddRange(listClass11);
            listClass11.Clear();
            listIsoRings.AddRange(listClass10);
            listClass10.Clear();
            listIsoRings.AddRange(listClass9);
            listClass9.Clear();
            listIsoRings.AddRange(listClass8);
            listClass8.Clear();
            listIsoRings.AddRange(listClass7);
            listClass7.Clear();
            listIsoRings.AddRange(listClass6);
            listClass6.Clear();
            listIsoRings.AddRange(listClass5);
            listClass5.Clear();
            listIsoRings.AddRange(listClass4);
            listClass4.Clear();
            listIsoRings.AddRange(listClass3);
            listClass3.Clear();
            listIsoRings.AddRange(listClass2);
            listClass2.Clear();
            listIsoRings.AddRange(listClass1);
            listClass1.Clear();

			listClass1 = null;  //释放内存的操作
			listClass10 = null;
			listClass11 = null;
            listClass2 = null;
            listClass3 = null;
            listClass4 = null;
			listClass5 = null;
            listClass6 = null;
            listClass7 = null;
            listClass8 = null;
			listClass9 = null;
			
			return listIsoRings;
        }

        private List<IsoPolygonInfo> GetIsoBands(List<IsoRingInfo> listIsoRings)
        {
            List<IsoPolygonInfo> listIsoPolys = new List<IsoPolygonInfo>();
			IsoPolygonInfo isoPolygon;
			bool needAdd = false;
			for(int i = 0;i<listIsoRings.Count;i++){  //循环遍历每一个多边形，找到直接子多边形
				double ringValue = listIsoRings[i].Value;
				isoPolygon = new IsoPolygonInfo(listIsoRings[i].IsoRing);
				for(int index = i+1;index<listIsoRings.Count;index++){
					PointCoord pnt = listIsoRings[index].IsoRing.Vertries[1];  //不用第一个点，因为第一个点可能在边界上，比较特殊
					if(listIsoRings[i].IsoRing.JudgePntInRing(pnt)){ //判断多边形是否是目标多边形的子多边形
						needAdd = true;
						for(int j = 0;j < isoPolygon.InterRings.Count;j++){
							if(isoPolygon.InterRings[j].JudgePntInRing(pnt)){
								needAdd = false;
								break;
							}
						}
						if(needAdd){
							isoPolygon.AddInterRing(listIsoRings[index].IsoRing);
							if(listIsoRings[i].ValueFlag)
							{
								listIsoRings[index].SetParentValue(ringValue);
								if(ringValue > listIsoRings[index].Value)
								{
									isoPolygon.MaxValue = ringValue;
								}
								else if(ringValue < listIsoRings[index].Value){
									isoPolygon.MinValue = ringValue;
								}
							}
							else{
								ringValue = listIsoRings[index].Value;
							}
						}
					}
				}
				if(isoPolygon.InterRings.Count == 0){
					if(ringValue>listIsoRings[i].ParentValue){
						isoPolygon.MinValue = ringValue;
					}else{
						isoPolygon.MaxValue = ringValue;
					}
					
				}
				listIsoPolys.Add(isoPolygon);
			}
			return listIsoPolys;
        }
        /*
		 * 将组成线的点转换成面的点数组，后面将点的数组换掉
		 * 2017.12.13，遗留工作标记
		 */
		private List<PointCoord> TransPntArrayToCoors(List<PointInfo> pntArray){
			List<PointCoord> coords = new List<PointCoord>();
            for (int i = 0; i < pntArray.Count; i++)
            {
                coords.Add(pntArray[i].PntCoord);
            }
			return coords;
		}

        /*
		 * 判断值格网点值与目标值的关系，小于最小值返回0，大于等于返回1
		 */
		private int GetTypeValue(double zValue, double lineValue) {
			int type = -1;
			if(zValue < lineValue) {
				type = 0;
			} else if(zValue >= lineValue) {
				type = 1;
			} 
			return type;
		}

        /*
		 * 将追踪的短线段添加到等值线列表中
		 * 输入参数lineFromPnt和lineToPnt，分别标识短线段的起始点
		 */
		private void UpdateIsolines(PointInfo lineFromPnt,PointInfo lineToPnt,double value){
			//当两个点都是边界点时，该等值线由这两个点组成
			if(lineFromPnt.IsEdge && lineToPnt.IsEdge){
				IsoLineInfo isoline = new IsoLineInfo(value);
				isoline.AddStartPoint(lineFromPnt);
				isoline.AddEndPoint(lineToPnt);
                //isoline.LineType = true;  //开放型等值线
				isoline.FinishState = true;  
				tempIsolines.Add(isoline);
			}
			else{
				bool matchFlag = false;
				if(tempIsolines.Count > 0){   //当等值线数量为空时
					for(int i = 0;i<tempIsolines.Count - 1;i++){  //遍历所有的等值线
						IsoLineInfo isoline = tempIsolines[i];
						if(isoline.FinishState)  //如果等值线追踪完成
							continue;
						matchFlag = false;
						PointInfo pntStart = isoline.FromPoint; //.GetLineFrom();
						PointInfo pntEnd = isoline.ToPoint; //.GetLineEnd();
						if(pntStart.IsEdge){
							if(lineFromPnt.IsEdge){
								matchFlag = pntEnd.Equals(lineToPnt);
								if(matchFlag){
									isoline.AddEndPoint(lineFromPnt);
									isoline.FinishState = true;
								}
							}
							else if(lineToPnt.IsEdge){
								matchFlag = pntEnd.Equals(lineFromPnt);
								if(matchFlag){
									isoline.AddEndPoint(lineToPnt);
									isoline.FinishState = true;
								}
							}
							else{
								matchFlag = pntEnd.Equals(lineToPnt);
								if(matchFlag){
									isoline.AddEndPoint(lineFromPnt);
								}
								else{
									matchFlag = pntEnd.Equals(lineFromPnt);
									if(matchFlag){
										isoline.AddEndPoint(lineToPnt);
									}
								}
							}
						}
						else if(pntEnd.IsEdge){   //在等值线的起始点插入值
							if(lineFromPnt.IsEdge){
								matchFlag = pntStart.Equals(lineToPnt);
								if(matchFlag){
									isoline.AddStartPoint(lineToPnt);
                                    isoline.AddStartPoint(lineFromPnt);
									isoline.FinishState = true;
								}
							}
							else if(lineToPnt.IsEdge){
								matchFlag = pntStart.Equals(lineFromPnt);
								if(matchFlag){
                                    isoline.AddStartPoint(lineFromPnt);
                                    isoline.AddStartPoint(lineToPnt);
									isoline.FinishState = true;
								}
							}
							else{
								matchFlag = pntStart.Equals(lineToPnt);
								if(matchFlag){
                                    isoline.AddStartPoint(lineFromPnt);
								}
								else{
									matchFlag = pntStart.Equals(lineFromPnt);
									if(matchFlag){
                                        isoline.AddStartPoint(lineToPnt);
									}
								}
							}
						}
						else{
							if(lineFromPnt.IsEdge){
								matchFlag = pntStart.Equals(lineToPnt);
								if(matchFlag){
                                    isoline.AddStartPoint(lineFromPnt);
                                    //isoline.LineType = true;
								}
								else{
									matchFlag = pntEnd.Equals(lineToPnt);
									if(matchFlag){
										isoline.AddEndPoint(lineFromPnt);
                                        //isoline.LineType = true;
									}
								}
							}
							else if(lineToPnt.IsEdge){
								matchFlag = pntStart.Equals(lineFromPnt);
								if(matchFlag){
                                    isoline.AddStartPoint(lineToPnt);
                                    //isoline.LineType = true;
								}
								else{
									matchFlag = pntEnd.Equals(lineFromPnt);
									if(matchFlag){
										isoline.AddEndPoint(lineToPnt);
                                        //isoline.LineType = true;
									}
								}
							}
							else{
								matchFlag = true;
								if(pntStart.Equals(lineFromPnt)&&pntEnd.Equals(lineToPnt)){
									isoline.AddEndPoint(lineFromPnt);
									isoline.FinishState = true;
								}
								else if(pntStart.Equals(lineToPnt)&&pntEnd.Equals(lineFromPnt)){
									isoline.AddEndPoint(lineToPnt);
									isoline.FinishState = true;
								}
								else if(pntStart.Equals(lineFromPnt)){
                                    isoline.AddStartPoint(lineToPnt);
								}
								else if(pntStart.Equals(lineToPnt)){
                                    isoline.AddStartPoint(lineFromPnt);
								}
								else if(pntEnd.Equals(lineFromPnt)){
									isoline.AddEndPoint(lineToPnt);
								}
								else if(pntEnd.Equals(lineToPnt)){
									isoline.AddEndPoint(lineFromPnt);
								}
								else{
									matchFlag = false;
								}
							}
						}
						if(matchFlag)  //如果找到匹配的等值线，则跳出循环
							break;
					}
				}
				if(!matchFlag){    //如果没有找到匹配的等值线，则添加一条新的等值线
                    IsoLineInfo isoline = new IsoLineInfo(value);
                    isoline.AddStartPoint(lineFromPnt);
					isoline.AddEndPoint(lineToPnt);
					
                    //if(lineFromPnt.IsEdge || lineToPnt.IsEdge){
                    //    isoline.LineType = true;  //开放型等值线
                    //}
					tempIsolines.Add(isoline);
				}
			}
		}

        /*
		 * 获取等值线的标注信息，包括位置，角度以及值
		 */
		private LabelInfo GetLabelInfo(IsoLineInfo isoline)
        {
			double angle = 0,dis;
			double maxDis = 0;
			List<PointCoord> linePnts = isoline.ListVertrix;
			PointCoord pnt1,pnt2;
            PointCoord pntLabel = new PointCoord();
            for (int i = 0; i < linePnts.Count - 1; i++)
            {
                pnt1 = linePnts[i];
                pnt2 = linePnts[i + 1];
                dis = Math.Sqrt((pnt1.X - pnt2.X) * (pnt1.X - pnt2.X) + (pnt1.Y - pnt2.Y) * (pnt1.Y - pnt2.Y));
                if (dis > maxDis)
                {
                    pntLabel.X = (pnt1.X + pnt2.X) / 2;
                    pntLabel.Y = (pnt1.Y + pnt2.Y) / 2;
                    angle = (pnt2.Y - pnt1.Y) / (pnt2.X - pnt1.X);
                }
            }
			return new LabelInfo(pntLabel,angle,isoline.LineValue);
		}

        private List<PointInfo> BsLine(List<PointInfo> pnts, int clipCount = 15)
        {
            try
            {
                List<PointInfo> listOutputPnts = new List<PointInfo>();

                double x0 = 2.0 * pnts[0].PntCoord.X - pnts[1].PntCoord.X;
                double y0 = 2.0 * pnts[0].PntCoord.Y - pnts[1].PntCoord.Y;
                PointInfo pnt0 = new PointInfo(x0, y0);

                int count = pnts.Count;
                double xn = 2.0 * pnts[count - 1].PntCoord.X - pnts[count - 2].PntCoord.X;
                double yn = 2.0 * pnts[count - 1].PntCoord.Y - pnts[count - 2].PntCoord.Y;
                PointInfo pntn = new PointInfo(xn, yn);

                pnts.Insert(0, pnt0);
                pnts.Add(pntn);

                double A0, A1, A2, A3;
                double B0, B1, B2, B3;

                double dt = 1.0 / clipCount;
                for (int i = 0; i < pnts.Count - 3; i++)
                {
                    A0 = (pnts[i].PntCoord.X + 4.0 * pnts[i + 1].PntCoord.X + pnts[i + 2].PntCoord.X) / 6.0;
                    A1 = -(pnts[i].PntCoord.X - pnts[i + 2].PntCoord.X) / 2.0;
                    A2 = (pnts[i].PntCoord.X - 2.0 * pnts[i + 1].PntCoord.X + pnts[i + 2].PntCoord.X) / 2.0;
                    A3 = -(pnts[i].PntCoord.X - 3.0 * pnts[i + 1].PntCoord.X + 3.0 * pnts[i + 2].PntCoord.X - pnts[i + 3].PntCoord.X) / 6.0;
                    B0 = (pnts[i].PntCoord.Y + 4.0 * pnts[i + 1].PntCoord.Y + pnts[i + 2].PntCoord.Y) / 6.0;
                    B1 = -(pnts[i].PntCoord.Y - pnts[i + 2].PntCoord.Y) / 2.0;
                    B2 = (pnts[i].PntCoord.Y - 2.0 * pnts[i + 1].PntCoord.Y + pnts[i + 2].PntCoord.Y) / 2.0;
                    B3 = -(pnts[i].PntCoord.Y - 3.0 * pnts[i + 1].PntCoord.Y + 3.0 * pnts[i + 2].PntCoord.Y - pnts[i + 3].PntCoord.Y) / 6.0;

                    double t1, t2, t3 = 0;
                    for (int j = 0; j < clipCount + 1; j++)
                    {
                        t1 = dt * j;
                        t2 = t1 * t1;
                        t3 = t1 * t2;

                        double x = A0 + A1 * t1 + A2 * t2 + A3 * t3;
                        double y = B0 + B1 * t1 + B2 * t2 + B3 * t3;

                        listOutputPnts.Add(new PointInfo(x, y));
                    }
                }
                return listOutputPnts;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /*
		 * 合并单值等值线，将一段一段的线合并为一整条等值线
		 */
		private void MergeIsolines()
        {
			for(int i = 0;i < tempIsolines.Count; i++){
				IsoLineInfo line = tempIsolines[i];
				if(line.FinishState)
					continue;
				if(MergeLine(i)){
                    tempIsolines.RemoveAt(i);
					i = -1;
				}
			}
		}

        /*
		 * 指定某段线的合并实现
		 */
		private bool MergeLine(int index)
        {
			IsoLineInfo line = tempIsolines[index];
			for(int i = 0;i < tempIsolines.Count; i++){
				if(i == index)
					continue;
				IsoLineInfo lineM = tempIsolines[i];
				if(lineM.FinishState)
					continue;
				PointInfo pntMFrom = lineM.FromPoint;//.GetLineFrom();
				PointInfo pntMEnd = lineM.ToPoint;//.GetLineEnd();
				
				PointInfo pntFrom = line.FromPoint;//.GetLineFrom();
				PointInfo pntEnd = line.ToPoint;//.GetLineEnd();
				
				if(pntMFrom.Equals(pntFrom) && pntMEnd.Equals(pntEnd)){  //首尾相接
                    for (int ij = line.ListVertrix.Count - 2; ij >= 0; ij--)
                    {
                        lineM.ListVertrix.Add(line.ListVertrix[ij]);
                    }
                    lineM.SetToPoint(pntFrom);
                    lineM.FinishState = true;
					return true;
				}
				else if(pntMFrom.Equals(pntEnd) && pntMEnd.Equals(pntFrom)){  //首尾相接
                    lineM.ListVertrix.AddRange(line.ListVertrix);
                    lineM.SetToPoint(pntEnd);
                    lineM.FinishState = true;
                    return true;
				}
				else if(pntMFrom.Equals(pntFrom)){
                    for(int ij = 0;ij<line.ListVertrix.Count;ij++){
                        lineM.ListVertrix.Insert(0, line.ListVertrix[ij]);
                    }
                    lineM.SetFromPoint(pntEnd);
					if(pntMEnd.IsEdge && pntEnd.IsEdge)
					{
						lineM.FinishState = true;
					}
                    //lineM.LineType = (lineM.LineType || line.LineType);
					return true;
				}
				else if(pntMFrom.Equals(pntEnd)){
                    for (int ij = line.ListVertrix.Count - 1; ij >= 0; ij--)
                    {
                        lineM.ListVertrix.Insert(0, line.ListVertrix[ij]);
                    }
                    lineM.SetFromPoint(pntFrom);
					if(pntMEnd.IsEdge && pntFrom.IsEdge)
					{
						lineM.FinishState = true;
					}
                    //lineM.LineType = (lineM.LineType || line.LineType);
					return true;
				}
				else if(pntMEnd.Equals(pntFrom)){
					lineM.ListVertrix.AddRange(line.ListVertrix);
                    lineM.SetToPoint(pntEnd);
					if(pntMFrom.IsEdge && pntEnd.IsEdge)
					{
						lineM.FinishState = true;
					}
                    //lineM.LineType = (lineM.LineType || line.LineType);
					return true;
				}
				else if(pntMEnd.Equals(pntEnd)){
                    for (int ij = line.ListVertrix.Count - 1; ij >= 0; ij--)
                    {
                        lineM.ListVertrix.Add(line.ListVertrix[ij]);
                    }
                    lineM.SetToPoint(pntFrom);
					if(pntMFrom.IsEdge && pntFrom.IsEdge){
						lineM.FinishState = true;
					}
                    //lineM.LineType = (lineM.LineType || line.LineType);
					return true;
				}
			}
			
			return false;
		}

        private void GetIsolines(double lineValue)
        {
			tempIsolines.Clear(); //清空数组
			for(int i = 0; i < pntGrid.GetLength(0) - 1; i++) {
				for(int j = 0; j < pntGrid.GetLength(1) - 1; j++) {
					PointInfo pntV4 = pntGrid[i,j];
					PointInfo pntV1 = pntGrid[i,j + 1];
					PointInfo pntV2 = pntGrid[i + 1,j + 1];
					PointInfo pntV3 = pntGrid[i + 1,j];

					int type1 = GetTypeValue(pntV1.Z, lineValue);
					int type2 = GetTypeValue(pntV2.Z, lineValue);
					int type3 = GetTypeValue(pntV3.Z, lineValue);
					int type4 = GetTypeValue(pntV4.Z, lineValue);
					string type = type1.ToString() + type2.ToString() + type3.ToString() + type4.ToString();
					
					//j为0时，点4为边界点；i为0时，点1为边界点；i为length-1时，点3为边界点；j为length-1时，点2为边界点
					PointInfo pnt1,pnt2,pnt3,pnt4;
					double x1, y1, x2, y2;
					switch(type){
						case "0000":
						case "1111":
							break;
						case "0001":  //1
						case "1110": //14
                            x1 = pntV4.PntCoord.X + (lineValue - pntV4.Z) / (pntV3.Z - pntV4.Z) * (pntV3.PntCoord.X - pntV4.PntCoord.X);
                            y1 = pntV4.PntCoord.Y + (lineValue - pntV4.Z) / (pntV1.Z - pntV4.Z) * (pntV1.PntCoord.Y - pntV4.PntCoord.Y);
							if(i == 0)  
							{
                                pnt1 = new PointInfo(pntV1.PntCoord.X, y1, lineValue, true);
								
							}else{
                                pnt1 = new PointInfo(pntV1.PntCoord.X, y1, lineValue, false);
							}
							if(j == 0){
                                pnt4 = new PointInfo(x1, pntV4.PntCoord.Y, lineValue, true);
							}
							else{
                                pnt4 = new PointInfo(x1, pntV4.PntCoord.Y, lineValue, false);
							}
							UpdateIsolines(pnt1,pnt4,lineValue);
							break;
						case "0010"://2
						case "1101"://13
                            x1 = pntV4.PntCoord.X + (lineValue - pntV4.Z) / (pntV3.Z - pntV4.Z) * (pntV3.PntCoord.X - pntV4.PntCoord.X);
                            y1 = pntV3.PntCoord.Y + (lineValue - pntV3.Z) / (pntV2.Z - pntV3.Z) * (pntV2.PntCoord.Y - pntV3.PntCoord.Y);
                            if (i == pntGrid.GetLength(0) - 2)  
							{
                                pnt3 = new PointInfo(pntV3.PntCoord.X, y1, lineValue, true);
								
							}else{
                                pnt3 = new PointInfo(pntV3.PntCoord.X, y1, lineValue, false);
							}
							if(j == 0){
                                pnt4 = new PointInfo(x1, pntV4.PntCoord.Y, lineValue, true);
							}
							else{
                                pnt4 = new PointInfo(x1, pntV4.PntCoord.Y, lineValue, false);
							}
							UpdateIsolines(pnt3,pnt4,lineValue);
							break;
						case "0011":  //3
						case "1100":  //12
                            y1 = pntV4.PntCoord.Y + (lineValue - pntV4.Z) / (pntV1.Z - pntV4.Z) * (pntV1.PntCoord.Y - pntV4.PntCoord.Y);
                            y2 = pntV3.PntCoord.Y + (lineValue - pntV3.Z) / (pntV2.Z - pntV3.Z) * (pntV2.PntCoord.Y - pntV3.PntCoord.Y);
							if(i == 0)  
							{
                                pnt1 = new PointInfo(pntV1.PntCoord.X, y1, lineValue, true);
                                pnt3 = new PointInfo(pntV3.PntCoord.X, y2, lineValue, false);
								
							}
							else if(i == pntGrid.GetLength(0) - 2)  
							{
                                pnt1 = new PointInfo(pntV1.PntCoord.X, y1, lineValue, false);
                                pnt3 = new PointInfo(pntV3.PntCoord.X, y2, lineValue, true);
							}
							else{
                                pnt1 = new PointInfo(pntV1.PntCoord.X, y1, lineValue, false);
                                pnt3 = new PointInfo(pntV3.PntCoord.X, y2, lineValue, false);
							}
							UpdateIsolines(pnt3,pnt1,lineValue);
							break;
						case "0100":   //4
						case "1011":   //11
                            x1 = pntV1.PntCoord.X + (lineValue - pntV1.Z) / (pntV2.Z - pntV1.Z) * (pntV2.PntCoord.X - pntV1.PntCoord.X);
                            y2 = pntV3.PntCoord.Y + (lineValue - pntV3.Z) / (pntV2.Z - pntV3.Z) * (pntV2.PntCoord.Y - pntV3.PntCoord.Y);
                            if (j == pntGrid.GetLength(1) - 2)
                            {
                                pnt2 = new PointInfo(x1, pntV1.PntCoord.Y, lineValue, true);
							}
							else{
                                pnt2 = new PointInfo(x1, pntV1.PntCoord.Y, lineValue, false);
							}
                            if (i == pntGrid.GetLength(0) - 2)  
							{
                                pnt3 = new PointInfo(pntV3.PntCoord.X, y2, lineValue, true);
							}
							else{
                                pnt3 = new PointInfo(pntV3.PntCoord.X, y2, lineValue, false);
							}
							UpdateIsolines(pnt3,pnt2,lineValue);
							break;
						case "0101": //5
                            y1 = pntV4.PntCoord.Y + (lineValue - pntV4.Z) / (pntV1.Z - pntV4.Z) * (pntV1.PntCoord.Y - pntV4.PntCoord.Y);
                            x1 = pntV1.PntCoord.X + (lineValue - pntV1.Z) / (pntV2.Z - pntV1.Z) * (pntV2.PntCoord.X - pntV1.PntCoord.X);
                            y2 = pntV3.PntCoord.Y + (lineValue - pntV3.Z) / (pntV2.Z - pntV3.Z) * (pntV2.PntCoord.Y - pntV3.PntCoord.Y);
                            x2 = pntV4.PntCoord.X + (lineValue - pntV4.Z) / (pntV3.Z - pntV4.Z) * (pntV3.PntCoord.X - pntV4.PntCoord.X);
							if(j == 0){
                                pnt2 = new PointInfo(x1, pntV1.PntCoord.Y, lineValue, false);
                                pnt4 = new PointInfo(x2, pntV4.PntCoord.Y, lineValue, true);
							}
                            else if (j == pntGrid.GetLength(1) - 2)
							{
                                pnt2 = new PointInfo(x1, pntV1.PntCoord.Y, lineValue, true);
                                pnt4 = new PointInfo(x2, pntV4.PntCoord.Y, lineValue, false);
							}
							else{
                                pnt2 = new PointInfo(x1, pntV1.PntCoord.Y, lineValue, false);
                                pnt4 = new PointInfo(x2, pntV4.PntCoord.Y, lineValue, false);
							}
							if(i == 0){
                                pnt1 = new PointInfo(pntV1.PntCoord.X, y1, lineValue, true);
                                pnt3 = new PointInfo(pntV3.PntCoord.X, y2, lineValue, false);
							}
                            else if (i == pntGrid.GetLength(0) - 2)  
							{
                                pnt1 = new PointInfo(pntV1.PntCoord.X, y1, lineValue, false);
                                pnt3 = new PointInfo(pntV3.PntCoord.X, y2, lineValue, true);
							}
							else{
                                pnt1 = new PointInfo(pntV1.PntCoord.X, y1, lineValue, false);
                                pnt3 = new PointInfo(pntV3.PntCoord.X, y2, lineValue, false);
							}
							UpdateIsolines(pnt1,pnt2,lineValue);
							UpdateIsolines(pnt3,pnt4,lineValue);
							break;
						case "0110":  //6
						case "1001":  //9
                            x1 = pntV1.PntCoord.X + (lineValue - pntV1.Z) / (pntV2.Z - pntV1.Z) * (pntV2.PntCoord.X - pntV1.PntCoord.X);
                            x2 = pntV4.PntCoord.X + (lineValue - pntV4.Z) / (pntV3.Z - pntV4.Z) * (pntV3.PntCoord.X - pntV4.PntCoord.X);
							if(j == 0){
                                pnt2 = new PointInfo(x1, pntV1.PntCoord.Y, lineValue, false);
                                pnt4 = new PointInfo(x2, pntV4.PntCoord.Y, lineValue, true);
							}
                            else if (j == pntGrid.GetLength(1) - 2)
							{
                                pnt2 = new PointInfo(x1, pntV1.PntCoord.Y, lineValue, true);
                                pnt4 = new PointInfo(x2, pntV4.PntCoord.Y, lineValue, false);
							}
							else{
                                pnt2 = new PointInfo(x1, pntV1.PntCoord.Y, lineValue, false);
                                pnt4 = new PointInfo(x2, pntV4.PntCoord.Y, lineValue, false);
							}
							UpdateIsolines(pnt4,pnt2,lineValue);
							break;
						case "0111":  //7
						case "1000": //8
                            y1 = pntV4.PntCoord.Y + (lineValue - pntV4.Z) / (pntV1.Z - pntV4.Z) * (pntV1.PntCoord.Y - pntV4.PntCoord.Y);
                            x1 = pntV1.PntCoord.X + (lineValue - pntV1.Z) / (pntV2.Z - pntV1.Z) * (pntV2.PntCoord.X - pntV1.PntCoord.X);
                            if (j == pntGrid.GetLength(1) - 2)
                            {
                                pnt2 = new PointInfo(x1, pntV1.PntCoord.Y, lineValue, true);
							}
							else{
                                pnt2 = new PointInfo(x1, pntV1.PntCoord.Y, lineValue, false);
							}
							if(i==0){
                                pnt1 = new PointInfo(pntV1.PntCoord.X, y1, lineValue, true);
							}
							else{
                                pnt1 = new PointInfo(pntV1.PntCoord.X, y1, lineValue, false);
							}
							UpdateIsolines(pnt1,pnt2,lineValue);
							break;
						case "1010":  //10
                            y1 = pntV4.PntCoord.Y + (lineValue - pntV4.Z) / (pntV1.Z - pntV4.Z) * (pntV1.PntCoord.Y - pntV4.PntCoord.Y);
                            x1 = pntV1.PntCoord.X + (lineValue - pntV1.Z) / (pntV2.Z - pntV1.Z) * (pntV2.PntCoord.X - pntV1.PntCoord.X);
                            y2 = pntV3.PntCoord.Y + (lineValue - pntV3.Z) / (pntV2.Z - pntV3.Z) * (pntV2.PntCoord.Y - pntV3.PntCoord.Y);
                            x2 = pntV4.PntCoord.X + (lineValue - pntV4.Z) / (pntV3.Z - pntV4.Z) * (pntV3.PntCoord.X - pntV4.PntCoord.X);
							if(j==0){
                                pnt2 = new PointInfo(x1, pntV1.PntCoord.Y, lineValue, false);
                                pnt4 = new PointInfo(x2, pntV4.PntCoord.Y, lineValue, true);
							}
                            else if (j == pntGrid.GetLength(1) - 2)
							{
                                pnt2 = new PointInfo(x1, pntV1.PntCoord.Y, lineValue, true);
                                pnt4 = new PointInfo(x2, pntV4.PntCoord.Y, lineValue, false);
							}
							else{
                                pnt2 = new PointInfo(x1, pntV1.PntCoord.Y, lineValue, false);
                                pnt4 = new PointInfo(x2, pntV4.PntCoord.Y, lineValue, false);
							}
							if(i==0){
                                pnt1 = new PointInfo(pntV1.PntCoord.X, y1, lineValue, true);
                                pnt3 = new PointInfo(pntV3.PntCoord.X, y2, lineValue, false);
							}
                            else if (i == pntGrid.GetLength(0) - 2)  
							{
                                pnt1 = new PointInfo(pntV1.PntCoord.X, y1, lineValue, false);
                                pnt3 = new PointInfo(pntV3.PntCoord.X, y2, lineValue, true);
							}
							else{
                                pnt1 = new PointInfo(pntV1.PntCoord.X, y1, lineValue, false);
                                pnt3 = new PointInfo(pntV3.PntCoord.X, y2, lineValue, false);
							}
							UpdateIsolines(pnt3,pnt2,lineValue);
							UpdateIsolines(pnt1,pnt4,lineValue);
							break;
					}
				}
			}
		}
    }
}
