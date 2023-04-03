namespace ClientCommon;

public class SEBEliteMonsterKillCountUpdatedEventBody : SEBServerEventBody
{
	public int eliteMonsterId;

	public int killCount;

	public int maxHP;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(eliteMonsterId);
		writer.Write(killCount);
		writer.Write(maxHP);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		eliteMonsterId = reader.ReadInt32();
		killCount = reader.ReadInt32();
		maxHP = reader.ReadInt32();
	}
}
