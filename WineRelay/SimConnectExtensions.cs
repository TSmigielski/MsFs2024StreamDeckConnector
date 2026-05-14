using Microsoft.FlightSimulator.SimConnect;
using WineRelay.Enums.SimConnect;
using WineRelay.Models;

namespace WineRelay;
public static class SimConnectExtensions
{
   extension (SimConnect simConnect)
   {
      public void TransmitEvent(Events @event, uint data = 0) => simConnect.TransmitClientEvent(SimConnect.SIMCONNECT_OBJECT_ID_USER, @event, data, Priority.Default, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
      public void RequestData(Definition dataDefinition) => simConnect.RequestDataOnSimObject(Request.Request1, dataDefinition, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SIM_FRAME, SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 2, 0, 1);

      public void Initialize()
      {
         simConnect.MapClientEventToSimEvent(Events.AutopilotOn, "AUTOPILOT_ON");
         simConnect.MapClientEventToSimEvent(Events.AutopilotOff, "AUTOPILOT_OFF");
         simConnect.MapClientEventToSimEvent(Events.FlightDirectorToggle, "TOGGLE_FLIGHT_DIRECTOR");
         simConnect.MapClientEventToSimEvent(Events.FlightLevelChangeToggle, "FLIGHT_LEVEL_CHANGE");
         simConnect.MapClientEventToSimEvent(Events.VerticalSpeedSet, "AP_VS_SET");

         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT MASTER", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT FLIGHT DIRECTOR ACTIVE", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT FLIGHT LEVEL CHANGE", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT VERTICAL HOLD", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         // simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT DEFAULT PITCH MODE", "Enum", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         // simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT DEFAULT ROLL MODE", "Enum", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);

         simConnect.RegisterDataDefineStruct<AutopilotSettings>(Definition.AutopilotData);
      }
   }
}
