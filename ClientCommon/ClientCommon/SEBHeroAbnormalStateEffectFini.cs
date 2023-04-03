using System;

namespace ClientCommon;

public class SEBHeroAbnormalStateEffectFinishedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public long abnormalStateEffectInstanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(abnormalStateEffectInstanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		abnormalStateEffectInstanceId = reader.ReadInt64();
	}
}
