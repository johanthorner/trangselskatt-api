using System;
using System.Collections.Generic;

namespace TrangselskattAPI
{
    public class TaxRequest
    {
        public string VehicleType { get; set; }
        public List<DateTime> PassagesThroughCustoms { get; set; }       
        
    }
}
