namespace ClientCommon;

public class CreatureCardComposeCommandBody : CommandBody
{
	public int cardId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(cardId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		cardId = reader.ReadInt32();
	}
}
