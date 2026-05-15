namespace WineRelay.Models;

public struct AutopilotAltitude
{
   public int Altitude { get; set; }

   public AutopilotAltitude() { }
   public AutopilotAltitude(int alt) => Altitude = alt;
}

public struct AutopilotVerticalSpeed
{
   public int VerticalSpeed { get; set; }
   public AutopilotVerticalSpeed() { }
   public AutopilotVerticalSpeed(int vs) => VerticalSpeed = vs;
}
