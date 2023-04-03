using System;

namespace ClientCommon;

public class PDHeroDailyQuestProgressCount : PDPacketData
{
	public Guid id;

	public int progressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(progressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		progressCount = reader.ReadInt32();
	}
}
