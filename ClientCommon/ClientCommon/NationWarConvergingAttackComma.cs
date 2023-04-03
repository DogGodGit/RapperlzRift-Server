namespace ClientCommon;

public class NationWarConvergingAttackCommandBody : CommandBody
{
	public int targetMonsterArrangeId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetMonsterArrangeId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetMonsterArrangeId = reader.ReadInt32();
	}
}
