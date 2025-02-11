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
            List<CorporateDto> corporates = await _userRepository.GetAllCorporatesAsync();
            return Ok(corporates);
        }
        
        [HttpGet("RetrieveCorporateStructure/{corporateId}")]
        public async Task<IActionResult> RetrieveCorporateStructure(string corporateId)
        {
            List<CorporateEntityDto> corporateStructure = await _userRepository.GetCorporateStructureAsync(corporateId);
            return Ok(corporateStructure);
        }
    }
}
