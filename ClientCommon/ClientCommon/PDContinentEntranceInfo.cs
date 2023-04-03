using System;

namespace ClientCommon;

public class PDContinentEntranceInfo : PDPacketData
{
	public Guid placeInstanceId;

	public int continentId;

	public int nationId;

	public PDHero[] heroes;

	public PDMonsterInstance[] monsters;

	public PDContinentObjectInstance[] objectInsts;

	public PDCartInstance[] cartInsts;

	public PDVector3 position;

	public float rotationY;

	public PDContinentEntranceInfo()
	{
	}

	public PDContinentEntranceInfo(Guid placeInstanceId, int nContinentId, int nNationId, PDHero[] heroes, PDMonsterInstance[] monsters, PDContinentObjectInstance[] objectInsts, PDCartInstance[] cartInsts, PDVector3 position, float fRotationY)
	{
		this.placeInstanceId = placeInstanceId;
		continentId = nContinentId;
		nationId = nNationId;
		this.heroes = heroes;
		this.monsters = monsters;
		this.objectInsts = objectInsts;
		this.cartInsts = cartInsts;
		this.position = position;
		rotationY = fRotationY;
	}

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(placeInstanceId);
		writer.Write(continentId);
		writer.Write(nationId);
		writer.Write(heroes);
		writer.Write(monsters);
		writer.Write(objectInsts);
		writer.Write(cartInsts);
		writer.Write(position);
		writer.Write(rotationY);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		placeInstanceId = reader.ReadGuid();
		continentId = reader.ReadInt32();
		nationId = reader.ReadInt32();
		heroes = reader.ReadPDPacketDatas<PDHero>();
		monsters = reader.ReadPDMonsterInstances();
		objectInsts = reader.ReadPDPacketDatas<PDContinentObjectInstance>();
		cartInsts = reader.ReadPDCartInstances();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
	}
}
