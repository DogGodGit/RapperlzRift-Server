namespace ClientCommon;

public class SEBAncientRelicWaveStartEventBody : SEBServerEventBody
{
	public int waveNo;

	public PDAncientRelicMonsterInstance[] monsterInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(waveNo);
		writer.Write(monsterInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		waveNo = reader.ReadInt32();
		monsterInsts = reader.ReadPDMonsterInstances<PDAncientRelicMonsterInstance>();
	}
}
