namespace ClientCommon;

public class ConsumeEventMissionRewardReceiveCommandBody : CommandBody
{
	public int eventId;

	public int missionNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(eventId);
		writer.Write(missionNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		eventId = reader.ReadInt32();
		missionNo = reader.ReadInt32();
	}
}
public class ConsumeEventMissionRewardReceiveResponseBody : ResponseBody
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
