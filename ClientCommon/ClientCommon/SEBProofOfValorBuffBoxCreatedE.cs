namespace ClientCommon;

public class SEBProofOfValorBuffBoxCreatedEventBody : SEBServerEventBody
{
	public PDProofOfValorBuffBoxInstance[] buffBoxInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(buffBoxInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		buffBoxInsts = reader.ReadPDPacketDatas<PDProofOfValorBuffBoxInstance>();
	}
}
