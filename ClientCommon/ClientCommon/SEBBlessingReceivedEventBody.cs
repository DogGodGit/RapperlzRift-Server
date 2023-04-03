namespace ClientCommon;

public class SEBBlessingReceivedEventBody : SEBServerEventBody
{
	public PDHeroBlessing blessing;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(blessing);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		blessing = reader.ReadPDPacketData<PDHeroBlessing>();
	}
}
