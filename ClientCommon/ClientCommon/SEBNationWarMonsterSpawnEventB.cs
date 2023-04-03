namespace ClientCommon;

public class SEBNationWarMonsterSpawnEventBody : SEBServerEventBody
{
	public int monsterArrangeId;

	public int nationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterArrangeId);
		writer.Write(nationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterArrangeId = reader.ReadInt32();
		nationId = reader.ReadInt32();
	}
}
