using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Models
{
    public class GeocodingModel
    {
        public string FullAddress { get; set; }
        public string PlaceId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
