using System;

namespace ClientCommon;

public class GuildApplyResponseBody : ResponseBody
{
	public PDHeroGuildApplication application;

	public DateTime date;

	public int dailyApplicationCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(application);
		writer.Write(date);
		writer.Write(dailyApplicationCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		application = reader.ReadPDPacketData<PDHeroGuildApplication>();
		date = reader.ReadDateTime();
		dailyApplicationCount = reader.ReadInt32();
	}
}
