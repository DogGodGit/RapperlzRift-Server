using System;

namespace ClientCommon;

public class SEBHeroWarMemoryTransformationMonsterSkillCastEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int skillId;

	public PDVector3 skillTargetPosition;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(skillId);
		writer.Write(skillTargetPosition);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		skillId = reader.ReadInt32();
		skillTargetPosition = reader.ReadPDVector3();
	}
}
public class SEBHeroWarMemoryTransformationObjectInteractionStartEventBody : SEBServerEventBody
{
	public Guid heroId;

	public long objectInstanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(objectInstanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		objectInstanceId = reader.ReadInt64();
	}
}
public class SEBHeroWarMemoryTransformationObjectInteractionFinishedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public long objectInstanceId;

	public int point;

	public long pointUpdatedTimeTicks;

	public int maxHp;

	public int hp;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(objectInstanceId);
		writer.Write(point);
		writer.Write(pointUpdatedTimeTicks);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		objectInstanceId = reader.ReadInt64();
		point = reader.ReadInt32();
		pointUpdatedTimeTicks = reader.ReadInt64();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
public class SEBHeroWarMemoryTransformationObjectInteractionCancelEventBody : SEBServerEventBody
{
	public Guid heroId;

	public long objectInstanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(objectInstanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		objectInstanceId = reader.ReadInt64();
	}
}
