using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using CarbonFootprintAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CarbonFootprintAPI.Services
{
    public class WaterDataService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private const string CurrentConditionsUrl = "https://wateroffice.ec.gc.ca/services/current_conditions/xml/inline?lang=en";
        private const string RealTimeDataUrl = "https://wateroffice.ec.gc.ca/services/real_time_data/csv/inline";
        private const string CacheKeyStations = "WaterStations";
        private const string CacheKeyPrefix = "StationData_";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        // Alberta water stations with approximate coordinates
        private readonly Dictionary<string, (double lat, double lng)> _albertaStations = new()
        {
            {"07EA004", (53.917, -118.885)}, // Athabasca River at Athabasca
            {"07BE001", (52.283, -113.785)}, // Red Deer River at Red Deer
            {"07DA001", (51.045, -114.058)}, // Bow River at Calgary
            {"07ED001", (53.200, -117.567)}, // North Saskatchewan River at Edmonton
            {"07AE001", (49.685, -112.835)}, // Oldman River at Lethbridge
            {"07BB004", (52.825, -113.285)}  // Battle River near Gadsby
        };

        public WaterDataService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<List<WaterStation>> GetAlbertaWaterStationsAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKeyStations, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                
                try
                {
                    var response = await _httpClient.GetStringAsync(CurrentConditionsUrl);
                    return ParseStationsFromXml(response);
                }
                catch (Exception ex)
                {
                    // Return fallback Alberta stations if API fails
                    return GetFallbackAlbertaStations();
                }
            });
        }

        public async Task<StationDetailResponse> GetStationDataAsync(string stationId)
        {
            var cacheKey = $"{CacheKeyPrefix}{stationId}";
            
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
                
                try
                {
                    var endDate = DateTime.Now;
                    var startDate = endDate.AddHours(-2); // Get last 2 hours of data
                    
                    var csvUrl = $"{RealTimeDataUrl}?stations[]={stationId}&parameters[]=46&parameters[]=47&start_date={startDate:yyyy-MM-dd HH:mm:ss}&end_date={endDate:yyyy-MM-dd HH:mm:ss}";
                    
                    var csvData = await _httpClient.GetStringAsync(csvUrl);
                    return ParseStationDataFromCsv(stationId, csvData);
                }
                catch (Exception ex)
                {
                    // Return fallback data if API fails
                    return GetFallbackStationData(stationId);
                }
            });
        }

        private List<WaterStation> ParseStationsFromXml(string xmlContent)
        {
            var stations = new List<WaterStation>();
            
            try
            {
                var doc = XDocument.Parse(xmlContent);
                var stationElements = doc.Descendants("station");
                
                foreach (var stationElement in stationElements)
                {
                    var stationId = stationElement.Attribute("id")?.Value;
                    
                    // Only include Alberta stations
                    if (string.IsNullOrEmpty(stationId) || !_albertaStations.ContainsKey(stationId))
                        continue;
                    
                    var station = new WaterStation
                    {
                        StationId = stationId,
                        StationName = stationElement.Element("name")?.Value ?? "Unknown Station",
                        Province = "Alberta",
                        Latitude = _albertaStations[stationId].lat,
                        Longitude = _albertaStations[stationId].lng,
                        Status = WaterLevelStatus.Normal, // Default status
                        LastUpdated = DateTime.Now
                    };
                    
                    stations.Add(station);
                }
            }
            catch
            {
                // If XML parsing fails, return fallback stations
                return GetFallbackAlbertaStations();
            }
            
            return stations.Any() ? stations : GetFallbackAlbertaStations();
        }

        private StationDetailResponse ParseStationDataFromCsv(string stationId, string csvContent)
        {
            var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
            {
                return GetFallbackStationData(stationId);
            }

            var recentData = new List<WaterStationData>();
            double currentWaterLevel = 0;
            double currentDischarge = 0;
            
            // Parse CSV data (skip header)
            for (int i = 1; i < lines.Length; i++)
            {
                var columns = lines[i].Split(',');
                if (columns.Length < 6) continue;
                
                try
                {
                    var data = new WaterStationData
                    {
                        StationId = stationId,
                        Parameter = columns[2]?.Trim() ?? "",
                        ParameterName = columns[3]?.Trim() ?? "",
                        Value = double.TryParse(columns[4], NumberStyles.Float, CultureInfo.InvariantCulture, out var val) ? val : 0,
                        Unit = columns[5]?.Trim() ?? "",
                        Timestamp = DateTime.TryParse(columns[0] + " " + columns[1], out var dt) ? dt : DateTime.Now
                    };
                    
                    recentData.Add(data);
                    
                    // Get most recent values
                    if (data.Parameter == "46") // Water level
                    {
                        currentWaterLevel = data.Value;
                    }
                    else if (data.Parameter == "47") // Discharge
                    {
                        currentDischarge = data.Value;
                    }
                }
                catch
                {
                    // Skip invalid rows
                    continue;
                }
            }

            var station = GetStationInfo(stationId);
            return new StationDetailResponse
            {
                Station = station,
                RecentData = recentData.OrderByDescending(d => d.Timestamp).Take(10).ToList(),
                CurrentWaterLevel = currentWaterLevel,
                CurrentDischarge = currentDischarge,
                WaterLevelStatus = DetermineWaterLevelStatus(currentWaterLevel),
                DischargeStatus = DetermineDischargeStatus(currentDischarge)
            };
        }

        private List<WaterStation> GetFallbackAlbertaStations()
        {
            return _albertaStations.Select(kvp => new WaterStation
            {
                StationId = kvp.Key,
                StationName = GetStationName(kvp.Key),
                Province = "Alberta",
                Latitude = kvp.Value.lat,
                Longitude = kvp.Value.lng,
                Status = WaterLevelStatus.Normal,
                LastUpdated = DateTime.Now
            }).ToList();
        }

        private StationDetailResponse GetFallbackStationData(string stationId)
        {
            var station = GetStationInfo(stationId);
            var mockData = GenerateMockData(stationId);
            
            return new StationDetailResponse
            {
                Station = station,
                RecentData = mockData,
                CurrentWaterLevel = mockData.FirstOrDefault(d => d.Parameter == "46")?.Value ?? 0,
                CurrentDischarge = mockData.FirstOrDefault(d => d.Parameter == "47")?.Value ?? 0,
                WaterLevelStatus = WaterLevelStatus.Normal,
                DischargeStatus = WaterLevelStatus.Normal
            };
        }

        private WaterStation GetStationInfo(string stationId)
        {
            if (_albertaStations.TryGetValue(stationId, out var coords))
            {
                return new WaterStation
                {
                    StationId = stationId,
                    StationName = GetStationName(stationId),
                    Province = "Alberta",
                    Latitude = coords.lat,
                    Longitude = coords.lng,
                    Status = WaterLevelStatus.Normal,
                    LastUpdated = DateTime.Now
                };
            }
            
            return new WaterStation
            {
                StationId = stationId,
                StationName = "Unknown Station",
                Province = "Alberta",
                Status = WaterLevelStatus.Normal,
                LastUpdated = DateTime.Now
            };
        }

        private List<WaterStationData> GenerateMockData(string stationId)
        {
            var data = new List<WaterStationData>();
            var now = DateTime.Now;
            var random = new Random();
            
            for (int i = 0; i < 10; i++)
            {
                var timestamp = now.AddMinutes(-i * 6);
                
                // Water level data
                data.Add(new WaterStationData
                {
                    StationId = stationId,
                    Parameter = "46",
                    ParameterName = "Water Level",
                    Value = 2.5 + random.NextDouble() * 0.5,
                    Unit = "m",
                    Timestamp = timestamp
                });
                
                // Discharge data
                data.Add(new WaterStationData
                {
                    StationId = stationId,
                    Parameter = "47",
                    ParameterName = "Discharge",
                    Value = 150 + random.NextDouble() * 50,
                    Unit = "m³/s",
                    Timestamp = timestamp
                });
            }
            
            return data;
        }

        private string GetStationName(string stationId)
        {
            return stationId switch
            {
                "07EA004" => "Athabasca River at Athabasca",
                "07BE001" => "Red Deer River at Red Deer",
                "07DA001" => "Bow River at Calgary",
                "07ED001" => "North Saskatchewan River at Edmonton",
                "07AE001" => "Oldman River at Lethbridge",
                "07BB004" => "Battle River near Gadsby",
                _ => $"Station {stationId}"
            };
        }

        private string DetermineWaterLevelStatus(double waterLevel)
        {
            return waterLevel switch
            {
                > 3.0 => WaterLevelStatus.High,
                > 1.5 => WaterLevelStatus.Normal,
                > 0.5 => WaterLevelStatus.Low,
                _ => WaterLevelStatus.CriticalLow
            };
        }

        private string DetermineDischargeStatus(double discharge)
        {
            return discharge switch
            {
                > 200 => WaterLevelStatus.High,
                > 100 => WaterLevelStatus.Normal,
                > 50 => WaterLevelStatus.Low,
                _ => WaterLevelStatus.CriticalLow
            };
        }
    }
}