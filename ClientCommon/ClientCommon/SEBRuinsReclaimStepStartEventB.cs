namespace ClientCommon;

public class SEBRuinsReclaimStepStartEventBody : SEBServerEventBody
{
	public int stepNo;

	public PDRuinsReclaimRewardObjectInstance[] objectInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(stepNo);
		writer.Write(objectInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		stepNo = reader.ReadInt32();
		objectInsts = reader.ReadPDPacketDatas<PDRuinsReclaimRewardObjectInstance>();
	}
}
