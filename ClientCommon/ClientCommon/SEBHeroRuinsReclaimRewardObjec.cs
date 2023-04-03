using System;

namespace ClientCommon;

public class SEBHeroRuinsReclaimRewardObjectInteractionFinishedEventBody : SEBServerEventBody
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
public class SEBHeroRuinsReclaimRewardObjectInteractionStartEventBody : SEBServerEventBody
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
public class SEBHeroRuinsReclaimRewardObjectInteractionCancelEventBody : SEBServerEventBody
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
