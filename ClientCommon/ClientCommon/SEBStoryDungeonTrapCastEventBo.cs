namespace ClientCommon;

public class SEBStoryDungeonTrapCastEventBody : SEBServerEventBody
{
	public int trapId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(trapId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		trapId = reader.ReadInt32();
	}
}
