using System;

namespace ClientCommon;

public class ArtifactLevelUpCommandBody : CommandBody
{
	public Guid[] mainGears;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(mainGears);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		mainGears = reader.ReadGuids();
	}
}
