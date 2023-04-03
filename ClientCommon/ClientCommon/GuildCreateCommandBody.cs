namespace ClientCommon;

public class GuildCreateCommandBody : CommandBody
{
	public string guildName;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(guildName);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		guildName = reader.ReadString();
	}
}
