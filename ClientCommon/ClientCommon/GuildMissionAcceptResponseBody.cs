namespace ClientCommon;

public class GuildMissionAcceptResponseBody : ResponseBody
{
	public PDHeroGuildMission mission;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(mission);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		mission = reader.ReadPDPacketData<PDHeroGuildMission>();
	}
}
