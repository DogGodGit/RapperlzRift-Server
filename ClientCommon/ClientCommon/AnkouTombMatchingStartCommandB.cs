namespace ClientCommon;

public class AnkouTombMatchingStartCommandBody : CommandBody
{
	public bool isPartyEntrance;

	public int difficulty;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(isPartyEntrance);
		writer.Write(difficulty);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		isPartyEntrance = reader.ReadBoolean();
		difficulty = reader.ReadInt32();
	}
}
