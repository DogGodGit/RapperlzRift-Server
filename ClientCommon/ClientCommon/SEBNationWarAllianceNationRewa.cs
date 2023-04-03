namespace ClientCommon;

public class SEBNationWarAllianceNationRewardEventBody : SEBServerEventBody
{
	public bool isWin;

	public PDItemBooty booty;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(isWin);
		writer.Write(booty);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		isWin = reader.ReadBoolean();
		booty = reader.ReadPDBooty<PDItemBooty>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
