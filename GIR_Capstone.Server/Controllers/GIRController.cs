using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
        
        [HttpGet("RetrieveCorporateStructure/{corporateId}/{xmlParse?}")]
        public async Task<IActionResult> RetrieveCorporateStructure(string corporateId, bool xmlParse = false)
        {
            if(xmlParse)
            {
                List<CorporateEntityDto> corporateStructureXml = await _userRepository.GetCorporateStructureXmlAsync(corporateId);
                return Ok(corporateStructureXml);
            }

            List<CorporateEntityDto> corporateStructure = await _userRepository.GetCorporateStructureDbAsync(corporateId);
            return Ok(corporateStructure);
        }
        
        [HttpPost("BatchCorporateStructure")]
        public async Task<IActionResult> BatchCorporateStructure([FromBody] CorporateRequestModel corporate)
        {
            if (string.IsNullOrEmpty(corporate.CorporateId))
            {
                return BadRequest("Corporate ID is required.");
            }

            bool success = await _userRepository.BatchUpdateCorporateStructureAsync(corporate.CorporateId);

            if (!success)
            {
                return StatusCode(500, "Error processing the batch update.");
            }

            return Ok(new { Message = "Corporate structure batch update successful" });
        }
    }
}
