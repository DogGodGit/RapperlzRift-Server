namespace ClientCommon;

public class SEBMainQuestMonsterTransformationFinishedEventBody : SEBServerEventBody
{
	public int maxHP;

	public int hp;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
public class SEBMainQuestMonsterTransformationCanceledEventBody : SEBServerEventBody
{
	public int maxHP;

	public int hp;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
