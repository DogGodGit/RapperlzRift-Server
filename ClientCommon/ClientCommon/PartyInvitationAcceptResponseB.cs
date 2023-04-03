namespace ClientCommon;

public class PartyInvitationAcceptResponseBody : ResponseBody
{
	public PDParty party;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(party);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		party = reader.ReadPDPacketData<PDParty>();
	}
}
