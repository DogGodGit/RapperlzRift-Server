namespace ClientCommon;

public class SEBChattingMessageReceivedEventBody : SEBServerEventBody
{
	public int type;

	public string[] messages;

	public PDChattingLink link;

	public PDSimpleHero sender;

	public PDSimpleHero target;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(type);
		writer.Write(messages);
		writer.Write(link);
		writer.Write(sender);
		writer.Write(target);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		type = reader.ReadInt32();
		messages = reader.ReadStrings();
		link = reader.ReadPDChattingLink();
		sender = reader.ReadPDPacketData<PDSimpleHero>();
		target = reader.ReadPDPacketData<PDSimpleHero>();
	}
}
