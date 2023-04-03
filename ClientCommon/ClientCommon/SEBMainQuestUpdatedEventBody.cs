namespace ClientCommon;

public class SEBMainQuestUpdatedEventBody : SEBServerEventBody
{
	public int mainQuestNo;

	public int progressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(mainQuestNo);
		writer.Write(progressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		mainQuestNo = reader.ReadInt32();
		progressCount = reader.ReadInt32();
	}
}
