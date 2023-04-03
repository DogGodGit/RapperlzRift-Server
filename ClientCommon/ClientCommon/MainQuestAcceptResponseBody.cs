namespace ClientCommon;

public class MainQuestAcceptResponseBody : ResponseBody
{
	public PDMainQuestCartInstance cartInst;

	public int transformationMonsterId;

	public int maxHP;

	public int hp;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(cartInst);
		writer.Write(transformationMonsterId);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		cartInst = reader.ReadPDCartInstance<PDMainQuestCartInstance>();
		transformationMonsterId = reader.ReadInt32();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
