using System.Threading.Tasks;
using AE.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AE.WebApi
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<ImagesController> _logger;
        private readonly IImagesService imagesService;

        public ImagesController(
            ILogger<ImagesController> logger,
            IImagesService imagesService)
        {
            this.imagesService = imagesService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetImages([FromQuery] int? page = null)
        {
            return Ok(await imagesService.GetImages(page));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(string id)
        {
            return Ok(await imagesService.GetImage(id));
        }

        [HttpGet("/search/{term}")]
        public IActionResult SearchImages(string term)
        {
            return Ok(imagesService.Search(term));
        }
    }
}