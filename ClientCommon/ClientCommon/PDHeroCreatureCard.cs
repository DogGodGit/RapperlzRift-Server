namespace ClientCommon;

public class PDHeroCreatureCard : PDPacketData
{
	public int creatureCardId;

	public int count;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(creatureCardId);
		writer.Write(count);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		creatureCardId = reader.ReadInt32();
		count = reader.ReadInt32();
	}
}
