namespace ClientCommon;

public class SEBMainQuestDungeonMonsterSummonEventBody : SEBServerEventBody
{
	public long summonerInstanceId;

	public PDMainQuestDungeonSummonMonsterInstance[] summonMonsterInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(summonerInstanceId);
		writer.Write(summonMonsterInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		summonerInstanceId = reader.ReadInt64();
		summonMonsterInsts = reader.ReadPDMonsterInstances<PDMainQuestDungeonSummonMonsterInstance>();
	}
}
