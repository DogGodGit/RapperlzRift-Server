namespace ClientCommon;

public class SEBDragonNestClearEventBody : SEBServerEventBody
{
	public PDSimpleHero[] clearedHeroes;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(clearedHeroes);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		clearedHeroes = reader.ReadPDPacketDatas<PDSimpleHero>();
	}
}
