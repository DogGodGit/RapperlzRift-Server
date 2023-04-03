using System;

namespace ClientCommon;

public class CreatureCardShopPaidRefreshResponseBody : ResponseBody
{
	public DateTime date;

	public int dailyPaidRefreshCount;

	public PDHeroCreatureCardShopRandomProduct[] randomProducts;

	public int ownDia;

	public int unOwnDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(dailyPaidRefreshCount);
		writer.Write(randomProducts);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		dailyPaidRefreshCount = reader.ReadInt32();
		randomProducts = reader.ReadPDPacketDatas<PDHeroCreatureCardShopRandomProduct>();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
	}
}
