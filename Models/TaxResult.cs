using System;
using System.Collections.Generic;

namespace TrangselskattAPI
{
    public class TaxResult
    {
      
        public string Type { get; set; }
        public List<DateOfPassage> Dates { get; set; }       
        public Decimal TotalTax { get; set; }
    }
    public class DateOfPassage
    {  
        public DateTime Date { get; set; }
        public List<DateTime> PassagesThroughCustoms { get; set; }
        public Decimal Tax { get; set; }
    }
}
