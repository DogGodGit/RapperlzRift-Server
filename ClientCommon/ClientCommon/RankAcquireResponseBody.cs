namespace ClientCommon;

public class RankAcquireResponseBody : ResponseBody
{
	public int maxHP;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHP);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHP = reader.ReadInt32();
	}
}
