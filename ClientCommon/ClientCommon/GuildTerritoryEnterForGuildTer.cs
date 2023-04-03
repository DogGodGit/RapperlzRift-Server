using System;

namespace ClientCommon;

public class GuildTerritoryEnterForGuildTerritoryRevivalCommandBody : CommandBody
{
}
public class GuildTerritoryEnterForGuildTerritoryRevivalResponseBody : ResponseBody
{
	public Guid placeInstanceId;

	public PDHero[] heroes;

	public PDMonsterInstance[] monsters;

	public int hp;

	public PDVector3 position;

	public float rotationY;

	public DateTime date;

	public int paidImmediateRevivalDailyCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(placeInstanceId);
		writer.Write(heroes);
		writer.Write(monsters);
		writer.Write(hp);
		writer.Write(position);
		writer.Write(rotationY);
		writer.Write(date);
		writer.Write(paidImmediateRevivalDailyCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		placeInstanceId = reader.ReadGuid();
		heroes = reader.ReadPDPacketDatas<PDHero>();
		monsters = reader.ReadPDMonsterInstances();
		hp = reader.ReadInt32();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
		date = reader.ReadDateTime();
		paidImmediateRevivalDailyCount = reader.ReadInt32();
	}
}
