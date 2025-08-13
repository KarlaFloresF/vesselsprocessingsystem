using Newtonsoft.Json;

namespace VesselProcessingSystem.Models
{
    public class Vessel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
        public string Name { get; set; }
        public string VesselType { get; set; }
        public string ETA { get; set; }
        public string ETD { get; set; }
        public string Terminal { get; set; }
        public string ImportTrip { get; set; }
        public string ExportTrip { get; set; }
    }
}
