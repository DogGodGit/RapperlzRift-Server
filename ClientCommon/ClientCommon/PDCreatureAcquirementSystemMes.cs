namespace ClientCommon;

public class PDCreatureAcquirementSystemMessage : PDSystemMessage
{
	public int creatureId;

	public override int id => 4;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(creatureId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		creatureId = reader.ReadInt32();
	}
}
