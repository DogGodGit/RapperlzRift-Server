using System;

namespace ClientCommon;

public class SEBHeroRuinsReclaimMonsterTransformationCancelObjectInteractionStartEventBody : SEBServerEventBody
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
public class SEBHeroRuinsReclaimMonsterTransformationCancelObjectInteractionCancelEventBody : SEBServerEventBody
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
public class SEBHeroRuinsReclaimMonsterTransformationCancelObjectInteractionFinishedEventBody : SEBServerEventBody
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
public class SEBHeroRuinsReclaimMonsterTransformationStartEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int transformationMonsterId;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(transformationMonsterId);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		transformationMonsterId = reader.ReadInt32();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
public class SEBHeroRuinsReclaimMonsterTransformationFinishedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
	}
}
