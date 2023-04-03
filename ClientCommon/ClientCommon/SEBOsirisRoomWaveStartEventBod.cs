namespace ClientCommon;

public class SEBOsirisRoomWaveStartEventBody : SEBServerEventBody
{
	public int waveNo;

	public int monsterCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(waveNo);
		writer.Write(monsterCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		waveNo = reader.ReadInt32();
		monsterCount = reader.ReadInt32();
	}
}
