namespace ClientCommon;

public class SEBTradeShipObjectDestructionRewardEventBody : SEBServerEventBody
{
	public int point;

	public PDItemBooty booty;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(point);
		writer.Write(booty);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		point = reader.ReadInt32();
		booty = reader.ReadPDBooty<PDItemBooty>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
