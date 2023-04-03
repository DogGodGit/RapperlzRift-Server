namespace ClientCommon;

public class BattleSettingSetCommandBody : CommandBody
{
	public int lootingItemMinGrade;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(lootingItemMinGrade);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		lootingItemMinGrade = reader.ReadInt32();
	}
}
