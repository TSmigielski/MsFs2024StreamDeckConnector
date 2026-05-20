using System.Runtime.InteropServices;

namespace WineRelay.Models;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct AutopilotSettings
{
   public bool AltitudeToggle { get; set; }
   public bool AutopilotMasterToggle { get; set; }
   public bool ApproachToggle { get; set; }
   public double AutoThrottleToggle { get; set; }
   public double AutoThrottleManToggle { get; set; }
   public bool FlightDirectorToggle { get; set; }
   public bool FlightLevelChangeToggle { get; set; }
   public bool HeadingToggle { get; set; }
   public bool LevelerToggle { get; set; }
   public bool NavigationToggle { get; set; }
   public bool VerticalSpeedToggle { get; set; }
   // public bool VerticalNavigationToggle { get; set; }
   public bool YawDamperToggle { get; set; }

   public int SelectedAltitude { get; set; }
   public int SelectedHeading { get; set; }
   public int SelectedSpeed { get; set; }
   public int SelectedVerticalSpeed { get; set; }

   public int IndicatedAltitude { get; set; }
   public int IndicatedSpeed { get; set; }
}
