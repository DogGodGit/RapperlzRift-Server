namespace ClientCommon;

public class SEBPartyApplicationAcceptedEventBody : SEBServerEventBody
{
	public long applicationNo;

	public PDParty party;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(applicationNo);
		writer.Write(party);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		applicationNo = reader.ReadInt64();
		party = reader.ReadPDPacketData<PDParty>();
	}
}
