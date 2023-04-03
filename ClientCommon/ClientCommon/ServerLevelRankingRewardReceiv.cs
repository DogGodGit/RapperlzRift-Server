namespace ClientCommon;

public class ServerLevelRankingRewardReceiveCommandBody : CommandBody
{
}
public class ServerLevelRankingRewardReceiveResponseBody : ResponseBody
{
	public int rewardedRankingNo;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(rewardedRankingNo);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		rewardedRankingNo = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
