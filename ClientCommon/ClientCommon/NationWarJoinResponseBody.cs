namespace ClientCommon;

public class NationWarJoinResponseBody : ResponseBody
{
	public int targetContinentId;

	public int targetNationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetContinentId);
		writer.Write(targetNationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetContinentId = reader.ReadInt32();
		targetNationId = reader.ReadInt32();
	}
}
