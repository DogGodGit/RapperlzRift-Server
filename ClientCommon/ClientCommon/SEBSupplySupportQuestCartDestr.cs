namespace ClientCommon;

public class SEBSupplySupportQuestCartDestructionRewardEventBody : SEBServerEventBody
{
	public PDItemBooty booty;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(booty);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		booty = reader.ReadPDBooty<PDItemBooty>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
