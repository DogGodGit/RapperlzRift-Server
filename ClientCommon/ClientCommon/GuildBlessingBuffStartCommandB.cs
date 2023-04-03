namespace ClientCommon;

public class GuildBlessingBuffStartCommandBody : CommandBody
{
	public int buffId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(buffId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		buffId = reader.ReadInt32();
	}
}
