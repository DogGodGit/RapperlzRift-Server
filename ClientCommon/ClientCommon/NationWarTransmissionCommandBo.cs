namespace ClientCommon;

public class NationWarTransmissionCommandBody : CommandBody
{
	public int targetAreaMonsterArrangeId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetAreaMonsterArrangeId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetAreaMonsterArrangeId = reader.ReadInt32();
	}
}
