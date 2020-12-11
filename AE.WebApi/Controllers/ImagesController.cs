using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AE.WebApi
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(ILogger<ImagesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("hey");
        }
    }
}
