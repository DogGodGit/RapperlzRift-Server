namespace ClientCommon;

public class SubGearUnequipResponseBody : ResponseBody
{
	public int changedInventorySlotIndex;

	public int maxHp;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedInventorySlotIndex);
		writer.Write(maxHp);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedInventorySlotIndex = reader.ReadInt32();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
