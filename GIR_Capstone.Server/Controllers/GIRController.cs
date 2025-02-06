using GIR_Capstone.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace GIR_Capstone.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GIRController : ControllerBase
    {
        private readonly ICorporateRepository _userRepository;

        public GIRController(ICorporateRepository userRepository)
        {
           _userRepository = userRepository;
        }

        [HttpGet(Name = "RetrieveCorporates")]
        public IActionResult RetrieveCorporates()
        {
            List<Corporate> corporates = _userRepository.GetAllCorporates();
            return Ok(corporates);
        }
    }
}
