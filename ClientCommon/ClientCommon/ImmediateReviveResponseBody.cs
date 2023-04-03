using System;

namespace ClientCommon;

public class ImmediateReviveResponseBody : ResponseBody
{
	public int hp;

	public DateTime date;

	public int freeImmediateRevivalDailyCount;

	public int paidImmediateRevivalDailyCount;

	public int ownDia;

	public int unOwnDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(hp);
		writer.Write(date);
		writer.Write(freeImmediateRevivalDailyCount);
		writer.Write(paidImmediateRevivalDailyCount);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		hp = reader.ReadInt32();
		date = reader.ReadDateTime();
		freeImmediateRevivalDailyCount = reader.ReadInt32();
		paidImmediateRevivalDailyCount = reader.ReadInt32();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
	}
}
