namespace ClientCommon;

public class SEBAccMonsterKillCountUpdatedEventBody : SEBServerEventBody
{
	public int count;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(count);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		count = reader.ReadInt32();
	}
}
