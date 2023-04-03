namespace ClientCommon;

public class SEBWisdomTemplePuzzleCompletedEventBody : SEBServerEventBody
{
	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHP;

	public int hp;

	public PDWisdomTemplePuzzleRewardObjectInstance[] puzzleRewardObjectInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(puzzleRewardObjectInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		puzzleRewardObjectInsts = reader.ReadPDPacketDatas<PDWisdomTemplePuzzleRewardObjectInstance>();
	}
}
