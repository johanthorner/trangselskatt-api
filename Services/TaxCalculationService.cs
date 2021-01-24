using TrangselskattAPI;
using System.Linq;
public class TaxCalculationService : ITaxCalculationService
{
    public TaxResult CreateTaxResultModel(TaxRequest request){
            
        var result = OrderPassagesByDate(request);      
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