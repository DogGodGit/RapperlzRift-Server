using System;

namespace ClientCommon;

public class SEBNationWarMultiKillEventBody : SEBServerEventBody
{
	public Guid heroId;

	public string heroName;

	public int nationId;

	public int killCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(heroName);
		writer.Write(nationId);
		writer.Write(killCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		heroName = reader.ReadString();
		nationId = reader.ReadInt32();
		killCount = reader.ReadInt32();
	}
}
