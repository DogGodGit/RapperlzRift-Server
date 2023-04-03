namespace ClientCommon;

public class PDHeroGuildMission : PDPacketData
{
	public int missionId;

	public int progressCount;

	public long monsterInstanceId;

	public int monsterSpawnedContinentId;

	public PDVector3 monsterPosition;

	public float remainingMonsterLifetime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(missionId);
		writer.Write(progressCount);
		writer.Write(monsterInstanceId);
		writer.Write(monsterSpawnedContinentId);
		writer.Write(monsterPosition);
		writer.Write(remainingMonsterLifetime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		missionId = reader.ReadInt32();
		progressCount = reader.ReadInt32();
		monsterInstanceId = reader.ReadInt64();
		monsterSpawnedContinentId = reader.ReadInt32();
		monsterPosition = reader.ReadPDVector3();
		remainingMonsterLifetime = reader.ReadSingle();
	}
}
