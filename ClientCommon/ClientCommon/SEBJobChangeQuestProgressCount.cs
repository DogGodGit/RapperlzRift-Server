using System;

namespace ClientCommon;

public class SEBJobChangeQuestProgressCountUpdatedEventBody : SEBServerEventBody
{
	public Guid instanceId;

	public int progressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(progressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadGuid();
		progressCount = reader.ReadInt32();
	}
}
