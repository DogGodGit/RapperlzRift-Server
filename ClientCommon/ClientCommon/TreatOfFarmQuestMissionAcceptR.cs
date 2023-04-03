namespace ClientCommon;

public class TreatOfFarmQuestMissionAcceptResponseBody : ResponseBody
{
	public PDHeroTreatOfFarmQuestMission heroTreatOfFarmQuestMission;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroTreatOfFarmQuestMission);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroTreatOfFarmQuestMission = reader.ReadPDPacketData<PDHeroTreatOfFarmQuestMission>();
	}
}
