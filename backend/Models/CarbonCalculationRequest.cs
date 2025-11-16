using System.ComponentModel.DataAnnotations;

namespace CarbonFootprintAPI.Models
{
    /// <summary>
    /// Model representing the carbon footprint calculation request
    /// Contains input parameters for air travel distance and red meat consumption
    /// </summary>
    public class CarbonCalculationRequest
    {
        /// <summary>
        /// Air travel distance in kilometers
        /// Must be non-negative
        /// </summary>
        [Required(ErrorMessage = "Air travel distance is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Air travel distance must be non-negative")]
        public double AirTravelKm { get; set; }

        /// <summary>
        /// Red meat consumption in kilograms
        /// Must be non-negative
        /// </summary>
        [Required(ErrorMessage = "Red meat consumption is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Red meat consumption must be non-negative")]
        public double RedMeatKg { get; set; }
    }
}