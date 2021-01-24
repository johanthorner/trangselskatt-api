using TrangselskattAPI;
public interface ITaxCalculationService
{
     public TaxResult CreateTaxResultModel(TaxRequest request);
}