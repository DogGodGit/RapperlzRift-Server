namespace ClientCommon;

public class PDSubGearChattingLink : PDChattingLink
{
	public PDFullHeroSubGear gear;

	public override int type => 2;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(gear);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		gear = reader.ReadPDPacketData<PDFullHeroSubGear>();
	}
}
