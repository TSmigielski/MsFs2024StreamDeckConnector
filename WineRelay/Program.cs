using System.Net.Sockets;
using System.Text.Json;
using Shared;
using SimConnect.NET;

using var udp = new UdpClient(SocketUtils.GetEndPoint());
using var simClient = new SimConnectClient("MSFS StreamDeck Connector");

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
   var response = await udp.ReceiveAsync();
   var message = JsonSerializer.Deserialize<Message>(response.Buffer);
   if (message == null)
      continue;

   Console.WriteLine($"Received a message:{Environment.NewLine}{JsonSerializer.Serialize(message)}");

   object? value = null;
   switch (message.Type)
   {
      case MessageType.GetDouble:
         value = await simClient.SimVars.GetAsync<double>(message.SimVarName, message.Unit);
         break;
   }

   if (value != null)
      await udp.SendAsync(JsonSerializer.SerializeToUtf8Bytes(value), response.RemoteEndPoint);

   // Get aircraft data
   // var airspeed = await simClient.SimVars.GetAsync<double>("AIRSPEED INDICATED", "knots");
   // var sb = new StringBuilder();
   //
   // sb.AppendLine($"Altitude: {altitude:F0} ft");
   // sb.AppendLine($"Airspeed: {airspeed:F0} kts");
   //
   // var bytes = Encoding.UTF8.GetBytes(sb.ToString()).ToList();
   // bytes.Add((byte)MessageType.Eot);
   //
   // await socket.SendAsync(bytes.ToArray());
   // await Task.Delay(100);
}
