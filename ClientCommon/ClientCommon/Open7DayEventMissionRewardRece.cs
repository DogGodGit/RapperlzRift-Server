namespace ClientCommon;

public class Open7DayEventMissionRewardReceiveCommandBody : CommandBody
{
	public int missionId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(missionId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		missionId = reader.ReadInt32();
	}
}
public class Open7DayEventMissionRewardReceiveResponseBody : ResponseBody
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
