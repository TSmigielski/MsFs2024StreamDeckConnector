using System.Net.Sockets;
using System.Text.Json;
using BarRaider.SdTools;
using Shared;
namespace SdPlugin;

[PluginActionId("com.tomasz-smigielski.msfs2024connector.simtest")]
public class SimAction(ISDConnection connection, InitialPayload payload) : KeypadBase(connection, payload)
{
   CancellationTokenSource? cts;

   public async override void KeyPressed(KeyPayload payload)
   {
      if (cts != null)
         await cts.CancelAsync();

      cts = new CancellationTokenSource();

      using var udp = new UdpClient();

      var message = new Message(MessageType.GetDouble, "AIRSPEED INDICATED", "knots");
      try
      {
         await udp.SendAsync(JsonSerializer.SerializeToUtf8Bytes(message), SocketUtils.GetEndPoint());
      }
      catch
      {
         await Connection.SetTitleAsync("Send failed");
         _ = Reset(cts.Token);
         return;
      }

      UdpReceiveResult response;

      try
      {
         response = await udp.ReceiveAsync();
      }
      catch
      {
         await Connection.SetTitleAsync("Recv failed");
         _ = Reset(cts.Token);
         return;
      }

      try
      {
         var obj = JsonSerializer.Deserialize<double?>(response.Buffer);
         if (obj != null)
            await Connection.SetTitleAsync(obj.Value.ToString("N1"));
         else
            await Connection.SetTitleAsync("No obj =C");
      }
      catch
      {
         await Connection.SetTitleAsync("JSON failed");
      }
   }

   private async Task Reset(CancellationToken ct)
   {
      await Task.Delay(2000);
      if (ct.IsCancellationRequested)
         return;

      await Connection.SetTitleAsync("...");
   }

   public override void KeyReleased(KeyPayload payload) { }

   public override void OnTick() { }

   public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

   public override void ReceivedSettings(ReceivedSettingsPayload payload) { }
   public override void Dispose() { }
}
