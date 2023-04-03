namespace ClientCommon;

public class SEBWisdomTempleFakeTreasureBoxKillEventBody : SEBServerEventBody
{
	public int row;

	public int col;

	public bool existAroundRealTreasureBox;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(row);
		writer.Write(col);
		writer.Write(existAroundRealTreasureBox);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		row = reader.ReadInt32();
		col = reader.ReadInt32();
		existAroundRealTreasureBox = reader.ReadBoolean();
	}
}
