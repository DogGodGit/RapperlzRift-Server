namespace ClientCommon;

public class ChattingMessageSendResponseBody : ResponseBody
{
	public PDInventorySlot changedInventorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedInventorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
