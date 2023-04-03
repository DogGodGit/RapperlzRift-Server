namespace ClientCommon;

public class PDHeroPotionAttr : PDPacketData
{
	public int potionAttrId;

	public int count;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(potionAttrId);
		writer.Write(count);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		potionAttrId = reader.ReadInt32();
		count = reader.ReadInt32();
	}
}
