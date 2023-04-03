using System;

namespace ClientCommon;

public class SEBHeroMoveModeChangedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public bool isWalking;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(isWalking);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		isWalking = reader.ReadBoolean();
	}
}
