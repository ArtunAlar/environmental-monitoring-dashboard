using CarbonFootprintAPI.Models;

namespace CarbonFootprintAPI.Services
{
    /// <summary>
    /// Service responsible for calculating carbon footprint emissions
    /// Contains the business logic for CO2 calculations based on different activities
    /// </summary>
    public class CarbonCalculatorService
    {
        // Carbon emission factors (kg CO2 per unit)
        private const double AirTravelFactor = 0.115; // kg CO2 per km
        private const double RedMeatFactor = 27.0;    // kg CO2 per kg

        /// <summary>
        /// Calculates total CO2 emissions based on air travel distance and red meat consumption
        /// </summary>
        /// <param name="airTravelKm">Distance traveled by air in kilometers</param>
        /// <param name="redMeatKg">Amount of red meat consumed in kilograms</param>
        /// <returns>CarbonCalculationResponse containing total CO2 and descriptive message</returns>
        public CarbonCalculationResponse CalculateCO2(double airTravelKm, double redMeatKg)
        {
            // Calculate CO2 emissions for each activity
            double airTravelCO2 = airTravelKm * AirTravelFactor;
            double redMeatCO2 = redMeatKg * RedMeatFactor;
            
            // Calculate total CO2 emissions
            double totalCO2 = airTravelCO2 + redMeatCO2;

            // Generate descriptive message based on total emissions
            string message = GenerateMessage(totalCO2, airTravelCO2, redMeatCO2);

            return new CarbonCalculationResponse
            {
                TotalCO2 = Math.Round(totalCO2, 2),
                Message = message
            };
        }

        /// <summary>
        /// Generates a descriptive message based on CO2 emissions
        /// </summary>
        /// <param name="totalCO2">Total CO2 emissions</param>
        /// <param name="airTravelCO2">CO2 from air travel</param>
        /// <param name="redMeatCO2">CO2 from red meat consumption</param>
        /// <returns>Descriptive message about the carbon footprint</returns>
        private string GenerateMessage(double totalCO2, double airTravelCO2, double redMeatCO2)
        {
            if (totalCO2 == 0)
            {
                return "No carbon emissions calculated. Great job on having zero footprint!";
            }

            string message = $"Your activities generated {totalCO2:F2} kg of CO2 emissions. ";

            // Add breakdown
            message += $"Air travel contributed {airTravelCO2:F2} kg and red meat consumption contributed {redMeatCO2:F2} kg. ";

            // Add environmental impact assessment
            if (totalCO2 < 10)
            {
                message += "This is a relatively low carbon footprint. Keep up the eco-friendly choices!";
            }
            else if (totalCO2 < 50)
            {
                message += "This is a moderate carbon footprint. Consider reducing air travel or meat consumption to lower your impact.";
            }
            else if (totalCO2 < 100)
            {
                message += "This is a significant carbon footprint. You might want to explore more sustainable alternatives.";
            }
            else
            {
                message += "This is a high carbon footprint. Consider making substantial changes to reduce your environmental impact.";
            }

            return message;
        }
    }
}