using System;

namespace ClientCommon;

public class ExpScrollUseResponseBody : ResponseBody
{
	public DateTime date;

	public int expScrollDailyUseCount;

	public float expScrollRemainingTime;

	public PDInventorySlot changedInventorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(expScrollDailyUseCount);
		writer.Write(expScrollRemainingTime);
		writer.Write(changedInventorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		expScrollDailyUseCount = reader.ReadInt32();
		expScrollRemainingTime = reader.ReadSingle();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
