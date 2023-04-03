using System;

namespace ClientCommon;

public class TargetProspectQuestRewardReceiveCommandBody : CommandBody
{
	public Guid instanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadGuid();
	}
}
public class TargetProspectQuestRewardReceiveResponseBody : ResponseBody
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
public class TargetProspectQuestRewardReceiveAllCommandBody : CommandBody
{
}
public class TargetProspectQuestRewardReceiveAllResponseBody : ResponseBody
{
	public PDInventorySlot[] changedInventorySlots;

	public Guid[] receivedInstanceIds;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedInventorySlots);
		writer.Write(receivedInstanceIds);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		receivedInstanceIds = reader.ReadGuids();
	}
}
