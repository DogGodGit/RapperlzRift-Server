namespace ClientCommon;

public class TitleItemUseResponseBody : ResponseBody
{
	public int titleId;

	public float remainingTime;

	public PDInventorySlot changedInventorySlot;

	public int maxHP;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(titleId);
		writer.Write(remainingTime);
		writer.Write(changedInventorySlot);
		writer.Write(maxHP);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		titleId = reader.ReadInt32();
		remainingTime = reader.ReadSingle();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
