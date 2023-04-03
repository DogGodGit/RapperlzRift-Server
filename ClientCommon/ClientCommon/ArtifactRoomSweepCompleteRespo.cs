namespace ClientCommon;

public class ArtifactRoomSweepCompleteResponseBody : ResponseBody
{
	public int currentFloor;

	public PDItemBooty[] booties;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(currentFloor);
		writer.Write(booties);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		currentFloor = reader.ReadInt32();
		booties = reader.ReadPDBooties<PDItemBooty>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
