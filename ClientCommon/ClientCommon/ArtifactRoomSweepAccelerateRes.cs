namespace ClientCommon;

public class ArtifactRoomSweepAccelerateResponseBody : ResponseBody
{
	public int currentFloor;

	public PDItemBooty[] booties;

	public PDInventorySlot[] changedInventorySlots;

	public int ownDia;

	public int unOwnDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(currentFloor);
		writer.Write(booties);
		writer.Write(changedInventorySlots);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		currentFloor = reader.ReadInt32();
		booties = reader.ReadPDBooties<PDItemBooty>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
	}
}
