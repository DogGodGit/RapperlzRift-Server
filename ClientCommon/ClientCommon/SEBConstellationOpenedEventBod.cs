namespace ClientCommon;

public class SEBConstellationOpenedEventBody : SEBServerEventBody
{
	public PDHeroConstellation[] constellations;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(constellations);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		constellations = reader.ReadPDPacketDatas<PDHeroConstellation>();
	}
}
