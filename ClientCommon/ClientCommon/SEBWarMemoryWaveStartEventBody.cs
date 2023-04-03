namespace ClientCommon;

public class SEBWarMemoryWaveStartEventBody : SEBServerEventBody
{
	public int waveNo;

	public PDWarMemoryMonsterInstance[] monsterInsts;

	public PDWarMemoryTransformationObjectInstance[] objectInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(waveNo);
		writer.Write(monsterInsts);
		writer.Write(objectInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		waveNo = reader.ReadInt32();
		monsterInsts = reader.ReadPDMonsterInstances<PDWarMemoryMonsterInstance>();
		objectInsts = reader.ReadPDPacketDatas<PDWarMemoryTransformationObjectInstance>();
	}
}
