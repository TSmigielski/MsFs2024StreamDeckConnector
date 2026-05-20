using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.FlightSimulator.SimConnect;
using WineRelay;
using WineRelay.Enums.SimConnect;
using WineRelay.Enums.Udp;
using WineRelay.Models;

var debug = false;
AutopilotSettings lastAutopilotSettings = default;

var jsonSettings = new JsonSerializerOptions
{
   IncludeFields = true,
   WriteIndented = true
};

IPEndPoint? remote = null;
using var udp = new UdpClient(new IPEndPoint(IPAddress.Loopback, 13337));
using var simConnect = new SimConnect("MSFS StreamDeck Connector", 0, 0x0402, null, 0);
simConnect.Initialize();

var udpLoopTask = UdpLoop();

while (true)
{
   simConnect.ReceiveDispatch(DispatchedEventHandler);
   await Task.Delay(50);
}

void DispatchedEventHandler(SIMCONNECT_RECV pData, uint cbData)
{
   if (remote == null)
      return;

   var @event = (SIMCONNECT_RECV_ID)pData.dwID;
   switch (@event)
   {
      case SIMCONNECT_RECV_ID.EXCEPTION:
         var exception = (SIMCONNECT_RECV_EXCEPTION)pData;
         Console.WriteLine(JsonSerializer.Serialize(exception, jsonSettings));
         return;

      case SIMCONNECT_RECV_ID.SIMOBJECT_DATA:
         var simObject = (SIMCONNECT_RECV_SIMOBJECT_DATA)pData;
         Console.WriteLine(JsonSerializer.Serialize(simObject, jsonSettings));
         switch ((Definition)simObject.dwDefineID)
         {
            case Definition.AutopilotData:
               if (debug)
                  Console.WriteLine($"Sending to: {remote.Address}:{remote.Port}");

               lastAutopilotSettings = (AutopilotSettings)simObject.dwData[0];
               udp.Send(JsonSerializer.SerializeToUtf8Bytes(lastAutopilotSettings), remote);
               return;

            default:
               Console.WriteLine($"Unhandled SimObjectEvent!");
               return;
         }

      default:
         Console.WriteLine($"Unhandled event `{pData.dwID}|{@event:G}`:" + JsonSerializer.Serialize(pData, jsonSettings));
         return;
   }
}

async Task UdpLoop()
{
   while (true)
   {
      var response = await udp.ReceiveAsync();
      remote = response.RemoteEndPoint;
      ActionModel? action;

      try
      {
         action = JsonSerializer.Deserialize<ActionModel>(response.Buffer);
         if (action == null)
            return;
      }
      catch (Exception ex)
      {
         Console.WriteLine("==============================================================");
         Console.WriteLine(Encoding.UTF8.GetString(response.Buffer));
         Console.WriteLine("==============================================================");
         Console.WriteLine(ex);
         simConnect.RequestData(Definition.AutopilotData);
         continue;
      }

      if (debug)
         Console.WriteLine($"Received an Action:{Environment.NewLine}{JsonSerializer.Serialize(action)}");

      if (action.Toggle.HasValue && !HandleToggle(action.Toggle.Value, action.DesiredState ?? false))
         continue;

      if (action.Dial.HasValue && !HandleDial(action.Dial.Value, action.DialValue ?? 0, !action.Toggle.HasValue && action.DesiredState is true))
         continue;

      simConnect.RequestDataDelayed(Definition.AutopilotData);
   }
}

bool HandleToggle(Toggle toggle, bool desiredState)
{
   switch (toggle)
   {
      case Toggle.Alt:
         simConnect.TransmitEvent(Events.AutopilotAltituteHoldToggle);
         break;

      case Toggle.Ap:
         simConnect.TransmitEvent(desiredState ? Events.AutopilotOn : Events.AutopilotOff);
         break;

      case Toggle.Apr:
         simConnect.TransmitEvent(Events.AutopilotApproachToggle);
         break;

      case Toggle.At:
         simConnect.TransmitEvent(Events.AutopilotAutoThrottleToggle);
         break;

      case Toggle.AtMan:
         simConnect.SetDataOnSimObject(Definition.AutoThrottleSpeedManuallySet, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_DATA_SET_FLAG.DEFAULT, new AutoThrottleSpeedManuallySet(desiredState));
         break;

      case Toggle.Fd:
         simConnect.TransmitEvent(Events.FlightDirectorToggle);
         break;

      case Toggle.Flc:
         simConnect.TransmitEvent(Events.FlightLevelChangeToggle);
         break;

      case Toggle.Hdg:
         simConnect.TransmitEvent(Events.AutopilotHeadingToggle);
         break;

      case Toggle.Lvl:
         simConnect.TransmitEvent(Events.AutopilotLevelerToggle);
         break;

      case Toggle.Nav:
         simConnect.TransmitEvent(Events.AutopilotNavToggle);
         break;

      case Toggle.Vs:
         simConnect.TransmitEvent(Events.VerticalSpeedSet, (uint)(desiredState ? 1 : 0));
         break;

      case Toggle.VNav:
         simConnect.TransmitEvent(Events.AutopilotVNavToggle);
         break;

      case Toggle.Yd:
         simConnect.TransmitEvent(Events.YawDamperToggle);
         break;

      default:
         Console.WriteLine("Unhandled toggle action: " + toggle.ToString());
         return false;
   }

   return true;
}

bool HandleDial(Dial dial, int dialValue, bool special)
{
   switch (dial)
   {
      case Dial.Alt:
         simConnect.SetAltitude((special ? (int)Math.Round(lastAutopilotSettings.IndicatedAltitude / 100f) : dialValue) * 100);
         break;

      case Dial.Hdg:
         if (special)
            simConnect.TransmitEvent(Events.AutopilotHeadingSetCurrent);
         else
            simConnect.TransmitEvent(Events.AutopilotHeadingSet, (uint)dialValue);
         break;

      case Dial.Vs:
         simConnect.SetVerticalSpeed(dialValue * 100);
         break;

      case Dial.Spd:
         simConnect.TransmitEvent(Events.AutopilotSpeedSet, (uint)(special ? lastAutopilotSettings.IndicatedSpeed : dialValue));
         break;

      default:
         Console.WriteLine("Unhandled dial action: " + dial.ToString());
         return false;
   }

   return true;
}
