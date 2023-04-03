using System;

namespace ClientCommon;

public class SEBOwnerProspectQuestTargetLevelUpdatedEventBody : SEBServerEventBody
{
	public Guid instanceId;

	public int targetLevel;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(targetLevel);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadGuid();
		targetLevel = reader.ReadInt32();
	}
}
