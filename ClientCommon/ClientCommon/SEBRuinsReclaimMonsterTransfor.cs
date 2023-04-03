namespace ClientCommon;

public class SEBRuinsReclaimMonsterTransformationCancelObjectLifetimeEndedEventBody : SEBServerEventBody
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
public class SEBRuinsReclaimMonsterTransformationFinishedEventBody : SEBServerEventBody
{
}
public class SEBRuinsReclaimMonsterTransformationCancelObjectInteractionCancelEventBody : SEBServerEventBody
{
}
public class SEBRuinsReclaimMonsterTransformationStartEventBody : SEBServerEventBody
{
	public int transformationMonsterId;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(transformationMonsterId);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		transformationMonsterId = reader.ReadInt32();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
public class SEBRuinsReclaimMonsterTransformationCancelObjectInteractionFinishedEventBody : SEBServerEventBody
{
}
