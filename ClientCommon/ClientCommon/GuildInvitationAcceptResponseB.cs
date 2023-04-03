namespace ClientCommon;

public class GuildInvitationAcceptResponseBody : ResponseBody
{
	public PDGuild guild;

	public int memberGrade;

	public int maxHp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(guild);
		writer.Write(memberGrade);
		writer.Write(maxHp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		guild = reader.ReadPDPacketData<PDGuild>();
		memberGrade = reader.ReadInt32();
		maxHp = reader.ReadInt32();
	}
}
