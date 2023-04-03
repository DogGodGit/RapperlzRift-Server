using System;

namespace ClientCommon;

public class MailReceiveAllResponseBody : ResponseBody
{
	public Guid[] receivedMails;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(receivedMails);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		receivedMails = reader.ReadGuids();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
