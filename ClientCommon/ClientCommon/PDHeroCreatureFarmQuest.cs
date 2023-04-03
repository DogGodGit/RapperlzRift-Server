using System;

namespace ClientCommon;

public class PDHeroCreatureFarmQuest : PDPacketData
{
	public Guid instanceId;

	public int missionNo;

	public int progressCount;

	public long monsterInstanceId;

	public PDVector3 monsterPosition;

	public float remainingMonsterLifetime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(missionNo);
		writer.Write(progressCount);
		writer.Write(monsterInstanceId);
		writer.Write(monsterPosition);
		writer.Write(remainingMonsterLifetime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadGuid();
		missionNo = reader.ReadInt32();
		progressCount = reader.ReadInt32();
		monsterInstanceId = reader.ReadInt64();
		monsterPosition = reader.ReadPDVector3();
		remainingMonsterLifetime = reader.ReadSingle();
	}
}
