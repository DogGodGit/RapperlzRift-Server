namespace ClientCommon;

public class GuildDailyObjectiveCompletionMemberListCommandBody : CommandBody
{
}
public class GuildDailyObjectiveCompletionMemberListResponseBody : ResponseBody
{
	public PDGuildDailyObjectiveCompletionMember[] completionMembers;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(completionMembers);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		completionMembers = reader.ReadPDPacketDatas<PDGuildDailyObjectiveCompletionMember>();
	}
}
