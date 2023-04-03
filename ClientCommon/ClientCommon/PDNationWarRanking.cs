using System;

namespace ClientCommon;

public class PDNationWarRanking : PDPacketData
{
	public int ranking;

	public Guid heroId;

	public string heroName;

	public int killCount;

	public Guid guildId;

	public string guildName;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(ranking);
		writer.Write(heroId);
		writer.Write(heroName);
		writer.Write(killCount);
		writer.Write(guildId);
		writer.Write(guildName);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		ranking = reader.ReadInt32();
		heroId = reader.ReadGuid();
		heroName = reader.ReadString();
		killCount = reader.ReadInt32();
		guildId = reader.ReadGuid();
		guildName = reader.ReadString();
	}
}
