using System.Diagnostics;

using var wineRelayProcess = new Process();
wineRelayProcess.StartInfo = new ("protontricks-launch", "--appid 2537590 /home/ts-pl/Projects/dotnet/MsFsStreamDeckConnector/WineRelay/bin/Debug/net10.0/win-x64/WineRelay.exe");
wineRelayProcess.Start();
