using System;

namespace ClientCommon;

public class SEBGuildAltarSpellInjectionMissionCompletedEventBody : SEBServerEventBody
{
	public DateTime date;

	public int guildMoralPoint;

	public int giMoralPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(guildMoralPoint);
		writer.Write(giMoralPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		guildMoralPoint = reader.ReadInt32();
		giMoralPoint = reader.ReadInt32();
	}
}
public class SEBGuildAltarSpellInjectionMissionCanceledEventBody : SEBServerEventBody
{
}
