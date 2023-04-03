using System;

namespace ClientCommon;

public class SEBInterestTargetChangeEventBody : SEBServerEventBody
{
	public PDHero[] addedHeroes;

	public PDMonsterInstance[] addedMonsterInsts;

	public PDContinentObjectInstance[] addedContinentObjectInsts;

	public PDCartInstance[] addedCartInsts;

	public Guid[] removedHeroes;

	public long[] removedMonsterInsts;

	public long[] removedContinentObjectInsts;

	public long[] removedCartInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(addedHeroes);
		writer.Write(addedMonsterInsts);
		writer.Write(addedContinentObjectInsts);
		writer.Write(addedCartInsts);
		writer.Write(removedHeroes);
		writer.Write(removedMonsterInsts);
		writer.Write(removedContinentObjectInsts);
		writer.Write(removedCartInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		addedHeroes = reader.ReadPDPacketDatas<PDHero>();
		addedMonsterInsts = reader.ReadPDMonsterInstances();
		addedContinentObjectInsts = reader.ReadPDPacketDatas<PDContinentObjectInstance>();
		addedCartInsts = reader.ReadPDCartInstances();
		removedHeroes = reader.ReadGuids();
		removedMonsterInsts = reader.ReadLongs();
		removedContinentObjectInsts = reader.ReadLongs();
		removedCartInsts = reader.ReadLongs();
	}
}
