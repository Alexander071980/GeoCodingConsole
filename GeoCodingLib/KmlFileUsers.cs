using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using sb = SharpKml.Base;
using sd = SharpKml.Dom;
using SharpKml.Engine;
using System.Drawing;


namespace GeoCodingLib
{
    public class KmlFileUsers
    {
        sd.Document Document { get; set; } = new sd.Document();
        sd.Kml Kml { get; set; } = new sd.Kml();
        public KmlFileUsers(List<StyleDoc> styleDocs, string nameDoc)
        {
            Document.Name = nameDoc;
            Kml.Feature = Document;
            StylesInDoc(styleDocs);
        }

        public sd.Folder AddFolder(string name)
        {
            var folder = new sd.Folder { Name = name };
            Document.AddFeature(folder);
            return folder;
        }
        public void AddPoint(Point point, string idStyle, Dictionary<string, string> dicData, sd.Folder folder = null)
        {
            var pointDoc = new sd.Point
            {
                Coordinate = new sb.Vector(point.Lat, point.Lon)
            };
           
            var placemark = new sd.Placemark
            {
                Name = point.Name,
                Geometry = pointDoc,
                StyleUrl = new Uri($"#{idStyle}", UriKind.Relative),
                ExtendedData = ExtendedData(dicData)
            };

            if (folder == null)
            {
                Document.AddFeature(placemark);
            }
            else
            {
                folder.AddFeature(placemark);
            }

        }

        public void AddLine(Line lineIn, Dictionary<string, string> dicData)
        {
            var colorLine = sb.Color32.Parse(ConvertColor(sb.Color32.Parse(lineIn.Color), lineIn.Opacity));
            var style = new sd.Style
            {
                Line = new sd.LineStyle
                {
                    Color = colorLine,
                    Width = lineIn.Width,
                }
            };
            var line = new sd.LineString
            {
                Extrude = true,
                Tessellate = true,
                Coordinates = lineIn.Vectors
            };

            var placemark = new sd.Placemark
            {
                Name = lineIn.Name,
                Geometry = line,
                ExtendedData = ExtendedData(dicData)
            };
            placemark.AddStyle(style);

            Document.AddFeature(placemark);
        }

        public void AddPoligon(Polygon poly, Dictionary<string, string> dicData)
        {
            var colorPoly = sb.Color32.Parse(ConvertColor(sb.Color32.Parse(poly.ColorPoly), poly.OpacityPoly));
            var colorLine = sb.Color32.Parse(ConvertColor(sb.Color32.Parse(poly.ColorLine), poly.OpacityLine));
            var style = new sd.Style
            {
                Polygon = new sd.PolygonStyle
                {
                    Fill = true,
                    Color = colorPoly,
                    Outline = true                   
                },
                Line = new sd.LineStyle
                {
                    Color = colorLine,
                    Width = poly.WidthLine,
                }
            };
            var polygon = new sd.Polygon
            {
                Extrude = true,
                Tessellate = true,
                OuterBoundary = new sd.OuterBoundary 
                {
                    LinearRing = new sd.LinearRing 
                    {
                        Coordinates = poly.OuterBoundaryIs
                    } 
                }
            };

            var placemark = new sd.Placemark
            {
                Name = poly.Name,
                Geometry = polygon,
                ExtendedData = ExtendedData(dicData)
            };
            placemark.AddStyle(style);

            Document.AddFeature(placemark);
        }

        void StylesInDoc(List<StyleDoc> styleDocs)
        {
            foreach(var style in styleDocs)
            {
                var styleDoc = new sd.Style();
                styleDoc.Id = style.Id;
                styleDoc.Icon = new sd.IconStyle();
                styleDoc.Icon.Icon = new sd.IconStyle.IconLink(new Uri(style.HrefPoint, UriKind.Relative));
                styleDoc.Label = new sd.LabelStyle
                {
                    Scale = 0
                };
                Document.AddStyle(styleDoc);
            }
        }

        public void SaveKmz(string outputPathFile)
        {
            var kmlFile = KmlFile.Create(Kml, true);

            using KmzFile kmz = SaveKmlAndLinkedContentIntoAKmzArchive(kmlFile, outputPathFile);
            using Stream output = File.Create(outputPathFile);
            kmz.Save(output);
            Console.WriteLine("Saved to '{0}'.", outputPathFile);
        }
      

        private static KmzFile SaveKmlAndLinkedContentIntoAKmzArchive(KmlFile kml, string path)
        {
            // All the links in the KML will be relative to the KML file, so
            // find it's directory so we can add them later
            string basePath = Path.GetDirectoryName(path);

            // Create the archive with the KML data
            KmzFile kmz = KmzFile.Create(kml);

            // Now find all the linked content in the KML so we can add the
            // files to the KMZ archive
            var links = new LinkResolver(kml);

            // Next gather the local references and add them.
            foreach (string relativePath in links.GetRelativePaths())
            {
                // Make sure it doesn't point to a directory below the base path
                if (relativePath.StartsWith("..", StringComparison.Ordinal))
                {
                    continue;
                }

                // Add it to the archive
                string fullPath = Path.Combine(basePath, relativePath);
                using (Stream file = File.OpenRead(fullPath))
                {
                    kmz.AddFile(relativePath, file);
                }
            }

            return kmz;
        }

        private sd.ExtendedData ExtendedData(Dictionary<string, string> dicData)
        {
            if (dicData is null) return new sd.ExtendedData();
            var exdata = new sd.ExtendedData();

            foreach (var data in dicData)
            {
                string value = data.Value;
                if (data.Value.Length == 0)
                {
                    value = "Нет данных";
                }
                var dataDoc = new sd.Data
                {
                    Name = data.Key,
                    Value = value
                };
                exdata.AddData(dataDoc);
            }

            return exdata;
        }

        /// <summary>
        /// Конверт цвета в строковый формат kml
        /// </summary>
        /// <param name="color">Цвет в формате</param>
        /// <param name="opacity">Прозрачность в процентах</param>
        /// <returns></returns>
        private string ConvertColor(Color color, int opacity)
        {
            string clr = String.Format("{0:X2}{1:X2}{2:X2}", color.B, color.G, color.R);
            return ConvertColorPry(clr, opacity);
        }
        private string ConvertColor(sb.Color32 color, int opacity)
        {
            string clr = String.Format("{0:X2}{1:X2}{2:X2}", color.Red, color.Green, color.Blue);
            return ConvertColorPry(clr, opacity);
        }
        private string ConvertColorPry(string clr, int opacity)
        {
            string txtOpacity = opacity.ToString();           
            decimal percentOpacity = ((Convert.ToDecimal(txtOpacity) / 100) * 255);
            percentOpacity = Math.Floor(percentOpacity);
            opacity = Convert.ToInt32(percentOpacity);
            string opacityString = opacity.ToString("x");
            string polyColor = opacityString + clr;
            return polyColor;
        }


        public static void Run()
        {
            var point = new sd.Point
            {
                Coordinate = new sb.Vector(37.42052549, -122.0816695)
            };

            var placemark = new sd.Placemark
            {
                Name = "Да круто",
                Geometry = point
            };

            var point1 = new sd.Point
            {
                Coordinate = new sb.Vector(37.419837, -122.078902)
            };

            var placemark1 = new sd.Placemark
            {
                Name = "Да круто",
                Geometry = point1
            };

            var document = new sd.Document
            {
                Description = new sd.Description { Text = "Документ" }
            };

            var kml = new sd.Kml
            {
                Feature = document
            };

            var folder = new sd.Folder
            {
                Description = new sd.Description { Text = "Светофоры" },
                Name = "СО"

            };

            folder.AddFeature(placemark);
            document.AddFeature(folder);
            document.AddFeature(placemark1);

            //var serializer = new sb.Serializer();

            //using FileStream fileStream = new FileStream("kmlTest.kml", FileMode.OpenOrCreate);
            //serializer.Serialize(kml, fileStream);
            var kmlFile = KmlFile.Create(kml, true);

            //using KmzFile kmz = SaveKmlAndLinkedContentIntoAKmzArchive(kmlFile, OutputPath);
            //using Stream output = File.Create(OutputPath);
            //kmz.Save(output);
            //Console.WriteLine("Saved to '{0}'.", OutputPath);
            //Console.ReadKey();

        }
    }
}
