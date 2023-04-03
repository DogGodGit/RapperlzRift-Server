using System;

namespace ClientCommon;

public class PDHeroTrueHeroQuest : PDPacketData
{
	public Guid instanceId;

	public int stepNo;

	public int vipLevel;

	public DateTime acceptedDate;

	public bool completed;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(stepNo);
		writer.Write(vipLevel);
		writer.Write(acceptedDate);
		writer.Write(completed);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadGuid();
		stepNo = reader.ReadInt32();
		vipLevel = reader.ReadInt32();
		acceptedDate = reader.ReadDateTime();
		completed = reader.ReadBoolean();
	}
}
