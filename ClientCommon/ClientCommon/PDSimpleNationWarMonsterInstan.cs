namespace ClientCommon;

public class PDSimpleNationWarMonsterInstance : PDPacketData
{
	public int monsterArrangeId;

	public int nationId;

	public int monsterMaxHp;

	public int monsterHp;

	public bool isBattleMode;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterArrangeId);
		writer.Write(nationId);
		writer.Write(monsterMaxHp);
		writer.Write(monsterHp);
		writer.Write(isBattleMode);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterArrangeId = reader.ReadInt32();
		nationId = reader.ReadInt32();
		monsterMaxHp = reader.ReadInt32();
		monsterHp = reader.ReadInt32();
		isBattleMode = reader.ReadBoolean();
	}
}
