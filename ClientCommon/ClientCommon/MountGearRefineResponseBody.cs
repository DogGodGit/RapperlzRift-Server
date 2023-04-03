using System;

namespace ClientCommon;

public class MountGearRefineResponseBody : ResponseBody
{
	public DateTime date;

	public int mountGearRefinementDailyCount;

	public PDHeroMountGearOptionAttr changedHeroMountGearOptionAttr;

	public PDInventorySlot changedInventorySlot;

	public int maxHp;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(mountGearRefinementDailyCount);
		writer.Write(changedHeroMountGearOptionAttr);
		writer.Write(changedInventorySlot);
		writer.Write(maxHp);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		mountGearRefinementDailyCount = reader.ReadInt32();
		changedHeroMountGearOptionAttr = reader.ReadPDPacketData<PDHeroMountGearOptionAttr>();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
