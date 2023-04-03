namespace ClientCommon;

public class ProofOfValorAbandonResponseBody : ResponseBody
{
	public int previousContinentId;

	public int previousNationId;

	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHP;

	public int hp;

	public int soulPowder;

	public PDHeroProofOfValorInstance heroProofOfValorInst;

	public int proofOfValorPaidRefreshCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(previousContinentId);
		writer.Write(previousNationId);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(soulPowder);
		writer.Write(heroProofOfValorInst);
		writer.Write(proofOfValorPaidRefreshCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		previousContinentId = reader.ReadInt32();
		previousNationId = reader.ReadInt32();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		soulPowder = reader.ReadInt32();
		heroProofOfValorInst = reader.ReadPDPacketData<PDHeroProofOfValorInstance>();
		proofOfValorPaidRefreshCount = reader.ReadInt32();
	}
}
