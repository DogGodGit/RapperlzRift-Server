namespace ClientCommon;

public class CostumeItemUseResponseBody : ResponseBody
{
	public int costumeId;

	public float remainingTime;

	public PDInventorySlot changedInventorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(costumeId);
		writer.Write(remainingTime);
		writer.Write(changedInventorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		costumeId = reader.ReadInt32();
		remainingTime = reader.ReadSingle();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
