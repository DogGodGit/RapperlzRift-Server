using System;

namespace ClientCommon;

public class WarMemoryEnterResponseBody : ResponseBody
{
	public DateTime date;

	public Guid placeInstanceId;

	public PDVector3 position;

	public float rotationY;

	public float remainingStartTime;

	public float remainingLimitTime;

	public int waveNo;

	public PDHero[] heroes;

	public PDMonsterInstance[] monsterInsts;

	public PDWarMemoryTransformationObjectInstance[] objectInsts;

	public PDWarMemoryPoint[] points;

	public int hp;

	public int freePlayCount;

	public PDInventorySlot changedInventorySlot;

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
		writer.Write(waveNo);
		writer.Write(heroes);
		writer.Write(monsterInsts);
		writer.Write(objectInsts);
		writer.Write(points);
		writer.Write(hp);
		writer.Write(freePlayCount);
		writer.Write(changedInventorySlot);
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
		waveNo = reader.ReadInt32();
		heroes = reader.ReadPDPacketDatas<PDHero>();
		monsterInsts = reader.ReadPDMonsterInstances<PDMonsterInstance>();
		objectInsts = reader.ReadPDPacketDatas<PDWarMemoryTransformationObjectInstance>();
		points = reader.ReadPDPacketDatas<PDWarMemoryPoint>();
		hp = reader.ReadInt32();
		freePlayCount = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
		paidImmediateRevivalDailyCount = reader.ReadInt32();
	}
}
