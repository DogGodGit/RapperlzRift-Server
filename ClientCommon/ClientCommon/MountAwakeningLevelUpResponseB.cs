namespace ClientCommon;

public class MountAwakeningLevelUpResponseBody : ResponseBody
{
	public int maxHP;

	public int awakningLevel;

	public int awakningExp;

	public PDInventorySlot changedInvetorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHP);
		writer.Write(awakningLevel);
		writer.Write(awakningExp);
		writer.Write(changedInvetorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHP = reader.ReadInt32();
		awakningLevel = reader.ReadInt32();
		awakningExp = reader.ReadInt32();
		changedInvetorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
