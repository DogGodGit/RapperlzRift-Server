namespace ClientCommon;

public class PDHeroWingPart : PDPacketData
{
	public int partId;

	public PDHeroWingEnchant[] enchants;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(partId);
		writer.Write(enchants);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		partId = reader.ReadInt32();
		enchants = reader.ReadPDPacketDatas<PDHeroWingEnchant>();
	}
}
