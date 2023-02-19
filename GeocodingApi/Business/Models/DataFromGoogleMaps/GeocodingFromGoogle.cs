using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Models.DataFromGoogleMaps
{
    public class GeocodingFromGoogle
    {
        public IEnumerable<Address> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
    }
}
