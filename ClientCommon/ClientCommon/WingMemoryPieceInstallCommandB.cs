namespace ClientCommon;

public class WingMemoryPieceInstallCommandBody : CommandBody
{
	public int wingId;

	public int wingMemoryPieceType;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(wingId);
		writer.Write(wingMemoryPieceType);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		wingId = reader.ReadInt32();
		wingMemoryPieceType = reader.ReadInt32();
	}
}
