using System;

namespace ClientCommon;

public class PDGuildDailyObjectiveCompletionMember : PDPacketData
{
	public Guid id;

	public string name;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(name);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		name = reader.ReadString();
	}
}
