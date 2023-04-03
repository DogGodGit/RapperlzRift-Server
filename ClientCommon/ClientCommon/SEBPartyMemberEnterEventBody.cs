namespace ClientCommon;

public class SEBPartyMemberEnterEventBody : SEBServerEventBody
{
	public PDPartyMember member;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(member);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		member = reader.ReadPDPacketData<PDPartyMember>();
	}
}
