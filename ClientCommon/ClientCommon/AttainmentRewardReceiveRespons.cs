namespace ClientCommon;

public class AttainmentRewardReceiveResponseBody : ResponseBody
{
	public PDFullHeroMainGear[] addedMainGears;

	public int maxAcquisitionMainGearGrade;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(addedMainGears);
		writer.Write(maxAcquisitionMainGearGrade);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		addedMainGears = reader.ReadPDPacketDatas<PDFullHeroMainGear>();
		maxAcquisitionMainGearGrade = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
