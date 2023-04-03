using System;

namespace ClientCommon;

public class DailyConsumeEventMissionRewardReceiveCommandBody : CommandBody
{
	public DateTime date;

	public int missionNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(missionNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		missionNo = reader.ReadInt32();
	}
}
public class DailyConsumeEventMissionRewardReceiveResponseBody : ResponseBody
{
	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
