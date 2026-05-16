namespace WineRelay.Enums.SimConnect;

public enum Events
{
   AutopilotOn,
   AutopilotOff,
   FlightDirectorToggle,
   FlightLevelChangeToggle,

   /// <summary>
   /// Accepts a bool for enable/disable.
   /// </summary>
   VerticalSpeedSet,
   AutopilotHeadingToggle,
   AutopilotLevelerToggle,
   AutopilotAltituteHoldToggle,

   /// <summary>
   /// Accepts an integer.
   /// </summary>
   AutopilotHeadingSet,

   /// <summary>
   /// Accepts an integer.
   /// </summary>
   AutopilotSpeedSet,
   AutopilotHeadingSetCurrent,
   AutopilotAltitudeSetCurrent,

   AutopilotApproachToggle,
   AutopilotAutoThrottleToggle,
   AutopilotNavToggle,
   AutopilotVNavToggle,
}
