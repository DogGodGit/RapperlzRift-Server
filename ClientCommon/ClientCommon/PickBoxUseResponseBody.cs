namespace ClientCommon;

public class PickBoxUseResponseBody : ResponseBody
{
	public PDInventorySlot[] changedInventorySlots;

	public PDFullHeroMainGear[] addedHeroMainGears;

	public int maxAcquisitionMainGearGrade;

	public PDHeroMountGear[] addedHeroMountGears;

	public PDHeroCreatureCard[] changedHeroCreatureCards;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedInventorySlots);
		writer.Write(addedHeroMainGears);
		writer.Write(maxAcquisitionMainGearGrade);
		writer.Write(addedHeroMountGears);
		writer.Write(changedHeroCreatureCards);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		addedHeroMainGears = reader.ReadPDPacketDatas<PDFullHeroMainGear>();
		maxAcquisitionMainGearGrade = reader.ReadInt32();
		addedHeroMountGears = reader.ReadPDPacketDatas<PDHeroMountGear>();
		changedHeroCreatureCards = reader.ReadPDPacketDatas<PDHeroCreatureCard>();
	}
}
