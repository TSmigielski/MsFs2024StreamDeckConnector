using SimConnect.NET;

var client = new SimConnectClient("MSFS StreamDeck Connector");

try
{
   await client.ConnectAsync();
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
   var altitude = await client.SimVars.GetAsync<double>("PLANE ALTITUDE", "feet");
   var airspeed = await client.SimVars.GetAsync<double>("AIRSPEED INDICATED", "knots");

   Console.WriteLine($"Altitude: {altitude:F0} ft");
   Console.WriteLine($"Airspeed: {airspeed:F0} kts");

   await Task.Delay(1000);
}
