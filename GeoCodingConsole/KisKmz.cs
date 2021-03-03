using System;
using System.IO;
using System.Linq;
using GeoCodingLib;
using System.Collections.Generic;
using OfficeOpenXml;

namespace GeoCodingConsole
{
    public static class KisKmz
    {
        static string pathKmz = "output.kmz";
        static string pathXlsx = "test.xlsx";
        static string nameDocumentKml = "СоУдВолс";
        static List<PointSoUd> pointSoUds = new List<PointSoUd>();
        static List<StyleDoc> styleDocs = new List<StyleDoc>
        {
            new StyleDoc("svetoforGreen", "images/greenSO.png"),
            new StyleDoc("svetoforYellow", "images/yellowSO.png"),
            new StyleDoc("yellowUD", "images/yellowUD.png"),
            new StyleDoc("greenUD", "images/greenUD.png"),
        };
        public static void Run()
        {           
            FileInfo fi = new FileInfo(pathXlsx);
            using var exp = new ExcelPackage(fi);
            var dataSheet = exp.Workbook.Worksheets["Лист1"];
            int lastCells = dataSheet.Cells.Last().End.Row;

            for (int i = 2; i <= lastCells; i++)
            {
                try
                {
                    var pointSo = new PointSoUd
                    (
                        dataSheet.Cells[i, 1].Text,
                        dataSheet.Cells[i, 2].Text,
                        Double.Parse(dataSheet.Cells[i, 3].Text.Replace(",", ".")),
                        Double.Parse(dataSheet.Cells[i, 4].Text.Replace(",", ".")),
                        dataSheet.Cells[i, 8].Text,
                        "СО"
                    );
                    pointSoUds.Add(pointSo);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Ошибка данных в строке{i - 1} СО");
                }

                try
                {
                    var pointUd = new PointSoUd
                    (
                        null,
                        dataSheet.Cells[i, 5].Text,
                        Double.Parse(dataSheet.Cells[i, 6].Text.Replace(",", ".")),
                        Double.Parse(dataSheet.Cells[i, 7].Text.Replace(",", ".")),
                        dataSheet.Cells[i, 8].Text,
                        "УД"
                    );
                    pointSoUds.Add(pointUd);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Ошибка данных в строке{i - 1} УД");
                }
            }

            var kml = new KmlFileUsers(styleDocs, nameDocumentKml);
            var folderCompl = kml.AddFolder("Построено");
            var folderNo = kml.AddFolder("Запланировано");

            foreach (var point in pointSoUds)
            {
                if (point.BuildSatus == "Построено")
                {
                    string id = point.Type == "УД" ? "greenUD" : "svetoforGreen";

                    kml.AddPoint(point, id, point.DicExtDataKml(), folderCompl);
                }
                else
                {
                    string id = point.Type == "УД" ? "yellowUD" : "svetoforYellow";
                    kml.AddPoint(point, id, point.DicExtDataKml(), folderNo);
                }
            }

            kml.SaveKmz(pathKmz);           
        }
    }
}
