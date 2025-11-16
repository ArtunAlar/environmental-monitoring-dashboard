namespace CarbonFootprintAPI.Models
{
    public class WaterStation
    {
        public string StationId { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
    }

    public class WaterStationData
    {
        public string StationId { get; set; } = string.Empty;
        public string Parameter { get; set; } = string.Empty;
        public string ParameterName { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class StationDetailResponse
    {
        public WaterStation Station { get; set; } = new();
        public List<WaterStationData> RecentData { get; set; } = new();
        public double CurrentWaterLevel { get; set; }
        public double CurrentDischarge { get; set; }
        public string WaterLevelStatus { get; set; } = string.Empty;
        public string DischargeStatus { get; set; } = string.Empty;
    }

    public class WaterLevelStatus
    {
        public const string High = "High";
        public const string Normal = "Normal";
        public const string Low = "Low";
        public const string CriticalLow = "CriticalLow";
    }
}