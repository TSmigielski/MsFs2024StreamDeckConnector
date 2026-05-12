using Microsoft.FlightSimulator.SimConnect;
using WineRelay.Enums;

namespace WineRelay;
public static class SimConnectExtensions
{
   extension (SimConnect simConnect)
   {
      public void TransmitEvent(Events @event) => simConnect.TransmitClientEvent(SimConnect.SIMCONNECT_OBJECT_ID_USER, @event, 0, Priority.Default, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);

      public void Initialize()
      {
         simConnect.MapClientEventToSimEvent(Events.AutopilotOn, "AUTOPILOT_ON");
         simConnect.MapClientEventToSimEvent(Events.AutopilotOff, "AUTOPILOT_OFF");
      }
   }
}
