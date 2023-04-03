namespace ClientCommon;

public class SubGearLevelUpCommandBody : CommandBody
{
	public int subGearId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(subGearId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		subGearId = reader.ReadInt32();
	}
}
