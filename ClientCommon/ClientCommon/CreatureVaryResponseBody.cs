using System;

namespace ClientCommon;

public class CreatureVaryResponseBody : ResponseBody
{
	public DateTime date;

	public int dailyCreatureVariationCount;

	public int quality;

	public PDHeroCreatureBaseAttr[] baseAttrs;

	public int maxHP;

	public int hp;

	public PDInventorySlot changedInventorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(dailyCreatureVariationCount);
		writer.Write(quality);
		writer.Write(baseAttrs);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(changedInventorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		dailyCreatureVariationCount = reader.ReadInt32();
		quality = reader.ReadInt32();
		baseAttrs = reader.ReadPDPacketDatas<PDHeroCreatureBaseAttr>();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
