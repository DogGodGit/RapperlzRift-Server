using System;

namespace ClientCommon;

public class SEBNationWarNoblesseKillEventBody : SEBServerEventBody
{
	public Guid killerId;

	public string killerName;

	public int killerNationId;

	public Guid deadHeroId;

	public string deadHeroName;

	public int deadNoblesseId;

	public int deadNationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(killerId);
		writer.Write(killerName);
		writer.Write(killerNationId);
		writer.Write(deadHeroId);
		writer.Write(deadHeroName);
		writer.Write(deadNoblesseId);
		writer.Write(deadNationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		killerId = reader.ReadGuid();
		killerName = reader.ReadString();
		killerNationId = reader.ReadInt32();
		deadHeroId = reader.ReadGuid();
		deadHeroName = reader.ReadString();
		deadNoblesseId = reader.ReadInt32();
		deadNationId = reader.ReadInt32();
	}
}
