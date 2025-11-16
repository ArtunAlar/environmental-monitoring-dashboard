using Microsoft.AspNetCore.Mvc;
using CarbonFootprintAPI.Services;
using CarbonFootprintAPI.Models;

namespace CarbonFootprintAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WaterController : ControllerBase
    {
        private readonly WaterDataService _waterDataService;
        private readonly ILogger<WaterController> _logger;

        public WaterController(WaterDataService waterDataService, ILogger<WaterController> logger)
        {
            _waterDataService = waterDataService;
            _logger = logger;
        }

        [HttpGet("stations")]
        public async Task<ActionResult<List<WaterStation>>> GetStations()
        {
            try
            {
                var stations = await _waterDataService.GetAlbertaWaterStationsAsync();
                return Ok(stations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving water stations");
                return StatusCode(500, new { error = "Failed to retrieve water stations" });
            }
        }

        [HttpGet("station/{id}")]
        public async Task<ActionResult<StationDetailResponse>> GetStationData(string id)
        {
            try
            {
                var stationData = await _waterDataService.GetStationDataAsync(id);
                return Ok(stationData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving data for station {id}");
                return StatusCode(500, new { error = $"Failed to retrieve data for station {id}" });
            }
        }
    }
}