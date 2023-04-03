namespace ClientCommon;

public class PDStoryDungeonPlay : PDPacketData
{
	public int dungeonNo;

	public int count;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(dungeonNo);
		writer.Write(count);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		dungeonNo = reader.ReadInt32();
		count = reader.ReadInt32();
	}
}
