using System;

namespace ClientCommon;

public class GuildFoodWarehouseRewardReceiveCommandBody : CommandBody
{
}
public class GuildFoodWarehouseRewardReceiveResponseBody : ResponseBody
{
	public Guid receivedCollectionId;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(receivedCollectionId);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		receivedCollectionId = reader.ReadGuid();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
