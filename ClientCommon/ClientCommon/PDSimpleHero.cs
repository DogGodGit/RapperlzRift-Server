using System;

namespace ClientCommon;

public class PDSimpleHero : PDPacketData
{
	public Guid id;

	public string name;

	public int nationId;

	public int noblesseId;

	public int jobId;

	public int level;

	public int vipLevel;

	public long battlePower;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(name);
		writer.Write(nationId);
		writer.Write(noblesseId);
		writer.Write(jobId);
		writer.Write(level);
		writer.Write(vipLevel);
		writer.Write(battlePower);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		name = reader.ReadString();
		nationId = reader.ReadInt32();
		noblesseId = reader.ReadInt32();
		jobId = reader.ReadInt32();
		level = reader.ReadInt32();
		vipLevel = reader.ReadInt32();
		battlePower = reader.ReadInt64();
	}
}
