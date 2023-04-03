using System;

namespace ClientCommon;

public class ItemLuckyShopFreePickResponseBody : ResponseBody
{
	public DateTime date;

	public int freePickCount;

	public float freePickRemainingTime;

	public long gold;

	public long maxGold;

	public PDInventorySlot[] changedInventorySlots;

	public PDItemLuckyShopPickResult pickResult;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(freePickCount);
		writer.Write(freePickRemainingTime);
		writer.Write(gold);
		writer.Write(maxGold);
		writer.Write(changedInventorySlots);
		writer.Write(pickResult);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		freePickCount = reader.ReadInt32();
		freePickRemainingTime = reader.ReadSingle();
		gold = reader.ReadInt64();
		maxGold = reader.ReadInt64();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		pickResult = reader.ReadPDPacketData<PDItemLuckyShopPickResult>();
	}
}
