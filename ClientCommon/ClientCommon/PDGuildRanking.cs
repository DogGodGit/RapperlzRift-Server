using System;

namespace ClientCommon;

public class PDGuildRanking : PDPacketData
{
	public int ranking;

	public int nationId;

	public Guid guildId;

	public string guildName;

	public long might;

	public Guid guildMasterId;

	public string guildMasterName;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(ranking);
		writer.Write(nationId);
		writer.Write(guildId);
		writer.Write(guildName);
		writer.Write(might);
		writer.Write(guildMasterId);
		writer.Write(guildMasterName);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		ranking = reader.ReadInt32();
		nationId = reader.ReadInt32();
		guildId = reader.ReadGuid();
		guildName = reader.ReadString();
		might = reader.ReadInt64();
		guildMasterId = reader.ReadGuid();
		guildMasterName = reader.ReadString();
	}
}
