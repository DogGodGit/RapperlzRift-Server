namespace ClientCommon;

public class SEBNationAllianceConcludedEventBody : SEBServerEventBody
{
	public PDNationAlliance nationAlliance;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(nationAlliance);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		nationAlliance = reader.ReadPDPacketData<PDNationAlliance>();
	}
}
