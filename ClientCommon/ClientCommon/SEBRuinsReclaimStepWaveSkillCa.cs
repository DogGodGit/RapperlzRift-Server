namespace ClientCommon;

public class SEBRuinsReclaimStepWaveSkillCastEventBody : SEBServerEventBody
{
	public PDVector3 targetPosition;

	public PDRuinsReclaimMonsterTransformationCancelObjectInstance[] createdObjectInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetPosition);
		writer.Write(createdObjectInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetPosition = reader.ReadPDVector3();
		createdObjectInsts = reader.ReadPDPacketDatas<PDRuinsReclaimMonsterTransformationCancelObjectInstance>();
	}
}
