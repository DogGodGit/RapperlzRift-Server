namespace ClientCommon;

public class SEBNationFundChangedEventBody : SEBServerEventBody
{
	public int nationId;

	public long fund;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(nationId);
		writer.Write(fund);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		nationId = reader.ReadInt32();
		fund = reader.ReadInt64();
	}
}
