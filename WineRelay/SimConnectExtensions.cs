using Microsoft.FlightSimulator.SimConnect;
using WineRelay.Enums.SimConnect;
using WineRelay.Models;

namespace WineRelay;
public static class SimConnectExtensions
{
   extension (SimConnect simConnect)
   {
      public void TransmitEvent(Events @event, uint data = 0) => simConnect.TransmitClientEvent(SimConnect.SIMCONNECT_OBJECT_ID_USER, @event, data, Priority.Default, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
      public void RequestData(Definition dataDefinition) => simConnect.RequestDataOnSimObject(Request.Normal, dataDefinition, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.ONCE, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
      public void RequestDataDelayed(Definition dataDefinition) => simConnect.RequestDataOnSimObject(Request.Normal, dataDefinition, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SIM_FRAME, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 5, 0, 1);

      public void SetAltitude(int altitude) => simConnect.SetDataOnSimObject(Definition.AutopilotAltitude, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_DATA_SET_FLAG.DEFAULT, new AutopilotAltitude(altitude));
      public void SetVerticalSpeed(int verticalSpeed) => simConnect.SetDataOnSimObject(Definition.AutopilotVerticalSpeed, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_DATA_SET_FLAG.DEFAULT, new AutopilotVerticalSpeed(verticalSpeed));

      public void Initialize()
      {
         simConnect.MapClientEventToSimEvent(Events.AutopilotOn, "AUTOPILOT_ON");
         simConnect.MapClientEventToSimEvent(Events.AutopilotOff, "AUTOPILOT_OFF");
         simConnect.MapClientEventToSimEvent(Events.FlightDirectorToggle, "TOGGLE_FLIGHT_DIRECTOR");
         simConnect.MapClientEventToSimEvent(Events.FlightLevelChangeToggle, "FLIGHT_LEVEL_CHANGE");
         simConnect.MapClientEventToSimEvent(Events.VerticalSpeedSet, "AP_VS_SET");
         simConnect.MapClientEventToSimEvent(Events.AutopilotHeadingToggle, "AP_HDG_HOLD");
         simConnect.MapClientEventToSimEvent(Events.AutopilotLevelerToggle, "AP_WING_LEVELER");
         simConnect.MapClientEventToSimEvent(Events.AutopilotAltituteHoldToggle, "AP_ALT_HOLD");
         simConnect.MapClientEventToSimEvent(Events.AutopilotHeadingSet, "HEADING_BUG_SET");
         simConnect.MapClientEventToSimEvent(Events.AutopilotSpeedSet, "AP_SPD_VAR_SET");
         simConnect.MapClientEventToSimEvent(Events.AutopilotHeadingSetCurrent, "AP_HDG_CURRENT_HDG_SET");
         simConnect.MapClientEventToSimEvent(Events.AutopilotAltitudeSetCurrent, "AP_ALT_CURRENT_ALT_SET");
         simConnect.MapClientEventToSimEvent(Events.AutopilotApproachToggle, "AP_APR_HOLD");
         simConnect.MapClientEventToSimEvent(Events.AutopilotAutoThrottleToggle, "AUTO_THROTTLE_ARM");
         simConnect.MapClientEventToSimEvent(Events.AutopilotNavToggle, "AP_NAV1_HOLD");
         // simConnect.MapClientEventToSimEvent(Events.AutopilotVNavToggle, "VNAV_TOGGLE");
         simConnect.MapClientEventToSimEvent(Events.YawDamperToggle, "YAW_DAMPER_TOGGLE");

         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT ALTITUDE LOCK", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT MASTER", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT APPROACH HOLD", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOTHROTTLE ACTIVE", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT MANAGED THROTTLE ACTIVE", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT FLIGHT DIRECTOR ACTIVE", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT FLIGHT LEVEL CHANGE", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT HEADING LOCK", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT WING LEVELER", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT NAV1 LOCK", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT VERTICAL HOLD", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         // simConnect.AddToDataDefinition(Definition.AutopilotData, "GPS HAS GLIDEPATH", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT YAW DAMPER", "Bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);

         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT ALTITUDE LOCK VAR", "Feet", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT HEADING LOCK DIR", "Degrees", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT AIRSPEED HOLD VAR", "Knots", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AUTOPILOT VERTICAL HOLD VAR", "Feet/minute", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);

         simConnect.AddToDataDefinition(Definition.AutopilotData, "INDICATED ALTITUDE", "Feet", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.AddToDataDefinition(Definition.AutopilotData, "AIRSPEED INDICATED", "Knots", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);

         simConnect.RegisterDataDefineStruct<AutopilotSettings>(Definition.AutopilotData);

         simConnect.AddToDataDefinition(Definition.AutopilotAltitude, "AUTOPILOT ALTITUDE LOCK VAR", "Feet", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.RegisterDataDefineStruct<AutopilotAltitude>(Definition.AutopilotAltitude);

         simConnect.AddToDataDefinition(Definition.AutopilotVerticalSpeed, "AUTOPILOT VERTICAL HOLD VAR", "Feet/minute", SIMCONNECT_DATATYPE.FLOAT32, 0, SimConnect.SIMCONNECT_UNUSED);
         simConnect.RegisterDataDefineStruct<AutopilotVerticalSpeed>(Definition.AutopilotVerticalSpeed);


         // Longpolling request, will transmit data set in the sim
         simConnect.RequestDataOnSimObject(Request.LongPolling, Definition.AutopilotData, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0, 1, 0);
      }
   }
}
