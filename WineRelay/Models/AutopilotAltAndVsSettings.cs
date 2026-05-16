namespace WineRelay.Models;

public struct AutopilotAltitude
{
   public int Altitude { get; set; }

   public AutopilotAltitude() { }
   public AutopilotAltitude(int alt) => Altitude = alt;
}

public struct AutopilotVerticalSpeed
{
   public float VerticalSpeed { get; set; }
   public AutopilotVerticalSpeed() { }
   public AutopilotVerticalSpeed(float vs) => VerticalSpeed = vs;
}
