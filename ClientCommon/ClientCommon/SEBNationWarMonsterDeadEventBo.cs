namespace ClientCommon;

public class SEBNationWarMonsterDeadEventBody : SEBServerEventBody
{
	public int monsterArrangeId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterArrangeId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterArrangeId = reader.ReadInt32();
	}
}
