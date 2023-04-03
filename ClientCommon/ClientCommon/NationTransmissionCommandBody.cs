namespace ClientCommon;

public class NationTransmissionCommandBody : CommandBody
{
	public int npcId;

	public int nationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(npcId);
		writer.Write(nationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		npcId = reader.ReadInt32();
		nationId = reader.ReadInt32();
	}
}
