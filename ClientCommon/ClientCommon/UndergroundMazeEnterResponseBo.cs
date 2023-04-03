using System;

namespace ClientCommon;

public class UndergroundMazeEnterResponseBody : ResponseBody
{
	public DateTime date;

	public float playtime;

	public Guid placeInstanceId;

	public PDVector3 position;

	public float rotationY;

	public PDHero[] heroes;

	public PDUndergroundMazeMonsterInstance[] monsterInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(playtime);
		writer.Write(placeInstanceId);
		writer.Write(position);
		writer.Write(rotationY);
		writer.Write(heroes);
		writer.Write(monsterInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		playtime = reader.ReadSingle();
		placeInstanceId = reader.ReadGuid();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
		heroes = reader.ReadPDPacketDatas<PDHero>();
		monsterInsts = reader.ReadPDMonsterInstances<PDUndergroundMazeMonsterInstance>();
	}
}
