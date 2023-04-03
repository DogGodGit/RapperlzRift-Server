namespace ClientCommon;

public class SEBWingAcquisitionEventBody : SEBServerEventBody
{
	public PDHeroWing wing;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(wing);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		wing = reader.ReadPDPacketData<PDHeroWing>();
	}
}
