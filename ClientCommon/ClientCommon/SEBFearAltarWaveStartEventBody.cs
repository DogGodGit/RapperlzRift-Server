namespace ClientCommon;

public class SEBFearAltarWaveStartEventBody : SEBServerEventBody
{
	public int waveNo;

	public PDFearAltarMonsterInstance[] monsterInsts;

	public PDFearAltarHalidomMonsterInstance halidomMonsterInst;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(waveNo);
		writer.Write(monsterInsts);
		writer.Write(halidomMonsterInst);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		waveNo = reader.ReadInt32();
		monsterInsts = reader.ReadPDMonsterInstances<PDFearAltarMonsterInstance>();
		halidomMonsterInst = reader.ReadPDMonsterInstance<PDFearAltarHalidomMonsterInstance>();
	}
}
