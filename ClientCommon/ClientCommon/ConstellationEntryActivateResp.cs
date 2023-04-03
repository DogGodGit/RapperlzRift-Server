namespace ClientCommon;

public class ConstellationEntryActivateResponseBody : ResponseBody
{
	public int maxHP;

	public int starEssense;

	public long gold;

	public bool success;

	public int failPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHP);
		writer.Write(starEssense);
		writer.Write(gold);
		writer.Write(success);
		writer.Write(failPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHP = reader.ReadInt32();
		starEssense = reader.ReadInt32();
		gold = reader.ReadInt64();
		success = reader.ReadBoolean();
		failPoint = reader.ReadInt32();
	}
}
