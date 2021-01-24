using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TrangselskattAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaxCalucaulatorController : ControllerBase
    {     
        private readonly ILogger<TaxCalucaulatorController> _logger;
        private ITaxCalculationService _taxCalculationService;

        public TaxCalucaulatorController(ILogger<TaxCalucaulatorController> logger, ITaxCalculationService taxCalculationService)
        {
            _logger = logger;
            _taxCalculationService = taxCalculationService;
        }

        [HttpGet]
        public TaxResult Get(TaxRequest request)
        {          
            return _taxCalculationService.CreateTaxResultModel(request); 
        }
    }
}
