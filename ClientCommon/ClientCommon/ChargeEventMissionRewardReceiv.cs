namespace ClientCommon;

public class ChargeEventMissionRewardReceiveCommandBody : CommandBody
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
public class ChargeEventMissionRewardReceiveResponseBody : ResponseBody
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
