using System;

namespace ClientCommon;

public class BountyHunterQuestScrollUseResponseBody : ResponseBody
{
	public PDHeroBountyHunterQuest quest;

	public DateTime date;

	public int bountyHunterQuestDailyStartCount;

	public PDInventorySlot changedInventorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(quest);
		writer.Write(date);
		writer.Write(bountyHunterQuestDailyStartCount);
		writer.Write(changedInventorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		quest = reader.ReadPDPacketData<PDHeroBountyHunterQuest>();
		date = reader.ReadDateTime();
		bountyHunterQuestDailyStartCount = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
