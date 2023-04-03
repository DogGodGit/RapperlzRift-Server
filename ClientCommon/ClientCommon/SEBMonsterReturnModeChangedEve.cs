namespace ClientCommon;

public class SEBMonsterReturnModeChangedEventBody : SEBServerEventBody
{
	public long instanceId;

	public bool isReturnMode;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(isReturnMode);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		isReturnMode = reader.ReadBoolean();
	}
}
