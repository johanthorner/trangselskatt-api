using TrangselskattAPI;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Options;

public class TaxCalculationService : ITaxCalculationService
{   
    private string[] NonTaxPayingVehicle;
    private Decimal DayTaxLimit;
    private IEnumerable<TimeSpanFee> TimeSpans;

    public TaxCalculationService(IConfiguration configuration)
    {         
        DayTaxLimit = Decimal.Parse(configuration["DayTaxLimit"]);
        NonTaxPayingVehicle = configuration
            .GetSection("NonTaxPayingVehicle")
            .GetChildren()
            .Select(x => x.Value)
            .ToArray();

        TimeSpans = configuration.GetSection("TimeSpan")
            .GetChildren()
            .ToList()
            .Select(x => new TimeSpanFee{
                Start = DateTime.Parse(x.GetValue<string>("Start"), System.Globalization.CultureInfo.CurrentCulture),
                End = DateTime.Parse(x.GetValue<string>("End"), System.Globalization.CultureInfo.CurrentCulture),
                Fee = decimal.Parse(x.GetValue<string>("Fee"))
            });
    }
    public TaxResult GetTaxResult(TaxRequest request){        

        return NonTaxPayingVehicle.Any(a => a == request.VehicleType) ? new TaxResult() : CreateTaxResult(request);
    }      
   
    private TaxResult CreateTaxResult(TaxRequest request){
        
        var result = new TaxResult(); 

        result.Dates = request.PassagesThroughCustoms
        .Where(a => !DateIsTaxFree(a))
        .GroupBy(a => a.Date)      
        .Select(a => 
        new DateOfPassage()
        {
            Date = a.Key,
            PassagesThroughCustoms = a.ToList(),
            Tax = GetDayTax(a)
        })
        .ToList();

        return result;
    }  
    private decimal GetDayTax(IGrouping<DateTime, DateTime> groupOfDates){
        
        decimal totalTax = 0;
        var groupByHour = groupOfDates.GroupBy(a => a.Hour);           
          
            foreach (var hour in groupByHour)
            {
                var hourFee = hour.Select(a => GetFee(a)).OrderByDescending(a => a).First();  
                totalTax += hourFee;
            }      
           
            if(totalTax > DayTaxLimit)
            {
                totalTax = DayTaxLimit;
            }  
        
        return totalTax;
    }

    private bool DateIsTaxFree(DateTime date){

        var isTaxFreeMonth = date.Month == 7;     
        var isDayBeforeWeekend = (date.DayOfWeek + 1) == DayOfWeek.Saturday;
        var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Saturday;
        
        return (isTaxFreeMonth|| isWeekend || isDayBeforeWeekend);
    }

    private decimal GetFee(DateTime date){
     
     foreach (var timeSpan in TimeSpans)
     {       
         if(IsInTimeSpan(date, timeSpan.Start.TimeOfDay, timeSpan.End.TimeOfDay)){
             return timeSpan.Fee;
         }   
     }    

     return 0;   
    }

    private bool IsInTimeSpan(DateTime datetime, TimeSpan start, TimeSpan end){
   
    TimeSpan now = datetime.TimeOfDay;  
    if (start < end){       
        return start <= now && now <= end;
    }       

    return !(end < now && now < start);

    }

}