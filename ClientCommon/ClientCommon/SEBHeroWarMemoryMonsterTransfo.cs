using System;

namespace ClientCommon;

public class SEBHeroWarMemoryMonsterTransformationCancelEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int maxHp;

	public int hp;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
public class SEBHeroWarMemoryMonsterTransformationFinishedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int maxHp;

	public int hp;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
