namespace ClientCommon;

public class GuildListResponseBody : ResponseBody
{
	public PDSimpleGuild[] guilds;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(guilds);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		guilds = reader.ReadPDPacketDatas<PDSimpleGuild>();
	}
}
