namespace ClientCommon;

public class StoryDungeonMonsterTameResponseBody : ResponseBody
{
	public PDVector3 position;

	public float rotationY;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(position);
		writer.Write(rotationY);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
	}
}
