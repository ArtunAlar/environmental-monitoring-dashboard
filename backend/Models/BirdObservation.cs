using System;
using System.Collections.Generic;

namespace CarbonFootprintAPI.Models
{
    public class BirdObservation
    {
        public string SpeciesCode { get; set; } = string.Empty;
        public string CommonName { get; set; } = string.Empty;
        public string ScientificName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime ObservationDate { get; set; }
        public int Count { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string RegionCode { get; set; } = string.Empty;
        public string ObserverId { get; set; } = string.Empty;
        public bool HasMedia { get; set; }
        public bool Approved { get; set; }
        public string ChecklistId { get; set; } = string.Empty;
    }

    public class BirdSpecies
    {
        public string SpeciesCode { get; set; } = string.Empty;
        public string CommonName { get; set; } = string.Empty;
        public string ScientificName { get; set; } = string.Empty;
        public string Family { get; set; } = string.Empty;
        public string Order { get; set; } = string.Empty;
    }

    public class BirdMigrationResponse
    {
        public List<BirdObservation> Observations { get; set; } = new();
        public List<BirdSpecies> Species { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string RegionCode { get; set; } = string.Empty;
        public int TotalObservations { get; set; }
        public int TotalSpecies { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class MigrationRouteData
    {
        public string SpeciesCode { get; set; } = string.Empty;
        public string CommonName { get; set; } = string.Empty;
        public List<ObservationPoint> Points { get; set; } = new();
        public DateTime FirstObservation { get; set; }
        public DateTime LastObservation { get; set; }
        public int TotalCount { get; set; }
    }

    public class ObservationPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public string LocationName { get; set; } = string.Empty;
    }
}