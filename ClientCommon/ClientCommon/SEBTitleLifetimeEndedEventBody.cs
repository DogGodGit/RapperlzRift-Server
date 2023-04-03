namespace ClientCommon;

public class SEBTitleLifetimeEndedEventBody : SEBServerEventBody
{
	public int titleId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(titleId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		titleId = reader.ReadInt32();
	}
}
