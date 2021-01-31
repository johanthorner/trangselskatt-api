using TrangselskattAPI;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Options;

public class TaxCalculationService : ITaxCalculationService
{   
    private int TaxFreeMonth;
    private string[] NonTaxPayingVehicles;
    private Decimal DayTaxLimit;
    private IEnumerable<TimeSpanFee> FeeTimeSpans;

    public TaxCalculationService(IConfiguration configuration)
    {         
        DayTaxLimit = Decimal.Parse(configuration["DayTaxLimit"]);
        TaxFreeMonth = int.Parse(configuration["TaxFreeMonth"]);
        NonTaxPayingVehicles = configuration
            .GetSection("NonTaxPayingVehicle")
            .GetChildren()
            .Select(x => x.Value)
            .ToArray();

        FeeTimeSpans = configuration.GetSection("FeeTimeSpans")
            .GetChildren()
            .ToList()
            .Select(x => new TimeSpanFee{
                Start = DateTime.Parse(x.GetValue<string>("Start"), System.Globalization.CultureInfo.CurrentCulture),
                End = DateTime.Parse(x.GetValue<string>("End"), System.Globalization.CultureInfo.CurrentCulture),
                Fee = decimal.Parse(x.GetValue<string>("Fee"))
            });
    }
    public TaxResult GetTaxResult(TaxRequest request){        

        return NonTaxPayingVehicles.Any(a => a == request.VehicleType) ? new TaxResult() : CreateTaxResult(request);
    }      
   
    private TaxResult CreateTaxResult(TaxRequest request){
        
        var result = new TaxResult(); 

        result.Dates = request.PassagesThroughCustoms
        .Where(a => !DateIsTaxFree(a))
        .GroupBy(a => a.Date)      
        .Select(a => 
        new DateOfPassage()
        {
            Date = a.Key.ToString("yyyy/MM/dd"),
            Occasions = a.Select(a => a.ToString("HH:mm")).ToList(),
            Tax = GetDayTax(a)
        })
        .ToList();
        result.IsTaxPayingVehicle = true;

        return result;
    }  
    private decimal GetDayTax(IGrouping<DateTime, DateTime> groupOfDates){
        
        decimal totalTax = 0;
              
        var groupByHour = groupOfDates.GroupBy(a => a.Hour);           
          
            foreach (var hour in groupByHour)
            {
                var hourFee = hour.Select(a => GetFee(a.AddSeconds(-a.Second))).OrderByDescending(a => a).First();  
                totalTax += hourFee;
            }      
           
            if(totalTax > DayTaxLimit)
            {
                totalTax = DayTaxLimit;
            }  
        
        return totalTax;
    }

    private bool DateIsTaxFree(DateTime date){

        var isTaxFreeMonth = date.Month == TaxFreeMonth;     
        var isDayBeforeWeekend = (date.DayOfWeek + 1) == DayOfWeek.Saturday;
        var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        
        return (isTaxFreeMonth|| isWeekend || isDayBeforeWeekend);
    }
   
    private decimal GetFee(DateTime date){   
 
     return FeeTimeSpans.Where(a => IsInTimeSpan(date, a.Start.TimeOfDay, a.End.TimeOfDay)).FirstOrDefault().Fee;   
    }

    private bool IsInTimeSpan(DateTime datetime, TimeSpan start, TimeSpan end){
   
    TimeSpan now = datetime.TimeOfDay;  
    if (start < end){       
        return start <= now && now <= end;
    }       

    return !(end < now && now < start);

    }

}