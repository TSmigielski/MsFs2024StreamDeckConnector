using System.Text.Json.Serialization;
using WineRelay.Enums.Udp;

namespace WineRelay.Models;

public record ToggleActionModel
{
   [JsonConverter(typeof(JsonStringEnumConverter))]
   public required ToggleAction Toggle { get; set; }
   public required bool State { get; set; }
}
