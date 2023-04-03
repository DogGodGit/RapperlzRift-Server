using System;

namespace ClientCommon;

public class PDHeroGuildHuntingQuest : PDPacketData
{
	public Guid id;

	public int objectiveId;

	public int progressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(objectiveId);
		writer.Write(progressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		objectiveId = reader.ReadInt32();
		progressCount = reader.ReadInt32();
	}
}
