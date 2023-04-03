namespace ClientCommon;

public class RuneSocketUnmountResponseBody : ResponseBody
{
	public PDInventorySlot changedInventorySlot;

	public int maxHp;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedInventorySlot);
		writer.Write(maxHp);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
