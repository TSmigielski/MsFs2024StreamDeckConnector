using System.Text;
using Shared;
using SimConnect.NET;

var socket = SocketUtils.GetSocket();
var simClient = new SimConnectClient("MSFS StreamDeck Connector");

AppDomain.CurrentDomain.ProcessExit += async (sender, @event) => {
   var simDisconnectTask = simClient.DisconnectAsync();
   socket.Close();
   await simDisconnectTask;
};

try
{
    await socket.ConnectAsync(SocketUtils.GetEndPoint());
}
catch (Exception ex)
{
   Console.WriteLine("Failed to connect to the MsFsStreamDeckConnector socket, exception:");
   Console.WriteLine(ex);
   return;
}

try
{
   await simClient.ConnectAsync();
}
catch (Exception ex)
{
   Console.WriteLine("Failed to connect to MSFS, exception:");
   Console.WriteLine(ex);
   return;
}

while (true)
{
   // Get aircraft data
   var altitude = await simClient.SimVars.GetAsync<double>("PLANE ALTITUDE", "feet");
   var airspeed = await simClient.SimVars.GetAsync<double>("AIRSPEED INDICATED", "knots");
   var sb = new StringBuilder();

   sb.AppendLine($"Altitude: {altitude:F0} ft");
   sb.AppendLine($"Airspeed: {airspeed:F0} kts");

   var bytes = Encoding.UTF8.GetBytes(sb.ToString()).ToList();
   bytes.Add((byte) MessageType.Eot);

   await socket.SendAsync(bytes.ToArray());
   await Task.Delay(100);
}
