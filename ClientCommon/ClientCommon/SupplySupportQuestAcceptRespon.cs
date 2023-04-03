using System;

namespace ClientCommon;

public class SupplySupportQuestAcceptResponseBody : ResponseBody
{
	public PDSupplySupportQuestCartInstance cartInst;

	public float remainingTime;

	public DateTime date;

	public int supplySupportQuestDailyStartCount;

	public PDInventorySlot changedInventorySlot;

	public long gold;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(cartInst);
		writer.Write(remainingTime);
		writer.Write(date);
		writer.Write(supplySupportQuestDailyStartCount);
		writer.Write(changedInventorySlot);
		writer.Write(gold);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		cartInst = reader.ReadPDCartInstance<PDSupplySupportQuestCartInstance>();
		remainingTime = reader.ReadSingle();
		date = reader.ReadDateTime();
		supplySupportQuestDailyStartCount = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
		gold = reader.ReadInt64();
	}
}
