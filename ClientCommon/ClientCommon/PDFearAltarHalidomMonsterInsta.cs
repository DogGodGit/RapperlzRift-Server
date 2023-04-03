namespace ClientCommon;

public class PDFearAltarHalidomMonsterInstance : PDMonsterInstance
{
	public int halidomId;

	public override MonsterInstanceType type => MonsterInstanceType.FearAltarHalidomMonster;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(halidomId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		halidomId = reader.ReadInt32();
	}
}
