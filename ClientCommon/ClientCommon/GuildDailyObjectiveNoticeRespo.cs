namespace ClientCommon;

public class GuildDailyObjectiveNoticeResponseBody : ResponseBody
{
	public float remainingCoolTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(remainingCoolTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		remainingCoolTime = reader.ReadSingle();
	}
}
