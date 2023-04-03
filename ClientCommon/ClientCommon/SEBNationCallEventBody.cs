namespace ClientCommon;

public class SEBNationCallEventBody : SEBServerEventBody
{
	public PDNationCall call;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(call);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		call = reader.ReadPDPacketData<PDNationCall>();
	}
}
