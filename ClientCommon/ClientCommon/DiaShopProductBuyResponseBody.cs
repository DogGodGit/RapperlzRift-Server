using System;

namespace ClientCommon;

public class DiaShopProductBuyResponseBody : ResponseBody
{
	public DateTime date;

	public int dailyBuyCount;

	public int totalBuyCount;

	public int ownDia;

	public int unOwnDia;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(dailyBuyCount);
		writer.Write(totalBuyCount);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		dailyBuyCount = reader.ReadInt32();
		totalBuyCount = reader.ReadInt32();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
