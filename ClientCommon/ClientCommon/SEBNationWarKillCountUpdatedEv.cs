namespace ClientCommon;

public class SEBNationWarKillCountUpdatedEventBody : SEBServerEventBody
{
	public int killCount;

	public int accKillCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(killCount);
		writer.Write(accKillCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		killCount = reader.ReadInt32();
		accKillCount = reader.ReadInt32();
	}
}
