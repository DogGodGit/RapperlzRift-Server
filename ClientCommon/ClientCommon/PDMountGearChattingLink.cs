namespace ClientCommon;

public class PDMountGearChattingLink : PDChattingLink
{
	public PDHeroMountGear gear;

	public override int type => 3;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(gear);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		gear = reader.ReadPDPacketData<PDHeroMountGear>();
	}
}
