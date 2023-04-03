namespace ClientCommon;

public class SEBGuildNoticeChangedEventBody : SEBServerEventBody
{
	public string notice;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(notice);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		notice = reader.ReadString();
	}
}
