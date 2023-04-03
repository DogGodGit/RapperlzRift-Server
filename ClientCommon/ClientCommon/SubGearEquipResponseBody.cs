namespace ClientCommon;

public class SubGearEquipResponseBody : ResponseBody
{
	public int maxHp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHp = reader.ReadInt32();
	}
}
