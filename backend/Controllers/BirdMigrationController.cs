using System;
using System.Threading.Tasks;
using CarbonFootprintAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CarbonFootprintAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BirdMigrationController : ControllerBase
    {
        private readonly BirdMigrationService _birdService;
        private readonly ILogger<BirdMigrationController> _logger;

        public BirdMigrationController(BirdMigrationService birdService, ILogger<BirdMigrationController> logger)
        {
            _birdService = birdService;
            _logger = logger;
        }

        [HttpGet("observations")]
        public async Task<IActionResult> GetObservations(
            [FromQuery] string region = "CA-AB",
            [FromQuery] string? species = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int maxResults = 200)
        {
            try
            {
                _logger.LogInformation($"Fetching bird observations for region: {region}, species: {species ?? "all"}");
                
                var result = await _birdService.GetObservationsAsync(region, species, startDate, endDate, maxResults);
                
                if (result == null || result.Observations.Count == 0)
                {
                    return NotFound("No bird observations found for the specified criteria.");
                }
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching bird observations for region: {region}");
                return StatusCode(500, "An error occurred while fetching bird observations.");
            }
        }

        [HttpGet("migration-routes")]
        public async Task<IActionResult> GetMigrationRoutes(
            [FromQuery] string region = "CA-AB",
            [FromQuery] string species = "",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(species))
                {
                    return BadRequest("Species parameter is required for migration routes.");
                }

                _logger.LogInformation($"Fetching migration routes for {species} in {region}");
                
                var start = startDate ?? DateTime.Now.AddDays(-30);
                var end = endDate ?? DateTime.Now;
                
                var routes = await _birdService.GetMigrationRoutesAsync(region, species, start, end);
                
                if (routes == null || routes.Count == 0)
                {
                    return NotFound($"No migration routes found for species: {species}");
                }
                
                return Ok(routes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching migration routes for species: {species}");
                return StatusCode(500, "An error occurred while fetching migration routes.");
            }
        }

        [HttpGet("species")]
        public async Task<IActionResult> GetSpeciesInRegion(
            [FromQuery] string region = "CA-AB",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation($"Fetching species list for region: {region}");
                
                var observations = await _birdService.GetObservationsAsync(region, null, startDate, endDate, 500);
                
                if (observations == null || observations.Species.Count == 0)
                {
                    return NotFound("No species found for the specified region.");
                }
                
                return Ok(observations.Species);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching species for region: {region}");
                return StatusCode(500, "An error occurred while fetching species list.");
            }
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentObservations(
            [FromQuery] string region = "CA-AB",
            [FromQuery] int daysBack = 7,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var endDate = DateTime.Now;
                var startDate = endDate.AddDays(-daysBack);
                
                _logger.LogInformation($"Fetching recent bird observations for last {daysBack} days in {region}");
                
                var result = await _birdService.GetObservationsAsync(region, null, startDate, endDate, maxResults);
                
                if (result == null || result.Observations.Count == 0)
                {
                    return NotFound("No recent bird observations found.");
                }
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching recent observations for region: {region}");
                return StatusCode(500, "An error occurred while fetching recent observations.");
            }
        }
    }
}