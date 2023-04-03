namespace ClientCommon;

public class PDStoryDungeonClear : PDPacketData
{
	public int dungeonNo;

	public int clearMaxDifficulty;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(dungeonNo);
		writer.Write(clearMaxDifficulty);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		dungeonNo = reader.ReadInt32();
		clearMaxDifficulty = reader.ReadInt32();
	}
}
