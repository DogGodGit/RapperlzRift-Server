namespace ClientCommon;

public class SEBWisdomTempleStepStartEventBody : SEBServerEventBody
{
	public int stepNo;

	public int puzzleId;

	public int quizNo;

	public PDWisdomTempleMonsterInstance[] monsterInsts;

	public PDWisdomTempleColorMatchingObjectInstance[] colorMatchingObjectInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(stepNo);
		writer.Write(puzzleId);
		writer.Write(quizNo);
		writer.Write(monsterInsts);
		writer.Write(colorMatchingObjectInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		stepNo = reader.ReadInt32();
		puzzleId = reader.ReadInt32();
		quizNo = reader.ReadInt32();
		monsterInsts = reader.ReadPDMonsterInstances<PDWisdomTempleMonsterInstance>();
		colorMatchingObjectInsts = reader.ReadPDPacketDatas<PDWisdomTempleColorMatchingObjectInstance>();
	}
}
