namespace ClientCommon;

public class WingEnchantTotallyResponseBody : ResponseBody
{
	public PDHeroWingEnchant changedEnchant;

	public int wingStep;

	public int wingLevel;

	public int wingExp;

	public PDInventorySlot[] changedInventorySlots;

	public PDHeroWing addedWing;

	public int maxHp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedEnchant);
		writer.Write(wingStep);
		writer.Write(wingLevel);
		writer.Write(wingExp);
		writer.Write(changedInventorySlots);
		writer.Write(addedWing);
		writer.Write(maxHp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedEnchant = reader.ReadPDPacketData<PDHeroWingEnchant>();
		wingStep = reader.ReadInt32();
		wingLevel = reader.ReadInt32();
		wingExp = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		addedWing = reader.ReadPDPacketData<PDHeroWing>();
		maxHp = reader.ReadInt32();
	}
}
