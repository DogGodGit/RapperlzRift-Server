using System;

namespace ClientCommon;

public class TreatOfFarmQuestAcceptResponseBody : ResponseBody
{
	public DateTime date;

	public PDHeroTreatOfFarmQuest heroTreatOfFarmQuest;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(heroTreatOfFarmQuest);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		heroTreatOfFarmQuest = reader.ReadPDPacketData<PDHeroTreatOfFarmQuest>();
	}
}
