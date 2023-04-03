namespace ClientCommon;

public class GuildCreateResponseBody : ResponseBody
{
	public PDGuild guild;

	public int guildMemberGrade;

	public int maxHP;

	public int ownDia;

	public int unOnwDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(guild);
		writer.Write(guildMemberGrade);
		writer.Write(maxHP);
		writer.Write(ownDia);
		writer.Write(unOnwDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		guild = reader.ReadPDPacketData<PDGuild>();
		guildMemberGrade = reader.ReadInt32();
		maxHP = reader.ReadInt32();
		ownDia = reader.ReadInt32();
		unOnwDia = reader.ReadInt32();
	}
}
