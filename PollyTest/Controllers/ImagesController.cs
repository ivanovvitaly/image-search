using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PollyTest.Controllers
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
            _logger.LogDebug("1111111111111111111111111111111111111111111111111111");
            return Ok("hey");
        }
    }
}
