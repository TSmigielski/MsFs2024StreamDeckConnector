using System.Runtime.InteropServices;
using WineRelay.Enums.Aircraft;

namespace WineRelay.Models;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct AutopilotSettings
{
   public bool AutopilotMaster { get; set; }
   public bool FlightDirector { get; set; }
   public bool FlightLevelChange { get; set; }
   public bool VerticalSpeed { get; set; }
   // public RollMode RollMode { get; set; }
}
