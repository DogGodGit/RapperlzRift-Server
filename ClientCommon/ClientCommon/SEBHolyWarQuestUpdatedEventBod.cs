namespace ClientCommon;

public class SEBHolyWarQuestUpdatedEventBody : SEBServerEventBody
{
	public int killCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(killCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		killCount = reader.ReadInt32();
	}
}
