namespace ClientCommon;

public class StoryDungeonMonsterTameCommandBody : CommandBody
{
	public long monsterInstanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInstanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInstanceId = reader.ReadInt64();
	}
}
