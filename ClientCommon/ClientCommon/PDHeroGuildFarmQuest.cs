namespace ClientCommon;

public class PDHeroGuildFarmQuest : PDPacketData
{
	public bool isObjectiveCompleted;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(isObjectiveCompleted);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		isObjectiveCompleted = reader.ReadBoolean();
	}
}
