using System;
using System.Collections.Generic;
using System.Text;
using sb = SharpKml.Base;
using sd = SharpKml.Dom;
using SharpKml.Engine;

namespace GeoCodingLib
{
    public class Line
    {
        public string Name { get; set; }
        public LatLonStruct[] Coordinates { get; set; } = new LatLonStruct[] { };
        public string Color { get; set; }
        public int Width { get; set; }
        public Dictionary<string, string> DicData { get; set; }
        public int Opacity { get; set; }
        public sd.CoordinateCollection Vectors
        {
            get
            {
                var coordColl = new sd.CoordinateCollection();
                foreach(var obj in Coordinates)
                {
                    coordColl.Add(new sb.Vector(obj.Lat, obj.Lon));
                }

                return coordColl;
            }
        }
    }
}
