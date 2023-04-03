namespace ClientCommon;

public class SEBContinentObjectCreatedEventBody : SEBServerEventBody
{
	public long continentObjectInstanceId;

	public int arrangeNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(continentObjectInstanceId);
		writer.Write(arrangeNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		continentObjectInstanceId = reader.ReadInt64();
		arrangeNo = reader.ReadInt32();
	}
}
