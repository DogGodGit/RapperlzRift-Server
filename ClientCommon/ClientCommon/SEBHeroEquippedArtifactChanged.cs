using System;

namespace ClientCommon;

public class SEBHeroEquippedArtifactChangedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int equippedArtifactNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(equippedArtifactNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		equippedArtifactNo = reader.ReadInt32();
	}
}
