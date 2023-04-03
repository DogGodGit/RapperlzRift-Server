namespace ClientCommon;

public class NationWarNpcTransmissionCommandBody : CommandBody
{
	public int npcId;

	public int transmissionExitNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(npcId);
		writer.Write(transmissionExitNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		npcId = reader.ReadInt32();
		transmissionExitNo = reader.ReadInt32();
	}
}
