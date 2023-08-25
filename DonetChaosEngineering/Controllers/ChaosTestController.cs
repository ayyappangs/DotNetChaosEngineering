using DonetChaosEngineering.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DonetChaosEngineering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChaosTestController : ControllerBase
    {
        private readonly IChaosService _chaosService;

        public ChaosTestController(IChaosService chaosService)
        {
            _chaosService = chaosService;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            try
            {
                HttpResponseMessage response = await _chaosService.MakeApiCall();

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"Error: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}
