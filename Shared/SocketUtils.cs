using System.Net;

namespace Shared;

public static class SocketUtils
{
   public static IPEndPoint GetEndPoint() => new(IPAddress.Loopback, 13337);
}
