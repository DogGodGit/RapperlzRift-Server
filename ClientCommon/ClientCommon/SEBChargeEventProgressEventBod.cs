namespace ClientCommon;

public class SEBChargeEventProgressEventBody : SEBServerEventBody
{
	public int eventId;

	public int accUnOwnDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(eventId);
		writer.Write(accUnOwnDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		eventId = reader.ReadInt32();
		accUnOwnDia = reader.ReadInt32();
	}
}
