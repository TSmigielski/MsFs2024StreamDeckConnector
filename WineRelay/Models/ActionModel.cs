using System.Text.Json.Serialization;
using WineRelay.Enums.Udp;

namespace WineRelay.Models;

public record ActionModel
{
   [JsonConverter(typeof(JsonStringEnumConverter))]
   public Toggle? Toggle { get; set; }
   public bool? DesiredState { get; set; }

   [JsonConverter(typeof(JsonStringEnumConverter))]
   public Dial? Dial { get; set; }
   public int? DialValue { get; set; }
}
