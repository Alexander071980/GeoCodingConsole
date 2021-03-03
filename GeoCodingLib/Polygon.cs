using System;
using System.Collections.Generic;
using System.Text;
using sb = SharpKml.Base;
using sd = SharpKml.Dom;

namespace GeoCodingLib
{
    public class Polygon
    {
        public string Name { get; set; }
        public string ColorLine { get; set; }
        public string ColorPoly { get; set; }
        public int WidthLine { get; set; }
        public int OpacityLine { get; set; }
        public int OpacityPoly { get; set; }
        public Dictionary<string, string> DicData { get; set; }
        public sd.CoordinateCollection OuterBoundaryIs { get; set; }
        public sd.CoordinateCollection InnerBoundaryIs { get; set; }

        public Polygon
            (
            string name, 
            string colorLine, 
            string colorPoly, 
            int widthLine, 
            LatLonStruct[] outerBoundaryIs, 
            LatLonStruct[] innerBoundaryIs = null, 
            double? opacityLine = null, 
            double? opacitypoly = null
            )
        {
            Name = name;
            OuterBoundaryIs = GetVectors(outerBoundaryIs);
            InnerBoundaryIs = GetVectors(innerBoundaryIs);
            ColorLine = colorLine;
            ColorPoly = colorPoly;
            WidthLine = widthLine;
        }
      
        public sd.CoordinateCollection GetVectors(LatLonStruct[] latLonStruct)
        {
            if(latLonStruct is null) { return new sd.CoordinateCollection(); }
            var coordColl = new sd.CoordinateCollection();
            foreach (var obj in latLonStruct)
            {
                coordColl.Add(new sb.Vector(obj.Lat, obj.Lon));
            }

            return coordColl;
        }
    }
}
