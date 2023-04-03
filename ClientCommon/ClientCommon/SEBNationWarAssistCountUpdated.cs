namespace ClientCommon;

public class SEBNationWarAssistCountUpdatedEventBody : SEBServerEventBody
{
	public int assistCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(assistCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		assistCount = reader.ReadInt32();
	}
}
