using Microsoft.AspNetCore.Mvc;
using VesselProcessingSystem.Importers;
using VesselProcessingSystem.Models;
using VesselProcessingSystem.Services;


namespace VesselProcessingSystem.Controllers
{
    public class VesselController : Controller
    {
        private readonly ILogger<VesselController> _logger;
        private readonly ICruseGateHHImporter _cruseGateImporter;
        private readonly ICosmosDbService _cosmosDbService;

        public VesselController(
            ICruseGateHHImporter cruseGateImporter,
            ILogger<VesselController> logger,
            ICosmosDbService cosmosDbService)
        {
            _logger = logger;
            _cruseGateImporter = cruseGateImporter;
            _cosmosDbService = cosmosDbService;
        }

        /// <summary>
        /// Imports vessel data from HHLA Excel file.
        /// </summary>
        /// <returns>Returns the number of vessels imported.</returns>
        [HttpPost("import/hhla")]
        public async Task<IActionResult> ImportVesselsFromHHLA()
        {
            try
            {
                var vessels = _cruseGateImporter.GetVesselsList();
                
                if (vessels == null || !vessels.Any())
                {
                    return BadRequest(new
                    {
                        Message = "No vessels found in the Excel file.",
                    });
                }

                foreach (var vessel in vessels)
                {
                    vessel.Id = Guid.NewGuid().ToString();
                    vessel.PartitionKey = Guid.NewGuid().ToString();
                    await _cosmosDbService.AddItemAsync(vessel, vessel.PartitionKey);
                }

                return Ok(new
                {
                    vessels.Count
                });
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "Excel file not found.");
                return BadRequest(new
                {
                    Message = "Excel file not found."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while importing vessels.");
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred."
                });
            }
        }

        /// <summary>
        /// Getting all the vessels
        /// </summary>
        /// <returns>Returns all vessels</returns>
        [HttpGet("vessels")]
        public async Task<IActionResult> GetAllVessels()
        {
            try
            {
                var vessels = await _cosmosDbService.GetAllItemsAsync<Vessel>();

                if (vessels == null || !vessels.Any())
                {
                    return BadRequest(new
                    {
                        Message = "No vessels found in database.",
                    });
                }

                return Ok(vessels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting vessels.");
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred."
                });
            }
        }

        /// <summary>
        /// Getting vessels by Id
        /// </summary>
        /// <returns>Returns vessels by Id</returns>
        [HttpGet("vessels/by-id/{id}")]
        public async Task<IActionResult> GetVesselById(string id)
        {
            try
            {
                var vessel = await _cosmosDbService.GetItemByIdAsync<Vessel>(id);

                if (vessel == null)
                    return NotFound($"Vessel with id '{id}' not found.");

                return Ok(vessel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while getting vessel with id {id}.");
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred."
                });
            }
        }

    }
}
