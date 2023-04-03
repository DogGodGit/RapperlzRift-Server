using System;

namespace ClientCommon;

public class AncientRelicEnterResponseBody : ResponseBody
{
	public DateTime date;

	public Guid placeInstanceId;

	public PDVector3 position;

	public float rotationY;

	public float remainingStartTime;

	public float remainingLimitTime;

	public int stepNo;

	public int waveNo;

	public PDHero[] heroes;

	public PDMonsterInstance[] monsterInsts;

	public Guid[] trapEffectHeroes;

	public int[] removedObstacleIds;

	public int hp;

	public int stamina;

	public int playCount;

	public int paidImmediateRevivalDailyCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(placeInstanceId);
		writer.Write(position);
		writer.Write(rotationY);
		writer.Write(remainingStartTime);
		writer.Write(remainingLimitTime);
		writer.Write(stepNo);
		writer.Write(waveNo);
		writer.Write(heroes);
		writer.Write(monsterInsts);
		writer.Write(trapEffectHeroes);
		writer.Write(removedObstacleIds);
		writer.Write(hp);
		writer.Write(stamina);
		writer.Write(playCount);
		writer.Write(paidImmediateRevivalDailyCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		placeInstanceId = reader.ReadGuid();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
		remainingStartTime = reader.ReadSingle();
		remainingLimitTime = reader.ReadSingle();
		stepNo = reader.ReadInt32();
		waveNo = reader.ReadInt32();
		heroes = reader.ReadPDPacketDatas<PDHero>();
		monsterInsts = reader.ReadPDMonsterInstances<PDMonsterInstance>();
		trapEffectHeroes = reader.ReadGuids();
		removedObstacleIds = reader.ReadInts();
		hp = reader.ReadInt32();
		stamina = reader.ReadInt32();
		playCount = reader.ReadInt32();
		paidImmediateRevivalDailyCount = reader.ReadInt32();
	}
}
