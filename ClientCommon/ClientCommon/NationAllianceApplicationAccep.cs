using System;

namespace ClientCommon;

public class NationAllianceApplicationAcceptCommandBody : CommandBody
{
	public Guid applicationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(applicationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		applicationId = reader.ReadGuid();
	}
}
public class NationAllianceApplicationAcceptResponseBody : ResponseBody
{
	public PDNationAlliance nationAlliance;

	public long fund;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(nationAlliance);
		writer.Write(fund);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		nationAlliance = reader.ReadPDPacketData<PDNationAlliance>();
		fund = reader.ReadInt64();
	}
}
