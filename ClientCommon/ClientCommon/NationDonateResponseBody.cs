using System;

namespace ClientCommon;

public class NationDonateResponseBody : ResponseBody
{
	public DateTime date;

	public int dailyNationDonationCount;

	public int ownDia;

	public int unOwnDia;

	public long gold;

	public int acquiredExploitPoint;

	public int exploitPoint;

	public int dailyExploitPoint;

	public long nationFund;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(dailyNationDonationCount);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
		writer.Write(gold);
		writer.Write(acquiredExploitPoint);
		writer.Write(exploitPoint);
		writer.Write(dailyExploitPoint);
		writer.Write(nationFund);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		dailyNationDonationCount = reader.ReadInt32();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
		gold = reader.ReadInt64();
		acquiredExploitPoint = reader.ReadInt32();
		exploitPoint = reader.ReadInt32();
		dailyExploitPoint = reader.ReadInt32();
		nationFund = reader.ReadInt64();
	}
}
