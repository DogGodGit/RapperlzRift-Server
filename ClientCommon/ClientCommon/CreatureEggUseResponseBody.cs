namespace ClientCommon;

public class CreatureEggUseResponseBody : ResponseBody
{
	public PDInventorySlot[] changedInventorySlots;

	public PDHeroCreature[] addedHeroCreatures;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedInventorySlots);
		writer.Write(addedHeroCreatures);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		addedHeroCreatures = reader.ReadPDPacketDatas<PDHeroCreature>();
	}
}
