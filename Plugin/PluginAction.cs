using BarRaider.SdTools;
namespace Plugin;

[PluginActionId("com.tomasz-smigielski.msfs2024connector.test")]
public class PluginAction(ISDConnection connection, InitialPayload payload) : KeypadBase(connection, payload)
{
   private int counter;
   public async override void KeyPressed(KeyPayload payload)
   {
      counter++;
      await Connection.SetTitleAsync(counter.ToString());
   }

   public override void KeyReleased(KeyPayload payload) { }

   public override void OnTick() { }

   public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

   public override void ReceivedSettings(ReceivedSettingsPayload payload) { }
   public override void Dispose() { }
}
