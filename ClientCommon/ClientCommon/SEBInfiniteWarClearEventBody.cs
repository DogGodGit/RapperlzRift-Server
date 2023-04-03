namespace ClientCommon;

public class SEBInfiniteWarClearEventBody : SEBServerEventBody
{
	public PDInfiniteWarRanking[] rankings;

	public PDItemBooty[] booties;

	public PDItemBooty[] rankingBooties;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(rankings);
		writer.Write(booties);
		writer.Write(rankingBooties);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		rankings = reader.ReadPDPacketDatas<PDInfiniteWarRanking>();
		booties = reader.ReadPDBooties<PDItemBooty>();
		rankingBooties = reader.ReadPDBooties<PDItemBooty>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
