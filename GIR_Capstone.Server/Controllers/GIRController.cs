using Microsoft.AspNetCore.Mvc;

namespace GIR_Capstone.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GIRController : ControllerBase
    {
        private readonly ICorporateRepository _userRepository;

        public GIRController(ICorporateRepository userRepository)
        {
           _userRepository = userRepository;
        }

        [HttpGet("RetrieveCorporates")]
        public async Task<IActionResult> RetrieveCorporates()
        {
            List<Corporate> corporates = await _userRepository.GetAllCorporatesAsync();
            return Ok(corporates);
        }
    }
}
