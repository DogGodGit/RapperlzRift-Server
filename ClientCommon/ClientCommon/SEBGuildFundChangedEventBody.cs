namespace ClientCommon;

public class SEBGuildFundChangedEventBody : SEBServerEventBody
{
	public long fund;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(fund);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		fund = reader.ReadInt64();
	}
}
