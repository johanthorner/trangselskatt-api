using TrangselskattAPI;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;

public class TaxCalculationService : ITaxCalculationService
{
    private readonly IConfiguration Configuration;
    private string[] NonTaxPayingVehicle;

    public TaxCalculationService(IConfiguration configuration)
    {
        Configuration = configuration;
        NonTaxPayingVehicle = configuration
            .GetSection("NonTaxPayingVehicle")
            .GetChildren()
            .Select(x => x.Value)
            .ToArray();
    }
    public TaxResult CreateTaxResultModel(TaxRequest request){
            
        var result = OrderPassagesByDate(request); 
              
        if(NonTaxPayingVehicle.Any(a => a == request.VehicleType))
        {
            return result;
        }     

        result = CalculateTaxPerDay(result);

        return result;
    } 

    private TaxResult CalculateTaxPerDay(TaxResult result){
       
        return result;
    }

    private TaxResult OrderPassagesByDate(TaxRequest request){
        
        var result = new TaxResult(); 

        result.Dates = request.PassagesThroughCustoms
        .Where(a => !DateIsTaxFree(a))
        .GroupBy(a => a.Date)      
        .Select(a => 
        new DateOfPassage()
        {
            Date = a.Key,
            PassagesThroughCustoms = a.ToList()
        })
        .ToList();

        return result;
    }  

    private bool DateIsTaxFree(DateTime date){

        var isTaxFreeMonth = date.Month == 7;     
        var isDayBeforeWeekend = (date.DayOfWeek + 1) == DayOfWeek.Saturday;
        var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Saturday;
        
        if(isTaxFreeMonth|| isWeekend || isDayBeforeWeekend)
        {
            return true;
        }

        return false;
    }
}