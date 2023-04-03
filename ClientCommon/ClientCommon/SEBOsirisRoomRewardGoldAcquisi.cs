namespace ClientCommon;

public class SEBOsirisRoomRewardGoldAcquisitionEventBody : SEBServerEventBody
{
	public long gold;

	public long maxGold;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(gold);
		writer.Write(maxGold);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		gold = reader.ReadInt64();
		maxGold = reader.ReadInt64();
	}
}
