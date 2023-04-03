namespace ClientCommon;

public class CreatureCardCollectionActivateCommandBody : CommandBody
{
	public int collectionId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(collectionId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		collectionId = reader.ReadInt32();
	}
}
public class CreatureCardCollectionActivateResponseBody : ResponseBody
{
	public PDHeroCreatureCard[] changedCreatureCards;

	public int maxHP;

	public int creatureCardCollectionFamePoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedCreatureCards);
		writer.Write(maxHP);
		writer.Write(creatureCardCollectionFamePoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedCreatureCards = reader.ReadPDPacketDatas<PDHeroCreatureCard>();
		maxHP = reader.ReadInt32();
		creatureCardCollectionFamePoint = reader.ReadInt32();
	}
}
