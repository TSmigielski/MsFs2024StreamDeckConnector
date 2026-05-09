using System.Diagnostics;
using System.Text;
using Shared;

var socket = SocketUtils.GetSocket();
socket.Bind(SocketUtils.GetEndPoint());
socket.Listen();

// Start wine relay
Console.WriteLine("Starting...");
using var wineRelayProcess = new Process();
wineRelayProcess.StartInfo = new ("protontricks-launch", "--appid 2537590 /home/ts-pl/Projects/dotnet/MsFsStreamDeckConnector/WineRelay/bin/Debug/net10.0/win-x64/WineRelay.exe")
{
   RedirectStandardOutput = false,
   RedirectStandardError = false
};
wineRelayProcess.Start();
Console.WriteLine("Started?");

var client = await socket.AcceptAsync();
var buffer = new byte[1024*1024];

while (true)
{
   var bytesReceived = await client.ReceiveAsync(buffer);
   if (bytesReceived > 0)
      ProcessBuffer(bytesReceived);

   await Task.Delay(10);
}

void ProcessBuffer(int bytesReceived)
{
   var messageBytes = new List<byte>(bytesReceived);
   for (var i = 0; i < bytesReceived; i++)
   {
      var @byte = buffer[i];
      if (@byte == (byte) MessageType.Eot)
      {
         PrintMessage(messageBytes);
         return;
      }

      messageBytes.Add(@byte);
   }

   PrintMessage(messageBytes);
}

void PrintMessage(List<byte> bytes) => Console.WriteLine(Encoding.UTF8.GetString([.. bytes]));
