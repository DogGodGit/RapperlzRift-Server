namespace ClientCommon;

public class SEBNationWarDeadCountUpdatedEventBody : SEBServerEventBody
{
	public int deadCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(deadCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		deadCount = reader.ReadInt32();
	}
}
