using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TrangselskattAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaxCalucaulatorController : ControllerBase
    {     
        private readonly ILogger<TaxCalucaulatorController> _logger;

        public TaxCalucaulatorController(ILogger<TaxCalucaulatorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public TaxResult Get(TaxRequest request)
        {     
            var model = new TaxResult();  
            return model;
        }
    }
}
