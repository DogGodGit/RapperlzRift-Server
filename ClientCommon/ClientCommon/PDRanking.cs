using System;

namespace ClientCommon;

public class PDRanking : PDPacketData
{
	public int ranking;

	public Guid heroId;

	public int nationId;

	public int jobId;

	public string name;

	public int level;

	public long battlePower;

	public long exp;

	public int exploitPoint;

	public int collectionFamePoint;

	public int explorationPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(ranking);
		writer.Write(heroId);
		writer.Write(nationId);
		writer.Write(jobId);
		writer.Write(name);
		writer.Write(level);
		writer.Write(battlePower);
		writer.Write(exp);
		writer.Write(exploitPoint);
		writer.Write(collectionFamePoint);
		writer.Write(explorationPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		ranking = reader.ReadInt32();
		heroId = reader.ReadGuid();
		nationId = reader.ReadInt32();
		jobId = reader.ReadInt32();
		name = reader.ReadString();
		level = reader.ReadInt32();
		battlePower = reader.ReadInt64();
		exp = reader.ReadInt64();
		exploitPoint = reader.ReadInt32();
		collectionFamePoint = reader.ReadInt32();
		explorationPoint = reader.ReadInt32();
	}
}
