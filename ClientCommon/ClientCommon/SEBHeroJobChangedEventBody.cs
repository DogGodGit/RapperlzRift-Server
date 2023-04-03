using System;

namespace ClientCommon;

public class SEBHeroJobChangedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int jobId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(jobId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		jobId = reader.ReadInt32();
	}
}
