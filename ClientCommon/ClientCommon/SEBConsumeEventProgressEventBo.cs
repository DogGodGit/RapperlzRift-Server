namespace ClientCommon;

public class SEBConsumeEventProgressEventBody : SEBServerEventBody
{
	public int eventId;

	public int accDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(eventId);
		writer.Write(accDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		eventId = reader.ReadInt32();
		accDia = reader.ReadInt32();
	}
}
