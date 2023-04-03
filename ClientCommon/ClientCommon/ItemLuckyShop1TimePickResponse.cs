using System;

namespace ClientCommon;

public class ItemLuckyShop1TimePickResponseBody : ResponseBody
{
	public DateTime date;

	public int pick1TimeCount;

	public int ownDia;

	public int unOwnDia;

	public long gold;

	public long maxGold;

	public PDInventorySlot[] changedInventorySlots;

	public PDItemLuckyShopPickResult pickResult;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(pick1TimeCount);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
		writer.Write(gold);
		writer.Write(maxGold);
		writer.Write(changedInventorySlots);
		writer.Write(pickResult);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		pick1TimeCount = reader.ReadInt32();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
		gold = reader.ReadInt64();
		maxGold = reader.ReadInt64();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		pickResult = reader.ReadPDPacketData<PDItemLuckyShopPickResult>();
	}
}
