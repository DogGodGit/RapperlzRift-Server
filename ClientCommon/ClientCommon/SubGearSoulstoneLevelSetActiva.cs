namespace ClientCommon;

public class SubGearSoulstoneLevelSetActivateCommandBody : CommandBody
{
	public int setNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(setNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		setNo = reader.ReadInt32();
	}
}
public class SubGearSoulstoneLevelSetActivateResponseBody : ResponseBody
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
