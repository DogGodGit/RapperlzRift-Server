namespace ClientCommon;

public class MainGearRefinementApplyResponseBody : ResponseBody
{
	public PDHeroMainGearOptionAttr[] optionAttrs;

	public int maxHp;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(optionAttrs);
		writer.Write(maxHp);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		optionAttrs = reader.ReadPDPacketDatas<PDHeroMainGearOptionAttr>();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
