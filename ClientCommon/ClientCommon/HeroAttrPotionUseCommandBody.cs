namespace ClientCommon;

public class HeroAttrPotionUseCommandBody : CommandBody
{
	public int potionAttrId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(potionAttrId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		potionAttrId = reader.ReadInt32();
	}
}
