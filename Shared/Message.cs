namespace Shared;

public record Message
{
   public MessageType Type { get; set; }
   public string SimVarName { get; set; }
   public string Unit { get; set; }

   public Message(MessageType type, string simVarName, string unit)
   {
      Type = type;
      SimVarName = simVarName;
      Unit = unit;
   }
}
