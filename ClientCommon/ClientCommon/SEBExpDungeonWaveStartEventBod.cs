namespace ClientCommon;

public class SEBExpDungeonWaveStartEventBody : SEBServerEventBody
{
	public int waveNo;

	public PDExpDungeonMonsterInstance[] monsterInsts;

	public PDExpDungeonLakChargeMonsterInstance lakChargeMonsterInst;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(waveNo);
		writer.Write(monsterInsts);
		writer.Write(lakChargeMonsterInst);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		waveNo = reader.ReadInt32();
		monsterInsts = reader.ReadPDMonsterInstances<PDExpDungeonMonsterInstance>();
		lakChargeMonsterInst = reader.ReadPDMonsterInstance<PDExpDungeonLakChargeMonsterInstance>();
	}
}
