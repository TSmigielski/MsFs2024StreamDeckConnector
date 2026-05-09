using System.Net;
using System.Net.Sockets;

namespace Shared;

public static class SocketUtils
{
   // public static Socket GetSocket() => new(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
   public static Socket GetSocket() => new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Unspecified);
   // public static EndPoint GetEndPoint() => new UnixDomainSocketEndPoint("/tmp/MsFsStreamDeckConnector.sock");
   public static EndPoint GetEndPoint() => new IPEndPoint(IPAddress.Loopback, 54321);
}
