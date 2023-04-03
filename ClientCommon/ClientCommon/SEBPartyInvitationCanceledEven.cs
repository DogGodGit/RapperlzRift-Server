namespace ClientCommon;

public class SEBPartyInvitationCanceledEventBody : SEBServerEventBody
{
	public long invitationNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(invitationNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		invitationNo = reader.ReadInt64();
	}
}
