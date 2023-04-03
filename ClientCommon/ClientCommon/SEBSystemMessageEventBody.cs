namespace ClientCommon;

public class SEBSystemMessageEventBody : SEBServerEventBody
{
	public PDSystemMessage systemMessage;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(systemMessage);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		systemMessage = reader.ReadPDSystemMessage<PDSystemMessage>();
	}
}
