namespace ClientCommon;

public class PartyInviteResponseBody : ResponseBody
{
	public PDPartyInvitation invitation;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(invitation);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		invitation = reader.ReadPDPacketData<PDPartyInvitation>();
	}
}
