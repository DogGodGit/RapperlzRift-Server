namespace ClientCommon;

public class FieldOfHonorRankingRewardReceiveCommandBody : CommandBody
{
}
public class FieldOfHonorRankingRewardReceiveResponseBody : ResponseBody
{
	public int rewardedRankingNo;

	public PDItemBooty[] booties;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(rewardedRankingNo);
		writer.Write(booties);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		rewardedRankingNo = reader.ReadInt32();
		booties = reader.ReadPDBooties<PDItemBooty>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
