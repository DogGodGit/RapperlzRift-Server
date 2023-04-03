namespace ClientCommon;

public class ExpDungeonSweepCommandBody : CommandBody
{
	public int difficulty;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(difficulty);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		difficulty = reader.ReadInt32();
	}
}
