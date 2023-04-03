namespace ClientCommon;

public class SEBProofOfValorRefreshedEventBody : SEBServerEventBody
{
	public PDHeroProofOfValorInstance heroProofOfValorInst;

	public int proofOfValorPaidRefreshCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroProofOfValorInst);
		writer.Write(proofOfValorPaidRefreshCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroProofOfValorInst = reader.ReadPDPacketData<PDHeroProofOfValorInstance>();
		proofOfValorPaidRefreshCount = reader.ReadInt32();
	}
}
