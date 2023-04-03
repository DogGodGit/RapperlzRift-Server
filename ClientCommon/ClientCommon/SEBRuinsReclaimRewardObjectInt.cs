namespace ClientCommon;

public class SEBRuinsReclaimRewardObjectInteractionCancelEventBody : SEBServerEventBody
{
}
public class SEBRuinsReclaimRewardObjectInteractionFinishedEventBody : SEBServerEventBody
{
	public long gold;

	public long maxGold;

	public PDItemBooty booty;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(gold);
		writer.Write(maxGold);
		writer.Write(booty);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		gold = reader.ReadInt64();
		maxGold = reader.ReadInt64();
		booty = reader.ReadPDBooty<PDItemBooty>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
