using System;

namespace ClientCommon;

public class PDInfiniteWarRanking : PDPacketData
{
	public int rank;

	public Guid heroId;

	public string name;

	public int jobId;

	public int point;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(rank);
		writer.Write(heroId);
		writer.Write(name);
		writer.Write(jobId);
		writer.Write(point);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		rank = reader.ReadInt32();
		heroId = reader.ReadGuid();
		name = reader.ReadString();
		jobId = reader.ReadInt32();
		point = reader.ReadInt32();
	}
}
