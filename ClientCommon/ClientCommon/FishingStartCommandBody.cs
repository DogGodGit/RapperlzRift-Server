namespace ClientCommon;

public class FishingStartCommandBody : CommandBody
{
	public int spotId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(spotId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		spotId = reader.ReadInt32();
	}
}
