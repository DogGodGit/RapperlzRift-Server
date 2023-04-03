namespace ClientCommon;

public class PDHeroTodayTask : PDPacketData
{
	public int taskId;

	public int progressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(taskId);
		writer.Write(progressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		taskId = reader.ReadInt32();
		progressCount = reader.ReadInt32();
	}
}
