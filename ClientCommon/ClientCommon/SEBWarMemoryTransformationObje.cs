namespace ClientCommon;

public class SEBWarMemoryTransformationObjectInteractionFinishedEventBody : SEBServerEventBody
{
	public int point;

	public long pointUpdatedTimeTicks;

	public int maxHp;

	public int hp;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(point);
		writer.Write(pointUpdatedTimeTicks);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		point = reader.ReadInt32();
		pointUpdatedTimeTicks = reader.ReadInt64();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
public class SEBWarMemoryTransformationObjectInteractionCancelEventBody : SEBServerEventBody
{
}
public class SEBWarMemoryTransformationObjectLifetimeEndedEventBody : SEBServerEventBody
{
	public long instanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
	}
}
