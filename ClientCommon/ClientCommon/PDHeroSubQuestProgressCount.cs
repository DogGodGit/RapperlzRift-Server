namespace ClientCommon;

public class PDHeroSubQuestProgressCount : PDPacketData
{
	public int questId;

	public int progressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(questId);
		writer.Write(progressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		questId = reader.ReadInt32();
		progressCount = reader.ReadInt32();
	}
}
