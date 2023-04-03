using System;

namespace ClientCommon;

public class UndergroundMazePortalExitResponseBody : ResponseBody
{
	public Guid placeInstanceId;

	public PDVector3 position;

	public float rotationY;

	public PDHero[] heroes;

	public PDUndergroundMazeMonsterInstance[] monsterInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(placeInstanceId);
		writer.Write(position);
		writer.Write(rotationY);
		writer.Write(heroes);
		writer.Write(monsterInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		placeInstanceId = reader.ReadGuid();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
		heroes = reader.ReadPDPacketDatas<PDHero>();
		monsterInsts = reader.ReadPDMonsterInstances<PDUndergroundMazeMonsterInstance>();
	}
}
