namespace ClientCommon;

public class SceneryQuestStartResponseBody : ResponseBody
{
	public float remainingTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(remainingTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		remainingTime = reader.ReadSingle();
	}
}
