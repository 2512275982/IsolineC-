using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hykj.GISModule
{
    /// <summary>
    /// 坐标转换类
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// 计算两点间距离
        /// </summary>
        /// <param name="xy1">位置1</param>
        /// <param name="xy2">位置2</param>
        /// <returns>距离(单位米)</returns>
        public static double Distance(高斯坐标 xy1, 高斯坐标 xy2)
        {
            double d = (xy1.x - xy2.x) * (xy1.x - xy2.x) + (xy1.y - xy2.y) * (xy1.y - xy2.y);
            return Math.Sqrt(d);
        }
        /// <summary>
        /// 高斯反算
        /// </summary>
        public static 大地坐标 xyh_to_BLH(高斯坐标 xyz, double L)
        {
            地球椭球 ellipsoid = new 地球椭球(enum坐标系.WGS84, eunm度带._3度带);
            大地坐标 BLH = new 大地坐标();
            xyz.y -= 500000;
            double Bf;
            double a = ellipsoid.a;
            double b = ellipsoid.b;
            double e = Math.Sqrt(a * a - b * b) / a;//第一偏心率
            double e1 = Math.Sqrt(a * a - b * b) / b;//第二偏心率
            double A = 1 + (3 / 4.0) * e * e + (45 / 64.0) * e * e * e * e + (175 / 256.0) * e * e * e * e * e * e + (11025 / 16384.0) * e * e * e * e * e * e * e * e;
            double B = (3 / 4.0) * e * e + (15 / 16.0) * e * e * e * e + (525 / 512.0) * e * e * e * e * e * e + (2205 / 2048.0) * e * e * e * e * e * e * e * e;
            double C = (15 / 64.0) * e * e * e * e + (105 / 256.0) * e * e * e * e * e * e + (2205 / 4096.0) * e * e * e * e * e * e * e * e;
            double D = (35 / 512.0) * e * e * e * e * e * e + (315 / 2048.0) * e * e * e * e * e * e * e * e;
            double E = (315 / 16384.0) * e * e * e * e * e * e * e * e;
            double Bf0 = xyz.x / A / a / (1 - e * e);
            do
            {
                Bf = xyz.x / a / (1 - e * e) / A + (B / 2 * Math.Sin(2 * Bf0) - C / 4 * Math.Sin(4 * Bf0) + D / 6 * Math.Sin(6 * Bf0) - E / 8 * Math.Sin(8 * Bf0)) / A;
                if (Math.Abs(Bf - Bf0) < 0.000000000001) break;
                Bf0 = Bf;
            } while (true);
            double t = Math.Tan(Bf);
            double W = Math.Sqrt(1 - e * e * Math.Sin(Bf) * Math.Sin(Bf));
            double M = a * (1 - e * e) / Math.Pow(W, 3);
            double N = a / W;
            double n = e1 * Math.Cos(Bf);

            BLH.B = Bf - t / 2 / M / N * xyz.y * xyz.y + t * (5 + 3 * Math.Pow(t, 2) + n * n - 9 * n * n * t * t) * Math.Pow(xyz.y, 4) / (24 * M * Math.Pow(N, 3)) - t * (61 + 90 * t * t + 45 * Math.Pow(t, 4)) * Math.Pow(xyz.y, 6) / (720 * M * Math.Pow(N, 5));
            BLH.B = BLH.B * fun角度转换.toDu;
            double l = xyz.y / N / Math.Cos(Bf) - Math.Pow(xyz.y / N, 3) / 6 / Math.Cos(Bf) * (1 + 2 * t * t + n * n) + Math.Pow(xyz.y / N, 5) / 120 / Math.Cos(Bf) * (5 + 28 * t * t + 24 * Math.Pow(t, 4) + 6 * n * n + 8 * n * n * t * t);
            int Mstrip = (int)((L + 1.5) / 3);//带号
            double l0 = 0;
            if (ellipsoid.i度带 == eunm度带._3度带)
            {
                l0 = Mstrip * 3;
            }
            if (ellipsoid.i度带 == eunm度带._6度带)
            {
                l0 = Mstrip * 6 - 3;
            }
            BLH.L = l * fun角度转换.toDu + l0;

            BLH.H = xyz.z;

            return BLH;
        }
        /// <summary>
        /// 高斯正算
        /// </summary>
        /// <param name="ellipsoid">椭球参数</param>
        public static 高斯坐标 BLH_to_xyh(大地坐标 BLH)
        {
            高斯坐标 xyz = new 高斯坐标();
            地球椭球 ellipsoid = new 地球椭球(enum坐标系.WGS84, eunm度带._3度带);
            double B = BLH.B;
            double L = BLH.L;
            double a = ellipsoid.a;
            double b = ellipsoid.b;
            double e = Math.Sqrt(a * a - b * b) / a;//第一偏心率
            double e1 = Math.Sqrt(a * a - b * b) / b;//第二偏心率
            double b1 = B * fun角度转换.toRad;
            double N = a / (Math.Sqrt(1 - e * e * Math.Sin(b1) * Math.Sin(b1)));
            double t = Math.Tan(b1);
            double n = e1 * Math.Cos(b1);

            //计算3度带
            if (ellipsoid.i度带 == eunm度带._3度带)
            {
                double n3 = (int)(L / 3 + 0.5);
                double l = 3 * n3;
                double l3 = (L - l) * Math.PI / 180;

                xyz.x = CalculateX(a, b, B) + N * Math.Sin(b1) * Math.Cos(b1) * l3 * l3 / 2.0 + N * Math.Sin(b1) * Math.Pow(Math.Cos(b1), 3) * (5 - t * t + 9 * n * n + 4 * Math.Pow(n, 4)) * Math.Pow(l3, 4) / 24.0 + N * Math.Sin(b1) * Math.Pow(Math.Cos(b1), 5) * (61 - 58 * t * t + Math.Pow(t, 4)) * Math.Pow(l3, 6) / 720.0;
                double y3 = N * Math.Cos(b1) * l3 + N * Math.Pow(Math.Cos(b1), 3) * (1 - t * t + n * n) * Math.Pow(l3, 3) / 6.0 + N * Math.Pow(Math.Cos(b1), 5) * (5 - 18 * t * t + t * t * t * t + 14 * n * n - 58 * n * n * t * t) * Math.Pow(l3, 5) / 120.0;
                //xyz.y = n3 * 1000000.0 + y3 + 500000;
                xyz.y = y3 + 500000;

            }
            //计算6度带
            if (ellipsoid.i度带 == eunm度带._6度带)
            {
                double n6 = (int)((L + 3) / 6 + 0.5);
                double l = n6 * 6 - 3;
                double l6 = (-l) * Math.PI / 180;

                xyz.x = CalculateX(a, b, B) + N * Math.Sin(b1) * Math.Cos(b1) * l6 * l6 / 2.0 + N * Math.Sin(b1) * Math.Pow(Math.Cos(b1), 3) * (5 - t * t + 9 * n * n + 4 * Math.Pow(n, 4)) * Math.Pow(l6, 4) / 24.0 + N * Math.Sin(b1) * Math.Pow(Math.Cos(b1), 5) * (61 - 58 * t * t + Math.Pow(t, 4)) * Math.Pow(l6, 6) / 720.0;
                double y6 = N * Math.Cos(b1) * l6 + N * Math.Pow(Math.Cos(b1), 3) * (1 - t * t + n * n) * Math.Pow(l6, 3) / 6.0 + N * Math.Pow(Math.Cos(b1), 5) * (5 - 18 * t * t + t * t * t * t + 14 * n * n - 58 * n * n * t * t) * Math.Pow(l6, 5) / 120.0;
                xyz.y = n6 * 1000000.0 + y6 + 500000;
            }

            xyz.z = BLH.H;

            return xyz;
        }
        /// <summary>
        /// 定义函数：子午线弧长公式,即计算公式中的X
        /// </summary>
        /// <param name="a">地球长半轴</param>
        /// <param name="b">地球短半轴</param>
        /// <param name="B2">经度</param>
        /// <returns>赤道延子午线到该点的一段弧长</returns>
        private static double CalculateX(double a, double b, double B2)
        {
            double e = Math.Sqrt(a * a - b * b) / a;
            double A = 1 + (3 / 4.0) * e * e + (45 / 64.0) * e * e * e * e + (175 / 256.0) * e * e * e * e * e * e + (11025 / 16384.0) * e * e * e * e * e * e * e * e + (43659 / 65536.0) * e * e * e * e * e * e * e * e * e * e;
            double B = (3 / 8.0) * e * e + (15 / 32.0) * e * e * e * e + (525 / 1024.0) * e * e * e * e * e * e + (2205 / 4096.0) * e * e * e * e * e * e * e * e + (72765 / 131072.0) * e * e * e * e * e * e * e * e * e * e;
            double C = (15 / 256.0) * e * e * e * e + (105 / 1024.0) * e * e * e * e * e * e + (2205 / 16384.0) * e * e * e * e * e * e * e * e + (10395 / 65536.0) * e * e * e * e * e * e * e * e * e * e;
            double D = (35 / 3072.0) * e * e * e * e * e * e + (105 / 4096.0) * e * e * e * e * e * e * e * e + (10395 / 262144.0) * e * e * e * e * e * e * e * e * e * e;
            double E = (315 / 131072.0) * e * e * e * e * e * e * e * e + (3465 / 524288.0) * e * e * e * e * e * e * e * e * e * e;
            double F = (639 / 1310720.0) * e * e * e * e * e * e * e * e * e * e;
            double B1 = B2 * fun角度转换.toRad;

            return a * (1 - e * e) * ((A * B1) - B * Math.Sin(2 * B1) + C * Math.Sin(4 * B1) - D * Math.Sin(6 * B1) + E * Math.Sin(8 * B1) - F * Math.Sin(10 * B1));
        }
    }

    #region 坐标转换相关
    public enum enum经纬半球
    {
        /// <summary>
        /// 东经
        /// </summary>
        E,
        /// <summary>
        /// 西经
        /// </summary>
        W,
        /// <summary>
        /// 北纬
        /// </summary>
        N,
        /// <summary>
        /// 南纬
        /// </summary>
        S
    }

    public enum enum坐标系
    {
        CGS2000,
        WGS84,
        西安1980,
        北京54
    }

    public enum eunm度带
    {
        _3度带 = 3,
        _6度带 = 6
    }

    public class 地球椭球
    {
        public enum坐标系 zbx = enum坐标系.WGS84;
        public eunm度带 i度带 = eunm度带._3度带;
        public double a = 6378137, b = 6356752.314;

        public 地球椭球() { }

        public 地球椭球(enum坐标系 zbx)
        {
            switch (zbx)
            {
                case enum坐标系.北京54:
                    a = 6378245; b = 6356863.0188;
                    break;
                case enum坐标系.西安1980:
                    a = 6378140; b = 6356755.2882;
                    break;
                case enum坐标系.WGS84:
                    a = 6378137; b = 6356752.314;
                    break;
                case enum坐标系.CGS2000:
                    a = 6378137; b = 6356752.3141;
                    break;
            }
        }

        public 地球椭球(enum坐标系 zbx, eunm度带 i度带)
            : this(zbx)
        {
            this.i度带 = i度带;
        }
    }

    public class 高斯坐标
    {
        public double x;
        public double y;
        public double z;

        public 高斯坐标() { }

        public 高斯坐标(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class 大地坐标
    {
        /// <summary>
        /// 纬度
        /// </summary>
        public double B { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double L { get; set; }
        /// <summary>
        /// 高程
        /// </summary>
        public double H { get; set; }

        public enum经纬半球 NS, EW;

        public 大地坐标()
        {
            this.NS = enum经纬半球.N;
            this.EW = enum经纬半球.E;
        }

        public 大地坐标(double B, double L, double H)
        {
            this.B = B;
            this.L = L;
            this.H = H;
            this.NS = B > 0 ? enum经纬半球.N : enum经纬半球.S;
            this.EW = L > 0 ? enum经纬半球.E : enum经纬半球.W;
        }

        public 大地坐标(double B, double L, double H, enum经纬半球 NS, enum经纬半球 EW)
        {
            this.B = (NS == enum经纬半球.N ? 1 : -1) * Math.Abs(B);
            this.L = (EW == enum经纬半球.E ? 1 : -1) * Math.Abs(L);
            this.H = H;
            this.EW = EW;
            this.NS = NS;

        }
    }

    public class fun角度转换
    {
        public static double toRad = Math.PI / 180;
        public static double toDu = 180 / Math.PI;
        /// <summary>
        /// 度分转度
        /// </summary>
        /// <param name="dufen">度分</param>
        /// <returns>度</returns>
        public static double dufen_to_du(double dufen)
        {
            double sign = Math.Sign(dufen), k = Math.Abs(dufen);
            double du = Math.Truncate(k / 100),
                   fen = k - du * 100;
            return sign * (du + fen / 60);
        }
        /// <summary>
        /// 度分秒转度
        /// </summary>
        /// <param name="dufenMiao">度分秒</param>
        /// <returns>度</returns>
        public static double dufenMiao_to_du(double dufenMiao)
        {
            double du = 0;
            return du;
        }
        /// <summary>
        /// 度分秒转度
        /// </summary>
        /// <param name="dufenMiao"度分秒(string类型)></param>
        /// <returns>度分秒</returns>
        public static double dufenMiao_to_du(string dufenMiao)
        {
            int du = int.Parse(dufenMiao.Split('°')[0]);
            double fen = int.Parse(dufenMiao.Split('°')[1].Split('′')[0]) / 60.0;
            double miao = double.Parse(dufenMiao.Split('′')[1].Split('″')[0]) / 3600;
            return du + fen + miao;
        }
    }
    #endregion
}
