using System;
using System.Collections.Generic;
using System.Text;

namespace GeoCodingFram
{
    public class StyleDoc
    {
        public string Id { get; set; }
        public string HrefPoint { get; set; }
        public StyleDoc(string id, string hrefPoint)
        {
            Id = id;
            HrefPoint = hrefPoint;
        }
    }
}
