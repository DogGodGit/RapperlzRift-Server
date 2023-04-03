namespace ClientCommon;

public class PDMainGearChattingLink : PDChattingLink
{
	public PDFullHeroMainGear gear;

	public override int type => 1;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(gear);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		gear = reader.ReadPDPacketData<PDFullHeroMainGear>();
	}
}
