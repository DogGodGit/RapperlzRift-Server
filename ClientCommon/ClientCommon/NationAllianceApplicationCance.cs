using System;

namespace ClientCommon;

public class NationAllianceApplicationCancelCommandBody : CommandBody
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
public class NationAllianceApplicationCancelResponseBody : ResponseBody
{
	public long fund;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(fund);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		fund = reader.ReadInt64();
	}
}
