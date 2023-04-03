namespace ClientCommon;

public class OpenGiftReceiveCommandBody : CommandBody
{
	public int day;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(day);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		day = reader.ReadInt32();
	}
}
