namespace ClientCommon;

public class MountEquipResponseBody : ResponseBody
{
	public int maxHp;

	public int hp;

	public bool isRiding;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(isRiding);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		isRiding = reader.ReadBoolean();
	}
}
