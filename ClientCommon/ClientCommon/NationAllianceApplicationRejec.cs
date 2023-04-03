using System;

namespace ClientCommon;

public class NationAllianceApplicationRejectCommandBody : CommandBody
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
public class NationAllianceApplicationRejectResponseBody : ResponseBody
{
}
