namespace ClientCommon;

public class SEBSoulCoveterMatchingRoomBanishedEventBody : SEBServerEventBody
{
	public int banishType;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(banishType);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		banishType = reader.ReadInt32();
	}
}
