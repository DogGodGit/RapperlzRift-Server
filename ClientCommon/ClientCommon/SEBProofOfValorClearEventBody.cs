namespace ClientCommon;

public class SEBProofOfValorClearEventBody : SEBServerEventBody
{
	public int clearGrade;

	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHP;

	public int hp;

	public int soulPowder;

	public PDHeroCreatureCard changedCreatureCard;

	public PDHeroProofOfValorInstance heroProofOfValorInst;

	public int proofOfValorPaidRefreshCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(clearGrade);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(soulPowder);
		writer.Write(changedCreatureCard);
		writer.Write(heroProofOfValorInst);
		writer.Write(proofOfValorPaidRefreshCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		clearGrade = reader.ReadInt32();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		soulPowder = reader.ReadInt32();
		changedCreatureCard = reader.ReadPDPacketData<PDHeroCreatureCard>();
		heroProofOfValorInst = reader.ReadPDPacketData<PDHeroProofOfValorInstance>();
		proofOfValorPaidRefreshCount = reader.ReadInt32();
	}
}
