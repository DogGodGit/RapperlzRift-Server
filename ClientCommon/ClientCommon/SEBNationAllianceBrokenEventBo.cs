using System;

namespace ClientCommon;

public class SEBNationAllianceBrokenEventBody : SEBServerEventBody
{
	public Guid nationAllianceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(nationAllianceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		nationAllianceId = reader.ReadGuid();
	}
}
