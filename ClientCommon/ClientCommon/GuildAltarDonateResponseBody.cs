using System;

namespace ClientCommon;

public class GuildAltarDonateResponseBody : ResponseBody
{
	public DateTime date;

	public long gold;

	public int guildMoralPoint;

	public int giMoralPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(gold);
		writer.Write(guildMoralPoint);
		writer.Write(giMoralPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		gold = reader.ReadInt64();
		guildMoralPoint = reader.ReadInt32();
		giMoralPoint = reader.ReadInt32();
	}
}
