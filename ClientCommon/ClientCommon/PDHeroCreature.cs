using System;

namespace ClientCommon;

public class PDHeroCreature : PDPacketData
{
	public Guid instanceId;

	public int creatureId;

	public int level;

	public int additionalOpenSkillSlotCount;

	public int exp;

	public int injectionLevel;

	public int injectionExp;

	public int injectionItemCount;

	public int quality;

	public bool cheered;

	public PDHeroCreatureBaseAttr[] baseAttrs;

	public int[] additionalAttrIds;

	public PDHeroCreatureSkill[] skills;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(creatureId);
		writer.Write(level);
		writer.Write(additionalOpenSkillSlotCount);
		writer.Write(exp);
		writer.Write(injectionLevel);
		writer.Write(injectionExp);
		writer.Write(quality);
		writer.Write(cheered);
		writer.Write(baseAttrs);
		writer.Write(additionalAttrIds);
		writer.Write(skills);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadGuid();
		creatureId = reader.ReadInt32();
		level = reader.ReadInt32();
		additionalOpenSkillSlotCount = reader.ReadInt32();
		exp = reader.ReadInt32();
		injectionLevel = reader.ReadInt32();
		injectionExp = reader.ReadInt32();
		quality = reader.ReadInt32();
		cheered = reader.ReadBoolean();
		baseAttrs = reader.ReadPDPacketDatas<PDHeroCreatureBaseAttr>();
		additionalAttrIds = reader.ReadInts();
		skills = reader.ReadPDPacketDatas<PDHeroCreatureSkill>();
	}
}
