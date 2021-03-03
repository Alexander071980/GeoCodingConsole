using System;
using System.IO;
using System.Linq;
using GeoCodingLib;
using System.Collections.Generic;
using OfficeOpenXml;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GeoCodingConsole
{
    class Program
    {
        static string pathKmz = "output.kmz";
        static string pathJson = "Nii.json";
        static string nameDocumentKml = "Nii";
        static List<PointSoUd> pointSoUds = new List<PointSoUd>();
        static List<StyleDoc> styleDocs = new List<StyleDoc>
        {
            new StyleDoc("niir", "images/niir.png")
           
        };

        public static async Task Main(string[] args)
        {
            Rootobject rootobject;

            using (FileStream fs = new FileStream(pathJson, FileMode.Open))
            {               
                rootobject =  await JsonSerializer.DeserializeAsync<Rootobject>(fs);
            }

            var kml = new KmlFileUsers(styleDocs, nameDocumentKml);

           

            foreach (var rootobj in rootobject.features)
            {
                if(rootobj.type == "placemark")
                {
                    var point = new Point
                    {
                        Name = rootobj.title,
                        Lat = rootobj.coordinates[1],
                        Lon = rootobj.coordinates[0]
                    };

                    

                    kml.AddPoint(point, "niir", new Dictionary<string, string> { { "Наименование", rootobj.title } });
                }
                else if(rootobj.type == "line")
                {
                    var line = new Line
                    {
                        Name = rootobj.title,
                        Color = rootobj.stroke.color,
                        Coordinates = rootobj.geometry.Coordinates
                            .Select(p => new LatLonStruct { Lat = p.Lat, Lon = p.Lon }).ToArray(),
                        Width = rootobj.stroke.width,
                        Opacity = (int)Math.Round(rootobj.stroke.opacity * 100, 0)
                    };

                    kml.AddLine(line, new Dictionary<string, string> { { "Наименование", rootobj.title} });
                }
                else if(rootobj.type == "polygon")
                {
                    LatLonStruct[] latLonStruct = rootobj.geometry.Coordinates
                        .Select(p => new LatLonStruct { Lat = p.Lat, Lon = p.Lon }).ToArray();
                    
                    var polygon = new Polygon(rootobj.title, rootobj.stroke.color, rootobj.fill.color, rootobj.stroke.width, latLonStruct);
                    polygon.OpacityLine = (int)Math.Round(rootobj.stroke.opacity * 100, 0);
                    polygon.OpacityPoly = (int)Math.Round(rootobj.fill.opacity * 100, 0);
                    kml.AddPoligon(polygon, new Dictionary<string, string> { { "Наименование", rootobj.title } });
                }
            }

            kml.SaveKmz(pathKmz);
        }
    }
}
