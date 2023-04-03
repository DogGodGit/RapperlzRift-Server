namespace ClientCommon;

public class ArtifactEquipCommandBody : CommandBody
{
	public int artifactNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(artifactNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		artifactNo = reader.ReadInt32();
	}
}
