using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.FlightSimulator.SimConnect;
using WineRelay;

var jsonSettings = new JsonSerializerOptions
{
   IncludeFields = true,
   WriteIndented = true
};

using var udp = new UdpClient(new IPEndPoint(IPAddress.Loopback, 13337));
using var simConnect = new SimConnect("MSFS StreamDeck Connector", 0, 0x0402, null, 0);
simConnect.Initialize();

simConnect.AddToDataDefinition(Definitions.Struct1, "Title", null, SIMCONNECT_DATATYPE.STRING256, 0, SimConnect.SIMCONNECT_UNUSED);
simConnect.AddToDataDefinition(Definitions.Struct1, "Airspeed True", "knots", SIMCONNECT_DATATYPE.FLOAT64, 0, SimConnect.SIMCONNECT_UNUSED);
simConnect.AddToDataDefinition(Definitions.Struct1, "Trailing Edge Flaps Left Percent", "percent", SIMCONNECT_DATATYPE.FLOAT64, 0, SimConnect.SIMCONNECT_UNUSED);
simConnect.AddToDataDefinition(Definitions.Struct1, "Spoilers Handle Position", "percent", SIMCONNECT_DATATYPE.FLOAT64, 0, SimConnect.SIMCONNECT_UNUSED);
simConnect.AddToDataDefinition(Definitions.Struct1, "Autopilot Master", "bool", SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);

simConnect.RegisterDataDefineStruct<Struct1>(Definitions.Struct1);
simConnect.RequestDataOnSimObject(Requests.Request1, Definitions.Struct1, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.ONCE, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);

var i = 0;

while (true)
{
   if (i >= 50)
   {
      i = 0;
      // simConnect.RequestDataOnSimObject(Requests.Request1, Definitions.Struct1, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.ONCE, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
   }
   // The handler gets called only when an event actually gets dispatched.
   simConnect.ReceiveDispatch(DispatchedEventHandler);
   await Task.Delay(50);
   i++;
}


void DispatchedEventHandler(SIMCONNECT_RECV pData, uint cbData)
{
   var @event = (SIMCONNECT_RECV_ID)pData.dwID;
   switch (@event)
   {
      case SIMCONNECT_RECV_ID.EXCEPTION:
         var exception = (SIMCONNECT_RECV_EXCEPTION) pData;
         Console.WriteLine(JsonSerializer.Serialize(exception, jsonSettings));
         return;

      case SIMCONNECT_RECV_ID.SIMOBJECT_DATA:
         var simObject = (SIMCONNECT_RECV_SIMOBJECT_DATA) pData;
         Console.WriteLine(JsonSerializer.Serialize(simObject, jsonSettings));
         return;

      default:
         Console.WriteLine($"Unhandled event `{pData.dwID}|{@event:G}`:" + JsonSerializer.Serialize(pData, jsonSettings));
         return;
   }
}

// while (true)
// {
//    var response = await udp.ReceiveAsync();
//    var message = JsonSerializer.Deserialize<Message>(response.Buffer);
//    if (message == null)
//       continue;
//
//    Console.WriteLine($"Received a message:{Environment.NewLine}{JsonSerializer.Serialize(message)}");
//
//    object? value = null;
//    switch (message.Type)
//    {
//       // case MessageType.GetDouble:
//       //    value = await simClient.SimVars.GetAsync<double>(message.SimVarName, message.Unit);
//       //    break;
//    }
//
//    if (value != null)
//       await udp.SendAsync(JsonSerializer.SerializeToUtf8Bytes(value), response.RemoteEndPoint);
// }

enum Definitions
{
   Struct1 = 333,
}

enum Requests
{
   Request1 = 666,
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct Struct1
{
   [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
   public string title; // Aircraft title
   public double trueAirspeed; // True airspeed in knots
   public double flaps; // Flaps position
   public double spoilers; // Airbrakes / Spoilers position
   public bool autopilotMaster;
}
