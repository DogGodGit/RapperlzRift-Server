using System;

namespace ClientCommon;

public class FearAltarReviveResponseBody : ResponseBody
{
	public int hp;

	public DateTime date;

	public int paidImmediateRevivalDailyCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(hp);
		writer.Write(date);
		writer.Write(paidImmediateRevivalDailyCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		hp = reader.ReadInt32();
		date = reader.ReadDateTime();
		paidImmediateRevivalDailyCount = reader.ReadInt32();
	}
}
