using System;

namespace ClientCommon;

public class ProofOfValorRefreshResponseBody : ResponseBody
{
	public PDHeroProofOfValorInstance heroProofOfValorInst;

	public DateTime date;

	public int dailyProofOfValorFreeRefreshCount;

	public int dailyProofOfValorPaidRefreshCount;

	public int proofOfValorPaidRefreshCount;

	public int ownDia;

	public int unOwnDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroProofOfValorInst);
		writer.Write(date);
		writer.Write(dailyProofOfValorFreeRefreshCount);
		writer.Write(dailyProofOfValorPaidRefreshCount);
		writer.Write(proofOfValorPaidRefreshCount);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroProofOfValorInst = reader.ReadPDPacketData<PDHeroProofOfValorInstance>();
		date = reader.ReadDateTime();
		dailyProofOfValorFreeRefreshCount = reader.ReadInt32();
		dailyProofOfValorPaidRefreshCount = reader.ReadInt32();
		proofOfValorPaidRefreshCount = reader.ReadInt32();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
	}
}
