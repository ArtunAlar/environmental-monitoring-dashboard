using Microsoft.AspNetCore.Mvc;
using CarbonFootprintAPI.Models;
using CarbonFootprintAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace CarbonFootprintAPI.Controllers
{
    /// <summary>
    /// API Controller for carbon footprint calculation endpoints
    /// Handles HTTP requests for CO2 emission calculations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CarbonCalculatorController : ControllerBase
    {
        private readonly CarbonCalculatorService _calculatorService;
        private readonly ILogger<CarbonCalculatorController> _logger;

        /// <summary>
        /// Constructor for CarbonCalculatorController
        /// </summary>
        /// <param name="calculatorService">Service for carbon footprint calculations</param>
        /// <param name="logger">Logger for error and information logging</param>
        public CarbonCalculatorController(CarbonCalculatorService calculatorService, ILogger<CarbonCalculatorController> logger)
        {
            _calculatorService = calculatorService;
            _logger = logger;
        }

        /// <summary>
        /// Calculates total CO2 emissions based on air travel distance and red meat consumption
        /// </summary>
        /// <param name="request">Request containing air travel distance and red meat consumption</param>
        /// <returns>CarbonCalculationResponse with total CO2 emissions and descriptive message</returns>
        [HttpPost("calculate")]
        [ProducesResponseType(typeof(CarbonCalculationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<CarbonCalculationResponse> Calculate([FromBody] CarbonCalculationRequest request)
        {
            try
            {
                // Validate the request model
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for carbon calculation request");
                    return ValidationProblem(ModelState);
                }

                // Validate individual values
                if (request.AirTravelKm < 0)
                {
                    ModelState.AddModelError(nameof(request.AirTravelKm), "Air travel distance cannot be negative");
                    return ValidationProblem(ModelState);
                }

                if (request.RedMeatKg < 0)
                {
                    ModelState.AddModelError(nameof(request.RedMeatKg), "Red meat consumption cannot be negative");
                    return ValidationProblem(ModelState);
                }

                _logger.LogInformation("Calculating CO2 emissions for AirTravelKm: {AirTravelKm}, RedMeatKg: {RedMeatKg}", 
                    request.AirTravelKm, request.RedMeatKg);

                // Perform the calculation
                var result = _calculatorService.CalculateCO2(request.AirTravelKm, request.RedMeatKg);

                _logger.LogInformation("CO2 calculation completed successfully. Total CO2: {TotalCO2}", result.TotalCO2);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided for CO2 calculation");
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during CO2 calculation");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred while calculating CO2 emissions",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        /// <summary>
        /// Health check endpoint to verify API is running
        /// </summary>
        /// <returns>Simple health status</returns>
        [HttpGet("health")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public ActionResult<object> Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}