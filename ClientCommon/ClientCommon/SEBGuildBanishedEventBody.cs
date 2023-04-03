namespace ClientCommon;

public class SEBGuildBanishedEventBody : SEBServerEventBody
{
	public int maxHp;

	public int hp;

	public int previousContinentId;

	public int previousNationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(previousContinentId);
		writer.Write(previousNationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		previousContinentId = reader.ReadInt32();
		previousNationId = reader.ReadInt32();
	}
}
