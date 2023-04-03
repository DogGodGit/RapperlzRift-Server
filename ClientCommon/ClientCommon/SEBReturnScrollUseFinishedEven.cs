namespace ClientCommon;

public class SEBReturnScrollUseFinishedEventBody : SEBServerEventBody
{
	public PDInventorySlot changedInventorySlot;

	public int targetContinentId;

	public int targetNationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedInventorySlot);
		writer.Write(targetContinentId);
		writer.Write(targetNationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
		targetContinentId = reader.ReadInt32();
		targetNationId = reader.ReadInt32();
	}
}
