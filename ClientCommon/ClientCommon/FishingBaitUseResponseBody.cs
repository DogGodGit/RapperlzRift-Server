using System;

namespace ClientCommon;

public class FishingBaitUseResponseBody : ResponseBody
{
	public PDHeroFishingQuest quest;

	public DateTime date;

	public int fishingQuestDailyStartCount;

	public int accEpicBaitItemUseCount;

	public int accLegendBaitItemUseCount;

	public PDInventorySlot changedInventorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(quest);
		writer.Write(date);
		writer.Write(fishingQuestDailyStartCount);
		writer.Write(accEpicBaitItemUseCount);
		writer.Write(accLegendBaitItemUseCount);
		writer.Write(changedInventorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		quest = reader.ReadPDPacketData<PDHeroFishingQuest>();
		date = reader.ReadDateTime();
		fishingQuestDailyStartCount = reader.ReadInt32();
		accEpicBaitItemUseCount = reader.ReadInt32();
		accLegendBaitItemUseCount = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
