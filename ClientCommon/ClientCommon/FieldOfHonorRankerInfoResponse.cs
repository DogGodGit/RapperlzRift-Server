namespace ClientCommon;

public class FieldOfHonorRankerInfoResponseBody : ResponseBody
{
	public PDFieldOfHonorHero ranker;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(ranker);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		ranker = reader.ReadPDPacketData<PDFieldOfHonorHero>();
	}
}
