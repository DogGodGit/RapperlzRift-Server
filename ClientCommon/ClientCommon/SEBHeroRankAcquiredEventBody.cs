using System;

namespace ClientCommon;

public class SEBHeroRankAcquiredEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int rankNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(rankNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		rankNo = reader.ReadInt32();
	}
}
