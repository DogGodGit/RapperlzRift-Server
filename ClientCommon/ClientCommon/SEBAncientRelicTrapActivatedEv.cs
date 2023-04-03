namespace ClientCommon;

public class SEBAncientRelicTrapActivatedEventBody : SEBServerEventBody
{
	public int id;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadInt32();
	}
}
