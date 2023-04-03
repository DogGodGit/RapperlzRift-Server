namespace ClientCommon;

public class IllustratedBookUseResponseBody : ResponseBody
{
	public int activationIllustratedBookId;

	public int explorationPoint;

	public PDInventorySlot changedInventorySlot;

	public int maxHP;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(activationIllustratedBookId);
		writer.Write(explorationPoint);
		writer.Write(changedInventorySlot);
		writer.Write(maxHP);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		activationIllustratedBookId = reader.ReadInt32();
		explorationPoint = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
