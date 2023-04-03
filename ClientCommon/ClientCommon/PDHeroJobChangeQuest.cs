using System;

namespace ClientCommon;

public class PDHeroJobChangeQuest : PDPacketData
{
	public Guid instanceId;

	public int questNo;

	public int progressCount;

	public int difficulty;

	public float remainingTime;

	public int status;

	public long monsterInstanceId;

	public PDVector3 monsterPosition;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(questNo);
		writer.Write(progressCount);
		writer.Write(difficulty);
		writer.Write(remainingTime);
		writer.Write(status);
		writer.Write(monsterInstanceId);
		writer.Write(monsterPosition);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadGuid();
		questNo = reader.ReadInt32();
		progressCount = reader.ReadInt32();
		difficulty = reader.ReadInt32();
		remainingTime = reader.ReadSingle();
		status = reader.ReadInt32();
		monsterInstanceId = reader.ReadInt64();
		monsterPosition = reader.ReadPDVector3();
	}
}
