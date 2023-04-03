namespace ClientCommon;

public class SEBArtifactRoomStartEventBody : SEBServerEventBody
{
	public PDArtifactRoomMonsterInstance[] monsterInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInsts = reader.ReadPDMonsterInstances<PDArtifactRoomMonsterInstance>();
	}
}
