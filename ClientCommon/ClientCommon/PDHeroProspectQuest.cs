using System;

namespace ClientCommon;

public class PDHeroProspectQuest : PDPacketData
{
	public Guid instanceId;

	public Guid ownerId;

	public string ownerName;

	public int ownerJobId;

	public int ownerLevel;

	public Guid targetId;

	public string targetName;

	public int targetJobId;

	public int targetLevel;

	public int blessingTargetLevelId;

	public float remainingTime;

	public bool isCompleted;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(ownerId);
		writer.Write(ownerName);
		writer.Write(ownerJobId);
		writer.Write(ownerLevel);
		writer.Write(targetId);
		writer.Write(targetName);
		writer.Write(targetJobId);
		writer.Write(targetLevel);
		writer.Write(blessingTargetLevelId);
		writer.Write(remainingTime);
		writer.Write(isCompleted);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadGuid();
		ownerId = reader.ReadGuid();
		ownerName = reader.ReadString();
		ownerJobId = reader.ReadInt32();
		ownerLevel = reader.ReadInt32();
		targetId = reader.ReadGuid();
		targetName = reader.ReadString();
		targetJobId = reader.ReadInt32();
		targetLevel = reader.ReadInt32();
		blessingTargetLevelId = reader.ReadInt32();
		remainingTime = reader.ReadSingle();
		isCompleted = reader.ReadBoolean();
	}
}
