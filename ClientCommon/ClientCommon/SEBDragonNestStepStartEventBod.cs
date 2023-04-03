namespace ClientCommon;

public class SEBDragonNestStepStartEventBody : SEBServerEventBody
{
	public int stepNo;

	public PDDragonNestMonsterInstance[] monsterInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(stepNo);
		writer.Write(monsterInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		stepNo = reader.ReadInt32();
		monsterInsts = reader.ReadPDMonsterInstances<PDDragonNestMonsterInstance>();
	}
}
