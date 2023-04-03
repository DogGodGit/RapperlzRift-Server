namespace ClientCommon;

public class BlessingQuestBlessingSendResponseBody : ResponseBody
{
	public long gold;

	public int ownDia;

	public int unOwnDia;

	public PDInventorySlot[] changedInventorySlots;

	public PDHeroProspectQuest newProspectQuest;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(gold);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
		writer.Write(changedInventorySlots);
		writer.Write(newProspectQuest);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		gold = reader.ReadInt64();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		newProspectQuest = reader.ReadPDPacketData<PDHeroProspectQuest>();
	}
}
