using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GeoCodingConsole
{
    public class Rootobject
    {
        public Feature[] features { get; set; }
    }

    public class Feature
    {
        public string title { get; set; }
        public string subtitle { get; set; }
        public bool isTextInTitle { get; set; }
        public int zIndex { get; set; }
        public string type { get; set; }
        public float[] coordinates { get; set; }
        public Stroke stroke { get; set; }
        public bool isComma { get; set; }
        public Content content { get; set; }
        public string caption { get; set; }
        public float[] balloonCoordinates { get; set; }
        public Geometry geometry { get; set; }
        public Fill fill { get; set; }
    }

    public class Stroke
    {
        public string color { get; set; }
        public float opacity { get; set; }
        public int width { get; set; }
    }

    public class Content
    {
        public string type { get; set; }
        public string text { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public object[][] coordinates { get; set; }
        public LatLon[] Coordinates { 
            get
            {
                if(coordinates != null)
                {
                    int coordLen;
                    LatLon[] coord;
                    if (this.type == "Polygon")
                    {
                        coordLen = coordinates[0].Length;
                        coord = new LatLon[coordLen];

                        var n = -1;
                        foreach (var obj in coordinates)
                        {
                            foreach (var objn in obj)
                            {
                                n++;

                                var jLon = (JsonElement)objn;
                                var jLat = (JsonElement)objn;

                                coord[n].Lat = jLat[1].GetDouble();
                                coord[n].Lon = jLon[0].GetDouble();
                            }
                        }
                    }
                    else
                    {
                        coordLen = coordinates.Length;
                        coord = new LatLon[coordLen];

                        for (int i = 0; i < coordLen; i++)
                        {
                            var jLon = (JsonElement)coordinates[i][0];
                            var jLat = (JsonElement)coordinates[i][1];
                            coord[i].Lat = jLat.GetDouble();
                            coord[i].Lon = jLon.GetDouble();
                        }
                    }

                   
                    return coord;
                }
                return new LatLon[] { };
            } 
        }
    }

    public struct LatLon
    {
        public double Lat;
        public double Lon;

        public LatLon(double lat, double lon)
        {
            Lat = lat;
            Lon = lon;
        }
    }

    public class Fill
    {
        public string color { get; set; }
        public float opacity { get; set; }
    }

}
