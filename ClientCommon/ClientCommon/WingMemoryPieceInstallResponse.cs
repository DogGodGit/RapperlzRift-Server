namespace ClientCommon;

public class WingMemoryPieceInstallResponseBody : ResponseBody
{
	public int memoryPieceStep;

	public PDHeroWingMemoryPieceSlot[] changedWingMemoryPieceSlots;

	public int maxHP;

	public int hp;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(memoryPieceStep);
		writer.Write(changedWingMemoryPieceSlots);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		memoryPieceStep = reader.ReadInt32();
		changedWingMemoryPieceSlots = reader.ReadPDPacketDatas<PDHeroWingMemoryPieceSlot>();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
