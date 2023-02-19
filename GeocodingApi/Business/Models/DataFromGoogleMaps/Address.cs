using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Models.DataFromGoogleMaps
{
    public class Address
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }
}
