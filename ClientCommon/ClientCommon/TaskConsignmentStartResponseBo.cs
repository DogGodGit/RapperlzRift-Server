using System;

namespace ClientCommon;

public class TaskConsignmentStartResponseBody : ResponseBody
{
	public DateTime date;

	public int achievementDailyPoint;

	public PDHeroTaskConsignmentStartCount startCount;

	public PDHeroTaskConsignment taskConsignment;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(achievementDailyPoint);
		writer.Write(startCount);
		writer.Write(taskConsignment);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		achievementDailyPoint = reader.ReadInt32();
		startCount = reader.ReadPDPacketData<PDHeroTaskConsignmentStartCount>();
		taskConsignment = reader.ReadPDPacketData<PDHeroTaskConsignment>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
