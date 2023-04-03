namespace ClientCommon;

public class CreatureAdditionalAttrSwitchResponseBody : ResponseBody
{
	public int[] additionalAttrIds;

	public int maxHP;

	public int hp;

	public PDInventorySlot changedInventorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(additionalAttrIds);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(changedInventorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		additionalAttrIds = reader.ReadInts();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
