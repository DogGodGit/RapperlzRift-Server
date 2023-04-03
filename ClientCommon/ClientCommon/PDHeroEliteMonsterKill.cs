namespace ClientCommon;

public class PDHeroEliteMonsterKill : PDPacketData
{
	public int eliteMonsterId;

	public int killCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(eliteMonsterId);
		writer.Write(killCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		eliteMonsterId = reader.ReadInt32();
		killCount = reader.ReadInt32();
	}
}
