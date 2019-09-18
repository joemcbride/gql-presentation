using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace example
{
    [Route("api/[controller]")]
    public class CharacterController : Controller
    {
        private readonly StarWarsData _data;
        private readonly ILogger<CharacterController> _logger;

        public CharacterController(StarWarsData data, ILogger<CharacterController> logger)
        {
            _data = data;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            _logger.LogInformation($"Fetching character {id}");

            var character = await _data.GetCharacter(id);
            return Json(character);
        }
    }
}
