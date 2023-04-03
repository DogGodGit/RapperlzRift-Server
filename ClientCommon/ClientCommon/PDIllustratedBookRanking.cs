using System;

namespace ClientCommon;

public class PDIllustratedBookRanking : PDPacketData
{
	public int ranking;

	public Guid heroId;

	public int nationId;

	public int jobId;

	public string name;

	public int level;

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
		explorationPoint = reader.ReadInt32();
	}
}
