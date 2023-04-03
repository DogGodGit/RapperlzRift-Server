namespace ClientCommon;

public class GuildBuildingLevelUpResponseBody : ResponseBody
{
	public int level;

	public long fund;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(level);
		writer.Write(fund);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		level = reader.ReadInt32();
		fund = reader.ReadInt64();
	}
}
