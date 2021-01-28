using TrangselskattAPI;
public interface ITaxCalculationService
{
     public TaxResult GetTaxResult(TaxRequest request);
}