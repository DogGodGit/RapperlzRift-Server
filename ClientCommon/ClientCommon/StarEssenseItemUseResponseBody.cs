using System;

namespace ClientCommon;

public class StarEssenseItemUseResponseBody : ResponseBody
{
	public DateTime date;

	public int dailyStarEssenseItemUseCount;

	public int starEssesne;

	public PDInventorySlot changedInventorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(dailyStarEssenseItemUseCount);
		writer.Write(starEssesne);
		writer.Write(changedInventorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		dailyStarEssenseItemUseCount = reader.ReadInt32();
		starEssesne = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
