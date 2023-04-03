using System;

namespace ClientCommon;

public class MainGearTransitCommandBody : CommandBody
{
	public Guid targetHeroMainGearId;

	public Guid materialHeroMainGearId;

	public bool isEnchantLevelTransit;

	public bool isOptionAttrTransit;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetHeroMainGearId);
		writer.Write(materialHeroMainGearId);
		writer.Write(isEnchantLevelTransit);
		writer.Write(isOptionAttrTransit);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetHeroMainGearId = reader.ReadGuid();
		materialHeroMainGearId = reader.ReadGuid();
		isEnchantLevelTransit = reader.ReadBoolean();
		isOptionAttrTransit = reader.ReadBoolean();
	}
}
