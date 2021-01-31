using System;
using System.Collections.Generic;

namespace TrangselskattAPI
{
    public class TaxResult
    {      
        public TaxResult()
        {
            Dates = new List<DateOfPassage>();
        }
        public bool IsTaxPayingVehicle { get; set; }
        public List<DateOfPassage> Dates { get; set; }    
    }
    public class DateOfPassage
    {  
        public string Date { get; set; }
        public List<string> Occasions { get; set; }
        public Decimal Tax { get; set; }
    }
}
