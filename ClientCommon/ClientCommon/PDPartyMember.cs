using System;

namespace ClientCommon;

public class PDPartyMember : PDPacketData
{
	public Guid id;

	public string name;

	public int jobId;

	public int level;

	public long battlePower;

	public int maxHP;

	public int hp;

	public bool isLoggedIn;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(name);
		writer.Write(jobId);
		writer.Write(level);
		writer.Write(battlePower);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(isLoggedIn);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		name = reader.ReadString();
		jobId = reader.ReadInt32();
		level = reader.ReadInt32();
		battlePower = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		isLoggedIn = reader.ReadBoolean();
	}
}
