using System;

namespace ClientCommon;

public class PDBlacklistEntry : PDPacketData
{
	public Guid heroId;

	public string name;

	public int nationId;

	public int jobId;

	public int level;

	public long battlePower;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(name);
		writer.Write(nationId);
		writer.Write(jobId);
		writer.Write(level);
		writer.Write(battlePower);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		name = reader.ReadString();
		nationId = reader.ReadInt32();
		jobId = reader.ReadInt32();
		level = reader.ReadInt32();
		battlePower = reader.ReadInt64();
	}
}
