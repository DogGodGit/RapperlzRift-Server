namespace ClientCommon;

public class PartySurroundingPartyListResponseBody : ResponseBody
{
	public PDSimpleParty[] parties;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(parties);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		parties = reader.ReadPDPacketDatas<PDSimpleParty>();
	}
}
