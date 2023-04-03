namespace ClientCommon;

public class CreatureCardDisassembleCommandBody : CommandBody
{
	public int cardId;

	public int count;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(cardId);
		writer.Write(count);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		cardId = reader.ReadInt32();
		count = reader.ReadInt32();
	}
}
