namespace ClientCommon;

public class RankPassiveSkillLevelUpResponseBody : ResponseBody
{
	public int maxHP;

	public long gold;

	public int spiritStone;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHP);
		writer.Write(gold);
		writer.Write(spiritStone);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHP = reader.ReadInt32();
		gold = reader.ReadInt64();
		spiritStone = reader.ReadInt32();
	}
}
