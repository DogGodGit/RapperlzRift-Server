namespace ClientCommon;

public class PDCreatureInjectionSystemMessage : PDSystemMessage
{
	public int creatureId;

	public int injectionLevel;

	public override int id => 5;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(creatureId);
		writer.Write(injectionLevel);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		creatureId = reader.ReadInt32();
		injectionLevel = reader.ReadInt32();
	}
}
