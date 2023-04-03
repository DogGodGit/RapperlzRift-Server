namespace ClientCommon;

public class PDHeroTreatOfFarmQuestMission : PDPacketData
{
	public int missionId;

	public long monsterInstanceId;

	public PDVector3 monsterPosition;

	public float remainingMonsterLifetime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(missionId);
		writer.Write(monsterInstanceId);
		writer.Write(monsterPosition);
		writer.Write(remainingMonsterLifetime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		missionId = reader.ReadInt32();
		monsterInstanceId = reader.ReadInt64();
		monsterPosition = reader.ReadPDVector3();
		remainingMonsterLifetime = reader.ReadSingle();
	}
}
