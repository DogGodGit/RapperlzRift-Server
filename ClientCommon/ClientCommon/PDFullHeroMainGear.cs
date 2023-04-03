using System;

namespace ClientCommon;

public class PDFullHeroMainGear : PDPacketData
{
	public Guid id;

	public int mainGearId;

	public int enchantLevel;

	public bool owned;

	public PDHeroMainGearOptionAttr[] optionAttrs;

	public PDHeroMainGearRefinement[] refinements;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(mainGearId);
		writer.Write(enchantLevel);
		writer.Write(owned);
		writer.Write(optionAttrs);
		writer.Write(refinements);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		mainGearId = reader.ReadInt32();
		enchantLevel = reader.ReadInt32();
		owned = reader.ReadBoolean();
		optionAttrs = reader.ReadPDPacketDatas<PDHeroMainGearOptionAttr>();
		refinements = reader.ReadPDPacketDatas<PDHeroMainGearRefinement>();
	}
}
