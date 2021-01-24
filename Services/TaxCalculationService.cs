using TrangselskattAPI;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
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
              
        if(NonTaxPayingVehicle.Any(a => a == request.VehicleType)){
            return result;
        }     

        return result;
    } 

    private TaxResult OrderPassagesByDate(TaxRequest request){
        
        var result = new TaxResult(); 

        result.Dates = request.PassagesThroughCustoms
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
}