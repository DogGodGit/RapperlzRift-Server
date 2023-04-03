using System;

namespace ClientCommon;

public class GuildFoodWarehouseStockResponseBody : ResponseBody
{
	public int addedFoodWarehouseExp;

	public int foodWarehouseLevel;

	public int foodWarehouseExp;

	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHp;

	public int hp;

	public PDInventorySlot changedInventorySlot;

	public DateTime date;

	public int dailyStockCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(addedFoodWarehouseExp);
		writer.Write(foodWarehouseLevel);
		writer.Write(foodWarehouseExp);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(changedInventorySlot);
		writer.Write(date);
		writer.Write(dailyStockCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		addedFoodWarehouseExp = reader.ReadInt32();
		foodWarehouseLevel = reader.ReadInt32();
		foodWarehouseExp = reader.ReadInt32();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
		date = reader.ReadDateTime();
		dailyStockCount = reader.ReadInt32();
	}
}
