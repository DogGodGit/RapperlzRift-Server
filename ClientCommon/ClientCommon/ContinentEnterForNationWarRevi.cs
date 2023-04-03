using System;

namespace ClientCommon;

public class ContinentEnterForNationWarReviveCommandBody : CommandBody
{
}
public class ContinentEnterForNationWarReviveResponseBody : ResponseBody
{
	public PDContinentEntranceInfo entranceInfo;

	public int hp;

	public DateTime date;

	public int paidImmediateRevivalDailyCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(entranceInfo);
		writer.Write(hp);
		writer.Write(date);
		writer.Write(paidImmediateRevivalDailyCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		entranceInfo = reader.ReadPDPacketData<PDContinentEntranceInfo>();
		hp = reader.ReadInt32();
		date = reader.ReadDateTime();
		paidImmediateRevivalDailyCount = reader.ReadInt32();
	}
}
