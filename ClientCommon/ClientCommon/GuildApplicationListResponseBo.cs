namespace ClientCommon;

public class GuildApplicationListResponseBody : ResponseBody
{
	public PDGuildApplication[] applications;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(applications);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		applications = reader.ReadPDPacketDatas<PDGuildApplication>();
	}
}
