namespace ClientCommon;

public class GuildAltarDefenseMissionStartResponseBody : ResponseBody
{
	public long monsterInstanceId;

	public PDVector3 monsterPosition;

	public float remainingCoolTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInstanceId);
		writer.Write(monsterPosition);
		writer.Write(remainingCoolTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInstanceId = reader.ReadInt64();
		monsterPosition = reader.ReadPDVector3();
		remainingCoolTime = reader.ReadSingle();
	}
}
