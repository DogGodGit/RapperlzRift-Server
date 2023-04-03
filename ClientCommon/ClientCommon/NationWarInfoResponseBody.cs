namespace ClientCommon;

public class NationWarInfoResponseBody : ResponseBody
{
	public PDSimpleNationWarMonsterInstance[] monsterInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInsts = reader.ReadPDPacketDatas<PDSimpleNationWarMonsterInstance>();
	}
}
