namespace ClientCommon;

public class SEBCostumePeriodExpiredEventBody : SEBServerEventBody
{
	public int costumeId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(costumeId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		costumeId = reader.ReadInt32();
	}
}
