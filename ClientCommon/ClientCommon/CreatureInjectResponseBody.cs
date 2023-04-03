namespace ClientCommon;

public class CreatureInjectResponseBody : ResponseBody
{
	public bool isCritical;

	public int injectionLevel;

	public int injectionExp;

	public int injectionItemCount;

	public int maxHP;

	public int hp;

	public long gold;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(isCritical);
		writer.Write(injectionLevel);
		writer.Write(injectionExp);
		writer.Write(injectionItemCount);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(gold);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		isCritical = reader.ReadBoolean();
		injectionLevel = reader.ReadInt32();
		injectionExp = reader.ReadInt32();
		injectionItemCount = reader.ReadInt32();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		gold = reader.ReadInt64();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
