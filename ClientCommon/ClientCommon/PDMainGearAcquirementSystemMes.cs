namespace ClientCommon;

public class PDMainGearAcquirementSystemMessage : PDSystemMessage
{
	public int mainGearId;

	public override int id => 1;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(mainGearId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		mainGearId = reader.ReadInt32();
	}
}
