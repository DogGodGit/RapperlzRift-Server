namespace ClientCommon;

public class PDHeroProofOfValorInstance : PDPacketData
{
	public int bossMonsterArrangeId;

	public int creatureCardId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(bossMonsterArrangeId);
		writer.Write(creatureCardId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		bossMonsterArrangeId = reader.ReadInt32();
		creatureCardId = reader.ReadInt32();
	}
}
