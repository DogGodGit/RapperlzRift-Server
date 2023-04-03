namespace ClientCommon;

public class SEBRuinsReclaimBanishedEventBody : SEBServerEventBody
{
	public int previousContinentId;

	public int previousNationId;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(previousContinentId);
		writer.Write(previousNationId);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		previousContinentId = reader.ReadInt32();
		previousNationId = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
