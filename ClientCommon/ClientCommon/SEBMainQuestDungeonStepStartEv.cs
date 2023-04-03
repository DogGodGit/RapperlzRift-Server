namespace ClientCommon;

public class SEBMainQuestDungeonStepStartEventBody : SEBServerEventBody
{
	public int stepNo;

	public PDMainQuestDungeonMonsterInstance[] monsterInsts;

	public PDVector3 heroPosition;

	public float heroRotationY;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(stepNo);
		writer.Write(monsterInsts);
		writer.Write(heroPosition);
		writer.Write(heroRotationY);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		stepNo = reader.ReadInt32();
		monsterInsts = reader.ReadPDMonsterInstances<PDMainQuestDungeonMonsterInstance>();
		heroPosition = reader.ReadPDVector3();
		heroRotationY = reader.ReadSingle();
	}
}
