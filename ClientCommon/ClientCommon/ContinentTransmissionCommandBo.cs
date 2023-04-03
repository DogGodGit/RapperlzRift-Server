namespace ClientCommon;

public class ContinentTransmissionCommandBody : CommandBody
{
	public int npcId;

	public int exitNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(npcId);
		writer.Write(exitNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		npcId = reader.ReadInt32();
		exitNo = reader.ReadInt32();
	}
}
