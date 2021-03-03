using System;
using System.Collections.Generic;
using System.Text;
using GeoCodingLib;
using System.Linq;


namespace GeoCodingConsole
{
    class PointSoUd : Point
    {
        public string BuildSatus { get;}
        public string Type { get; }

        public PointSoUd(string number, string adress, double lat, double lon, string buildSatus, string type)
            : base(number, adress, lat, lon)
        {
            BuildSatus = buildSatus;
            Type = type;
        }

        public Dictionary<string, string> DicExtDataKml()
        {
            var dicData = new Dictionary<string, string>
            {
                { "№ СО", Number },
                { "Адрес", Adress },
                { "Статус строительства", BuildSatus }
            };

            return dicData.Where(p => p.Value != null).ToDictionary(p=>p.Key, p=> p.Value);
        }
    }
}
