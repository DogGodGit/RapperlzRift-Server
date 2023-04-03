namespace ClientCommon;

public class MainGearEquipResponseBody : ResponseBody
{
	public bool mainGearOwned;

	public int maxEquippedMainGearEnchantLevel;

	public int changedInventorySlotIndex;

	public int maxHp;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(mainGearOwned);
		writer.Write(maxEquippedMainGearEnchantLevel);
		writer.Write(changedInventorySlotIndex);
		writer.Write(maxHp);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		mainGearOwned = reader.ReadBoolean();
		maxEquippedMainGearEnchantLevel = reader.ReadInt32();
		changedInventorySlotIndex = reader.ReadInt32();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
