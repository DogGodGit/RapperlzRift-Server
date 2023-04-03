namespace ClientCommon;

public class RookieGiftReceiveResponseBody : ResponseBody
{
	public int rookieGiftNo;

	public float rookieGiftRemainingTime;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(rookieGiftNo);
		writer.Write(rookieGiftRemainingTime);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		rookieGiftNo = reader.ReadInt32();
		rookieGiftRemainingTime = reader.ReadSingle();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
