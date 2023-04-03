namespace ClientCommon;

public class SEBSceneryQuestCanceledEventBody : SEBServerEventBody
{
	public int questId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(questId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		questId = reader.ReadInt32();
	}
}
