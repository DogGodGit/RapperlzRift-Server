namespace ClientCommon;

public class PDHeroBiograhyQuest : PDPacketData
{
	public int questNo;

	public int progressCount;

	public bool completed;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(questNo);
		writer.Write(progressCount);
		writer.Write(completed);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		questNo = reader.ReadInt32();
		progressCount = reader.ReadInt32();
		completed = reader.ReadBoolean();
	}
}
