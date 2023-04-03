using System;

namespace ClientCommon;

public class SEBHeroContinentObjectInteractionStartEventBody : SEBServerEventBody
{
	public Guid heroId;

	public long continentObjectInstanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(continentObjectInstanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		continentObjectInstanceId = reader.ReadInt64();
	}
}
public class SEBHeroContinentObjectInteractionFinishedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public long continentObjectInstanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(continentObjectInstanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		continentObjectInstanceId = reader.ReadInt64();
	}
}
public class SEBHeroContinentObjectInteractionCancelEventBody : SEBServerEventBody
{
	public Guid heroId;

	public long continentObjectInstanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(continentObjectInstanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		continentObjectInstanceId = reader.ReadInt64();
	}
}
