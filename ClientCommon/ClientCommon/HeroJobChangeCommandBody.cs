namespace ClientCommon;

public class HeroJobChangeCommandBody : CommandBody
{
	public int targetJobId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetJobId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetJobId = reader.ReadInt32();
	}
}
