using System;
using System.Collections.Generic;
using System.Text;
using Geodesy;

namespace GeoCodingLib
{
    class Point
    {
        private readonly GeodeticCalculator geoCalc = new GeodeticCalculator(Ellipsoid.WGS84);
        public double Lat { get; }
        public double Lon { get; }
        
        public GlobalCoordinates LatLon
        {
            get
            {
                return new GlobalCoordinates(
                    new Angle(Lat), new Angle(Lon)
                    );
            }
        }

        public Point(double lat, double lon)
        {            
            Lat = lat;
            Lon = lon;
        }
        /// <summary>
        /// Дистанция до Точки в метрах
        /// </summary>
        /// <param name="pointEnd">Точка до которой считается дистанция</param>
        /// <returns></returns>
        public double DistToPoint(Point pointEnd)
        {
            GeodeticCurve curve = geoCalc.CalculateGeodeticCurve( LatLon, pointEnd.LatLon);
            return curve.EllipsoidalDistance;
        }
        /// <summary>
        /// Азимут до точки в градусах
        /// </summary>
        /// <param name="pointEnd">Точка до которой рассчитывается азимут</param>
        /// <returns></returns>
        public int AngleToPoint(Point pointEnd)
        {
            GeodeticCurve curve = geoCalc.CalculateGeodeticCurve(LatLon, pointEnd.LatLon);
            return (int)curve.Azimuth.Degrees;
        }
    }
}
