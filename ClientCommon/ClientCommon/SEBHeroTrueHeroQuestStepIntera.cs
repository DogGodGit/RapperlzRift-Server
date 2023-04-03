using System;

namespace ClientCommon;

public class SEBHeroTrueHeroQuestStepInteractionFinishedEventBody : SEBServerEventBody
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
public class SEBHeroTrueHeroQuestStepInteractionCanceledEventBody : SEBServerEventBody
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
public class SEBHeroTrueHeroQuestStepInteractionStartedEventBody : SEBServerEventBody
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
