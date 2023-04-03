using System;

namespace ClientCommon;

public class CreatureCardLuckyShop1TimePickCommandBody : CommandBody
{
}
public class CreatureCardLuckyShop1TimePickResponseBody : ResponseBody
{
	public DateTime date;

	public int pick1TimeCount;

	public int ownDia;

	public int unOwnDia;

	public long gold;

	public long maxGold;

	public PDHeroCreatureCard changedCreatureCard;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(pick1TimeCount);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
		writer.Write(gold);
		writer.Write(maxGold);
		writer.Write(changedCreatureCard);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		pick1TimeCount = reader.ReadInt32();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
		gold = reader.ReadInt64();
		maxGold = reader.ReadInt64();
		changedCreatureCard = reader.ReadPDPacketData<PDHeroCreatureCard>();
	}
}
