using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.FlightSimulator.SimConnect;
using WineRelay;
using WineRelay.Enums.SimConnect;
using WineRelay.Enums.Udp;
using WineRelay.Models;

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
               Console.WriteLine($"Sending to: {remote.Address}:{remote.Port}");
               udp.Send(JsonSerializer.SerializeToUtf8Bytes(simObject.dwData[0]), remote);
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
      ToggleActionModel? action;

      try
      {
         action = JsonSerializer.Deserialize<ToggleActionModel>(response.Buffer);
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

      Console.WriteLine($"Received an Action:{Environment.NewLine}{JsonSerializer.Serialize(action)}");

      switch (action.Toggle)
      {
         case ToggleAction.Ap:
            simConnect.TransmitEvent(action.State ? Events.AutopilotOn : Events.AutopilotOff);
            break;

         case ToggleAction.Fd:
            simConnect.TransmitEvent(Events.FlightDirectorToggle);
            break;

         case ToggleAction.Flc:
            simConnect.TransmitEvent(Events.FlightLevelChangeToggle);
            break;

         case ToggleAction.Nav:
            Console.WriteLine("Not yet implemented");
            continue;

         case ToggleAction.Vs:
            simConnect.TransmitEvent(Events.VerticalSpeedSet, (uint)(action.State ? 1 : 0));
            break;

         case ToggleAction.Hdg:
            simConnect.TransmitEvent(Events.AutopilotHeadingToggle);
            break;

         case ToggleAction.Lvl:
            simConnect.TransmitEvent(Events.AutopilotLevelerToggle);
            break;

         case ToggleAction.Alt:
            simConnect.TransmitEvent(Events.AutopilotAltituteHoldToggle);
            break;

         default:
            Console.WriteLine("Unhandled ToggleAction: " + action.Toggle.ToString());
            continue;
      }

      simConnect.RequestDataDelayed(Definition.AutopilotData);
   }
}
