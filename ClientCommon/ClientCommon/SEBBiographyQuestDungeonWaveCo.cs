namespace ClientCommon;

public class SEBBiographyQuestDungeonWaveCompletedEventBody : SEBServerEventBody
{
	public int waveNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(waveNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		waveNo = reader.ReadInt32();
	}
}
