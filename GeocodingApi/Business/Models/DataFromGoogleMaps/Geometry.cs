using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Models.DataFromGoogleMaps
{
    public class Geometry
    {
        public string location_type { get; set; }
        public Dictionary<string, float> location { get; set; }
    }
}
