using System;

namespace ClientCommon;

public class IllustratedBookExplorationStepAcquireCommandBody : CommandBody
{
	public int targetStepNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetStepNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetStepNo = reader.ReadInt32();
	}
}
public class IllustratedBookExplorationStepAcquireResponseBody : ResponseBody
{
	public int maxHP;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHP);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
public class IllustratedBookExplorationStepRewardReceiveCommandBody : CommandBody
{
}
public class IllustratedBookExplorationStepRewardReceiveResponseBody : ResponseBody
{
	public DateTime date;

	public long gold;

	public long maxGold;

	public PDItemBooty[] booties;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(gold);
		writer.Write(maxGold);
		writer.Write(booties);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		gold = reader.ReadInt64();
		maxGold = reader.ReadInt64();
		booties = reader.ReadPDBooties<PDItemBooty>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
