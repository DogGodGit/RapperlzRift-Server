namespace ClientCommon;

public class PDCreatureCardAcquirementSystemMessage : PDSystemMessage
{
	public int creatureCardId;

	public override int id => 3;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(creatureCardId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		creatureCardId = reader.ReadInt32();
	}
}
