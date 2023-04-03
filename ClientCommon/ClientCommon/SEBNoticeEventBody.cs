namespace ClientCommon;

public class SEBNoticeEventBody : SEBServerEventBody
{
	public string content;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(content);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		content = reader.ReadString();
	}
}
