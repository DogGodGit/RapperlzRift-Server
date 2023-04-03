namespace ClientCommon;

public class SEBOsirisRoomMonsterSpawnEventBody : SEBServerEventBody
{
	public PDOsirisRoomMonsterInstance monsterInst;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInst);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInst = reader.ReadPDMonsterInstance<PDOsirisRoomMonsterInstance>();
	}
}
