namespace CarbonFootprintAPI.Models
{
    /// <summary>
    /// Model representing the carbon footprint calculation response
    /// Contains the calculated total CO2 emissions and descriptive message
    /// </summary>
    public class CarbonCalculationResponse
    {
        /// <summary>
        /// Total CO2 emissions in kilograms
        /// </summary>
        public double TotalCO2 { get; set; }

        /// <summary>
        /// Descriptive message about the carbon footprint
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}