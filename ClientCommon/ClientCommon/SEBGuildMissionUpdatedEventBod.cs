namespace ClientCommon;

public class SEBGuildMissionUpdatedEventBody : SEBServerEventBody
{
	public int progressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(progressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		progressCount = reader.ReadInt32();
	}
}
