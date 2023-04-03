using System;

namespace ClientCommon;

public class SEBHeroMainQuestMonsterTransformationFinishedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int maxHP;

	public int hp;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
public class SEBHeroMainQuestMonsterTransformationCanceledEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int maxHP;

	public int hp;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
public class SEBHeroMainQuestMonsterTransformationStartedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int transformationMonsterId;

	public int maxHP;

	public int hp;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(transformationMonsterId);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		transformationMonsterId = reader.ReadInt32();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
