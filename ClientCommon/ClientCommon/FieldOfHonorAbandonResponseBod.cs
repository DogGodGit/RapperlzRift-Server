namespace ClientCommon;

public class FieldOfHonorAbandonResponseBody : ResponseBody
{
	public int previousContinentId;

	public int previousNationId;

	public int hp;

	public int successiveCount;

	public int honorPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(previousContinentId);
		writer.Write(previousNationId);
		writer.Write(hp);
		writer.Write(successiveCount);
		writer.Write(honorPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		previousContinentId = reader.ReadInt32();
		previousNationId = reader.ReadInt32();
		hp = reader.ReadInt32();
		successiveCount = reader.ReadInt32();
		honorPoint = reader.ReadInt32();
	}
}
