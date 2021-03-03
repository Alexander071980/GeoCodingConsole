using System;
using System.Collections.Generic;
using System.Text;

namespace GeoCodingLib
{
    public struct LatLonStruct
    {      
        public double Lat;
        public double Lon;

        public LatLonStruct(double lat, double lon)
        {
            Lat = lat;
            Lon = lon;
        }        
    }
}
