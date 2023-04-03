namespace ClientCommon;

public class GuildFoodWarehouseInfoResponseBody : ResponseBody
{
	public int level;

	public int exp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(level);
		writer.Write(exp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		level = reader.ReadInt32();
		exp = reader.ReadInt32();
	}
}
