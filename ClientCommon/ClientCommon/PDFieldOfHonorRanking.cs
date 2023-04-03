using System;

namespace ClientCommon;

public class PDFieldOfHonorRanking : PDPacketData
{
	public int ranking;

	public Guid heroId;

	public string name;

	public int nationId;

	public long battlePower;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(ranking);
		writer.Write(heroId);
		writer.Write(name);
		writer.Write(nationId);
		writer.Write(battlePower);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		ranking = reader.ReadInt32();
		heroId = reader.ReadGuid();
		name = reader.ReadString();
		nationId = reader.ReadInt32();
		battlePower = reader.ReadInt64();
	}
}
