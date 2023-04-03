namespace ClientCommon;

public class SEBSubQuestsAcceptedEventBody : SEBServerEventBody
{
	public PDHeroSubQuest[] subQuests;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(subQuests);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		subQuests = reader.ReadPDPacketDatas<PDHeroSubQuest>();
	}
}
