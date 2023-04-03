using System;

namespace ClientCommon;

public class HeroInitEnterResponseBody : ResponseBody
{
	public PDVector3 position;

	public float rotationY;

	public Guid placeInstanceId;

	public PDHero[] heroes;

	public PDMonsterInstance[] monsterInsts;

	public PDContinentObjectInstance[] continentObjectInsts;

	public PDCartInstance[] cartInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(position);
		writer.Write(rotationY);
		writer.Write(placeInstanceId);
		writer.Write(heroes);
		writer.Write(monsterInsts);
		writer.Write(continentObjectInsts);
		writer.Write(cartInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
		placeInstanceId = reader.ReadGuid();
		heroes = reader.ReadPDPacketDatas<PDHero>();
		monsterInsts = reader.ReadPDMonsterInstances();
		continentObjectInsts = reader.ReadPDPacketDatas<PDContinentObjectInstance>();
		cartInsts = reader.ReadPDCartInstances();
	}
}
