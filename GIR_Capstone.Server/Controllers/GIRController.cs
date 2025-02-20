namespace GIR_Capstone.Server.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="GIRController" />
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GIRController : ControllerBase
    {
        /// <summary>
        /// Defines the _userRepository
        /// </summary>
        private readonly ICorporateRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GIRController"/> class.
        /// </summary>
        /// <param name="userRepository">The userRepository<see cref="ICorporateRepository"/></param>
        public GIRController(ICorporateRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// The RetrieveCorporates
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/></returns>
        [HttpGet("RetrieveCorporates")]
        public async Task<IActionResult> RetrieveCorporates()
        {
            List<CorporateDto> corporates = await _userRepository.GetAllCorporatesAsync();
            return Ok(corporates);
        }

        /// <summary>
        /// The RetrieveCorporateStructure
        /// </summary>
        /// <param name="corporateId">The corporateId<see cref="string"/></param>
        /// <returns>The <see cref="Task{IActionResult}"/></returns>
        [HttpGet("RetrieveCorporateStructure/{corporateId}")]
        public async Task<IActionResult> RetrieveCorporateStructure(string corporateId)
        {
            List<CorporateEntityDto> corporateStructure = await _userRepository.GetCorporateStructureAsync(corporateId);
            return Ok(corporateStructure);
        }

        /// <summary>
        /// The BatchCorporateStructure
        /// </summary>
        /// <param name="corporate">The corporate<see cref="CorporateRequestModel"/></param>
        /// <returns>The <see cref="Task{IActionResult}"/></returns>
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
