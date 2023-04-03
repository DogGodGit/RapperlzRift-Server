namespace ClientCommon;

public class BlessingRewardReceiveResponseBody : ResponseBody
{
	public long gold;

	public long maxGold;

	public int friendApplicationResult;

	public PDFriendApplication friendAppication;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(gold);
		writer.Write(maxGold);
		writer.Write(friendApplicationResult);
		writer.Write(friendAppication);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		gold = reader.ReadInt64();
		maxGold = reader.ReadInt64();
		friendApplicationResult = reader.ReadInt32();
		friendAppication = reader.ReadPDPacketData<PDFriendApplication>();
	}
}
