namespace ClientCommon;

public class SEBPartyCallEventBody : SEBServerEventBody
{
	public int continentId;

	public int nationId;

	public PDVector3 position;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(continentId);
		writer.Write(nationId);
		writer.Write(position);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		continentId = reader.ReadInt32();
		nationId = reader.ReadInt32();
		position = reader.ReadPDVector3();
	}
}
