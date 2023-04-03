namespace ClientCommon;

public class HeroSearchForFriendApplicationCommandBody : CommandBody
{
	public string text;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(text);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		text = reader.ReadString();
	}
}
public class HeroSearchForFriendApplicationResponseBody : ResponseBody
{
	public PDSearchHero[] results;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(results);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		results = reader.ReadPDPacketDatas<PDSearchHero>();
	}
}
