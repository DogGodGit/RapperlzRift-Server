namespace ClientCommon;

public class PDHeroSubQuest : PDPacketData
{
	public int questId;

	public int progressCount;

	public int status;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(questId);
		writer.Write(progressCount);
		writer.Write(status);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		questId = reader.ReadInt32();
		progressCount = reader.ReadInt32();
		status = reader.ReadInt32();
	}
}
