namespace ClientCommon;

public class SEBRuinsReclaimMatchingRoomBanishedEventBody : SEBServerEventBody
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
