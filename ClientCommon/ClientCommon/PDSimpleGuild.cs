using System;

namespace ClientCommon;

public class PDSimpleGuild : PDPacketData
{
	public Guid id;

	public string name;

	public string notice;

	public int level;

	public Guid masterId;

	public string masterName;

	public int memberCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(name);
		writer.Write(notice);
		writer.Write(level);
		writer.Write(masterId);
		writer.Write(masterName);
		writer.Write(memberCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		name = reader.ReadString();
		notice = reader.ReadString();
		level = reader.ReadInt32();
		masterId = reader.ReadGuid();
		masterName = reader.ReadString();
		memberCount = reader.ReadInt32();
	}
}
