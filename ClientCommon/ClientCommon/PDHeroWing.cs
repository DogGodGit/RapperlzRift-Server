namespace ClientCommon;

public class PDHeroWing : PDPacketData
{
	public int wingId;

	public int memoryPieceStep;

	public PDHeroWingMemoryPieceSlot[] memoryPieceSlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(wingId);
		writer.Write(memoryPieceStep);
		writer.Write(memoryPieceSlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		wingId = reader.ReadInt32();
		memoryPieceStep = reader.ReadInt32();
		memoryPieceSlots = reader.ReadPDPacketDatas<PDHeroWingMemoryPieceSlot>();
	}
}
