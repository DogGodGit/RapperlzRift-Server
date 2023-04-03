namespace ClientCommon;

public class PDNationPowerRanking : PDPacketData
{
	public int ranking;

	public int nationId;

	public int nationPower;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(ranking);
		writer.Write(nationId);
		writer.Write(nationPower);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		ranking = reader.ReadInt32();
		nationId = reader.ReadInt32();
		nationPower = reader.ReadInt32();
	}
}
