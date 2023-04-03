namespace ClientCommon;

public class PartyApplicationAcceptResponseBody : ResponseBody
{
	public PDPartyMember acceptedMember;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(acceptedMember);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		acceptedMember = reader.ReadPDPacketData<PDPartyMember>();
	}
}
