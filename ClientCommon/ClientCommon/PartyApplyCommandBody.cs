using System;

namespace ClientCommon;

public class PartyApplyCommandBody : CommandBody
{
	public Guid partyId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(partyId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		partyId = reader.ReadGuid();
	}
}
