namespace ClientCommon;

public class SEBPartyMembersUpdatedEventBody : SEBServerEventBody
{
	public PDPartyMember[] members;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(members);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		members = reader.ReadPDPacketDatas<PDPartyMember>();
	}
}
