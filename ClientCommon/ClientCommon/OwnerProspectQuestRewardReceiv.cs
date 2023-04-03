using System;

namespace ClientCommon;

public class OwnerProspectQuestRewardReceiveAllCommandBody : CommandBody
{
}
public class OwnerProspectQuestRewardReceiveAllResponseBody : ResponseBody
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
public class OwnerProspectQuestRewardReceiveCommandBody : CommandBody
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
public class OwnerProspectQuestRewardReceiveResponseBody : ResponseBody
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
