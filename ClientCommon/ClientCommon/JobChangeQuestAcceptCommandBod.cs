namespace ClientCommon;

public class JobChangeQuestAcceptCommandBody : CommandBody
{
	public int questNo;

	public int difficulty;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(questNo);
		writer.Write(difficulty);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		questNo = reader.ReadInt32();
		difficulty = reader.ReadInt32();
	}
}
