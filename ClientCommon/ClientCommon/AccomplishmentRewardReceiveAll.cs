namespace ClientCommon;

public class AccomplishmentRewardReceiveAllCommandBody : CommandBody
{
}
public class AccomplishmentRewardReceiveAllResponseBody : ResponseBody
{
	public int[] rewardedAccomplishments;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(rewardedAccomplishments);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		rewardedAccomplishments = reader.ReadInts();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
