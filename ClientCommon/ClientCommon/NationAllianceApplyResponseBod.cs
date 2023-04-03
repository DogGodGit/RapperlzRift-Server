namespace ClientCommon;

public class NationAllianceApplyResponseBody : ResponseBody
{
	public long fund;

	public PDNationAllianceApplication application;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(fund);
		writer.Write(application);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		fund = reader.ReadInt64();
		application = reader.ReadPDPacketData<PDNationAllianceApplication>();
	}
}
