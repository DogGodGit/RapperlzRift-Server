namespace ClientCommon;

public class SEBDropObjectLootedEventBody : SEBServerEventBody
{
	public PDFullHeroMainGear[] heroMainGears;

	public int maxAcquisitionMainGearGrade;

	public PDInventorySlot[] changedInventorySlots;

	public PDDropObject[] lootedDropObjects;

	public PDDropObject[] notLootedDropObjects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroMainGears);
		writer.Write(maxAcquisitionMainGearGrade);
		writer.Write(changedInventorySlots);
		writer.Write(lootedDropObjects);
		writer.Write(notLootedDropObjects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroMainGears = reader.ReadPDPacketDatas<PDFullHeroMainGear>();
		maxAcquisitionMainGearGrade = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		lootedDropObjects = reader.ReadPDDropObjects();
		notLootedDropObjects = reader.ReadPDDropObjects();
	}
}
