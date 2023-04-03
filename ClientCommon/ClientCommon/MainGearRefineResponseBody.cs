using System;

namespace ClientCommon;

public class MainGearRefineResponseBody : ResponseBody
{
	public DateTime date;

	public int refinementDailyCount;

	public PDHeroMainGearRefinement[] refinements;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(refinementDailyCount);
		writer.Write(refinements);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		refinementDailyCount = reader.ReadInt32();
		refinements = reader.ReadPDPacketDatas<PDHeroMainGearRefinement>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
