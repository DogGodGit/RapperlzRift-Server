namespace ClientCommon;

public class SEBNationAllianceAppliedEventBody : SEBServerEventBody
{
	public PDNationAllianceApplication application;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(application);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		application = reader.ReadPDPacketData<PDNationAllianceApplication>();
	}
}
