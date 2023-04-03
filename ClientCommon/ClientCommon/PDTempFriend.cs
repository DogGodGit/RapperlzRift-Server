using System;

namespace ClientCommon;

public class PDTempFriend : PDPacketData
{
	public Guid id;

	public string name;

	public int nationId;

	public int jobId;

	public int level;

	public long battlePower;

	public float regElapsedTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(name);
		writer.Write(nationId);
		writer.Write(jobId);
		writer.Write(level);
		writer.Write(battlePower);
		writer.Write(regElapsedTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		name = reader.ReadString();
		nationId = reader.ReadInt32();
		jobId = reader.ReadInt32();
		level = reader.ReadInt32();
		battlePower = reader.ReadInt64();
		regElapsedTime = reader.ReadSingle();
	}
}
