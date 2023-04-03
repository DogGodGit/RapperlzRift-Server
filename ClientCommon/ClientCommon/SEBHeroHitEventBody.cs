using System;

namespace ClientCommon;

public class SEBHeroHitEventBody : SEBServerEventBody
{
	public Guid heroId;

	public PDHitResult hitResult;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(hitResult);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		hitResult = reader.ReadPDPacketData<PDHitResult>();
	}
}
