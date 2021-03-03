using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GeoCodingFram
{
    public class Sector : Point
    {      
        public string SectorString { get; set; }
        public double DistanceMax { get; }
        public int WidthSec { get; }
        public int AzimuthSec { get; }
        public int AzStart { get; set; }
        public int AzEnd { get; set; }
        public int AzDelta { get; set; }
        public string Errors { get; }
        public Sector(string number, string adress, double lat, double lon, string sectorString, double distanceMax)
            : base(number, adress, lat, lon)
        {
            SectorString = sectorString;
            DistanceMax = distanceMax;
            if (!SectorStringConvertToAngle(sectorString))
            {
                Errors = "Ошибка в поле сектор";
            }
        }
        public Sector(string number, string adress, double lat, double lon, int azimuthSec, int widthSec, double distanceMax)
           : base(number, adress, lat, lon)
        {
            DistanceMax = distanceMax;
            WidthSec = widthSec;
            AzimuthSec = azimuthSec;
            if(!AzimuthSecConvertToAngle(azimuthSec, widthSec))
            {
                Errors = "Ошибка в поле Азимут сектора или Ширина сектора";
            }
        }

        /// <summary>
        /// Из столбца сектор вида 0-360 получаем массив значений углов
        /// </summary>
        /// <returns></returns>
        private bool SectorStringConvertToAngle(string sectorString)
        {
            if (sectorString == "") { return false; }

            int f1, f2, fdelta;

            if (sectorString.Contains("-"))
            {
                try
                {
                    MatchCollection matches = Regex.Matches(sectorString, @"\d+");
                    f1 = int.Parse(matches[0].Value);
                    int last = matches.Count - 1;
                    f2 = int.Parse(matches[last].Value);
                }
                catch (Exception)
                {

                    return false;
                }
               
            }
            else { return false; }
            
            if (f1 == 0 & f2 == 360)
            {
                fdelta = 360;
            }
            else
            {
                fdelta = (f2 - f1 + 360) % 360;
            }

            AzStart = f1;
            AzEnd = f2;
            AzDelta = fdelta;
            return true;
        }
        /// <summary>
        /// Конверт Азимута и угла в массив углов и в sectorString
        /// </summary>
        /// <param name="azimuthSec">Азиммут сектора в градусах</param>
        /// <param name="widthSec">Ширина сектора в градусах</param>
        /// <returns></returns>
        private bool AzimuthSecConvertToAngle(int azimuthSec, int widthSec)
        {
            if(widthSec == 0) { return false; }
            AzStart = (azimuthSec - WidthSec / 2 + 360) % 360;
            AzEnd = (azimuthSec + WidthSec / 2 + 360) % 360;
            AzDelta = widthSec;

            if (AzEnd < AzStart)
            {
                SectorString = $"{AzStart}-0-{AzEnd}";
            }
            else
            {
                SectorString = $"{AzStart}-{AzEnd}";
            }
            return true;
        }
        /// <summary>
        /// Точка в секторе по азимуту
        /// </summary>
        /// <param name="point">Точка в секторе или нет</param>
        /// <param name="distLimit">Учитывать ли Макс дистанцию сектора true - да</param>
        /// <returns></returns>
        public bool V_sectore(Point point, bool distLimit = false)
        {
            if (distLimit)
            {
                int distToPoint = (int)DistToPoint(point);
                if(distToPoint < DistanceMax) { return false; }
            }
            int azimuth_ot_bs = AngleToPoint(point);

            if (AzStart < AzEnd)
            {
                if (azimuth_ot_bs >= AzStart & azimuth_ot_bs <= AzEnd)
                { return true; }
                else
                { return false; }
            }
            else
            {
                if (azimuth_ot_bs >= AzStart & azimuth_ot_bs <= 360)
                { return true; }
                else if (azimuth_ot_bs >= 0 & azimuth_ot_bs <= AzEnd)
                { return true; }
                else
                { return false; }
            }

        }
    }
}
