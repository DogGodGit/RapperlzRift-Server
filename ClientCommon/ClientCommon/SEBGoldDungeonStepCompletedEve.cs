namespace ClientCommon;

public class SEBGoldDungeonStepCompletedEventBody : SEBServerEventBody
{
	public long rewardGold;

	public long gold;

	public long maxGold;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(rewardGold);
		writer.Write(gold);
		writer.Write(maxGold);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		rewardGold = reader.ReadInt64();
		gold = reader.ReadInt64();
		maxGold = reader.ReadInt64();
	}
}
