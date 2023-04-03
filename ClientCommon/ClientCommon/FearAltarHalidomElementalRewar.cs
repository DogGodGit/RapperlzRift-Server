using System;

namespace ClientCommon;

public class FearAltarHalidomElementalRewardReceiveCommandBody : CommandBody
{
	public int elementalId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(elementalId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		elementalId = reader.ReadInt32();
	}
}
public class FearAltarHalidomElementalRewardReceiveResponseBody : ResponseBody
{
	public DateTime weekStartDate;

	public PDItemBooty booty;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(weekStartDate);
		writer.Write(booty);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		weekStartDate = reader.ReadDateTime();
		booty = reader.ReadPDBooty<PDItemBooty>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
