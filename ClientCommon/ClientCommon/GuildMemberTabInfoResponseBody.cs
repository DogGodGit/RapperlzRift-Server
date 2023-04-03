using System;

namespace ClientCommon;

public class GuildMemberTabInfoResponseBody : ResponseBody
{
	public PDGuildMember[] members;

	public DateTime date;

	public int dailyBanishmentCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(members);
		writer.Write(date);
		writer.Write(dailyBanishmentCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		members = reader.ReadPDPacketDatas<PDGuildMember>();
		date = reader.ReadDateTime();
		dailyBanishmentCount = reader.ReadInt32();
	}
}
