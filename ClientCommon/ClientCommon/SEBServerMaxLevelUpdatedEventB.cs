namespace ClientCommon;

public class SEBServerMaxLevelUpdatedEventBody : SEBServerEventBody
{
	public int serverMaxLevel;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(serverMaxLevel);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		serverMaxLevel = reader.ReadInt32();
	}
}
