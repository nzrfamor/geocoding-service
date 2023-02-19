using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Models.DataFromGoogleMaps
{
    public class Result
    {
        public IEnumerable<GeocodingFromGoogle> results { get; set; }
    }
}
