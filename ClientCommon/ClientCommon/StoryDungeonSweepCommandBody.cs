namespace ClientCommon;

public class StoryDungeonSweepCommandBody : CommandBody
{
	public int dungeonNo;

	public int difficulty;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(dungeonNo);
		writer.Write(difficulty);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		dungeonNo = reader.ReadInt32();
		difficulty = reader.ReadInt32();
	}
}
