using System;

namespace ClientCommon;

public class DistortionScrollUseResponseBody : ResponseBody
{
	public DateTime date;

	public int distortionScrollDailyUseCount;

	public float distortionRemainingTime;

	public PDInventorySlot changedInventorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(distortionScrollDailyUseCount);
		writer.Write(distortionRemainingTime);
		writer.Write(changedInventorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		distortionScrollDailyUseCount = reader.ReadInt32();
		distortionRemainingTime = reader.ReadSingle();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
