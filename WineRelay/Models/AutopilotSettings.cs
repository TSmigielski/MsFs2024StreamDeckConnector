using System.Runtime.InteropServices;

namespace WineRelay.Models;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct AutopilotSettings
{
   public bool AutopilotMaster { get; set; }
   public bool FlightDirector { get; set; }
   public bool FlightLevelChange { get; set; }
   public bool VerticalSpeedMode { get; set; }
   public bool HeadingMode { get; set; }
   public bool LevelerMode { get; set; }
   public bool AltitudeHold { get; set; }
   public int Heading { get; set; }
   public int Altitude { get; set; }
   public int VerticalSpeed { get; set; }
   public int Speed { get; set; }
   public int IndicatedAltitude { get; set; }
   public int IndicatedSpeed { get; set; }
}
