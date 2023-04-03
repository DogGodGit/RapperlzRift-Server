using System;

namespace ClientCommon;

public class GuildTerritoryEnterResponseBody : ResponseBody
{
	public Guid placeInstanceId;

	public PDHero[] heroes;

	public PDMonsterInstance[] monsters;

	public PDVector3 position;

	public float rotationY;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(placeInstanceId);
		writer.Write(heroes);
		writer.Write(monsters);
		writer.Write(position);
		writer.Write(rotationY);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		placeInstanceId = reader.ReadGuid();
		heroes = reader.ReadPDPacketDatas<PDHero>();
		monsters = reader.ReadPDMonsterInstances();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
	}
}
