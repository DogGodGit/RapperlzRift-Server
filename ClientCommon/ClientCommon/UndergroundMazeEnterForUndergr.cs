using System;

namespace ClientCommon;

public class UndergroundMazeEnterForUndergroundMazeReviveCommandBody : CommandBody
{
}
public class UndergroundMazeEnterForUndergroundMazeReviveResponseBody : ResponseBody
{
	public int hp;

	public Guid placeInstanceId;

	public PDVector3 position;

	public float rotationY;

	public PDHero[] heroes;

	public PDUndergroundMazeMonsterInstance[] monsterInsts;

	public DateTime date;

	public int paidImmediateRevivalDailyCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(hp);
		writer.Write(placeInstanceId);
		writer.Write(position);
		writer.Write(rotationY);
		writer.Write(heroes);
		writer.Write(monsterInsts);
		writer.Write(date);
		writer.Write(paidImmediateRevivalDailyCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		hp = reader.ReadInt32();
		placeInstanceId = reader.ReadGuid();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
		heroes = reader.ReadPDPacketDatas<PDHero>();
		monsterInsts = reader.ReadPDMonsterInstances<PDUndergroundMazeMonsterInstance>();
		date = reader.ReadDateTime();
		paidImmediateRevivalDailyCount = reader.ReadInt32();
	}
}
