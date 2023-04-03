namespace ClientCommon;

public class BlacklistEntryAddResponseBody : ResponseBody
{
	public PDBlacklistEntry newEntry;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(newEntry);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		newEntry = reader.ReadPDPacketData<PDBlacklistEntry>();
	}
}
