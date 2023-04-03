namespace ClientCommon;

public class SEBInfiniteWarBuffBoxCreatedEventBody : SEBServerEventBody
{
	public PDInfiniteWarBuffBoxInstance[] buffBoxInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(buffBoxInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		buffBoxInsts = reader.ReadPDPacketDatas<PDInfiniteWarBuffBoxInstance>();
	}
}
