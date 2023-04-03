namespace ClientCommon;

public class PDHeroConstellation : PDPacketData
{
	public int id;

	public PDHeroConstellationStep[] steps;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(steps);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadInt32();
		steps = reader.ReadPDPacketDatas<PDHeroConstellationStep>();
	}
}
