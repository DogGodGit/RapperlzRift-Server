namespace ClientCommon;

public class MainGearTransitResponseBody : ResponseBody
{
	public bool targetOwned;

	public int targetEnchantLevel;

	public PDHeroMainGearOptionAttr[] targetOptionAttrs;

	public bool materialOwned;

	public int materialEnchantLevel;

	public PDHeroMainGearOptionAttr[] materialOptionAttrs;

	public int maxHp;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetOwned);
		writer.Write(targetEnchantLevel);
		writer.Write(targetOptionAttrs);
		writer.Write(materialOwned);
		writer.Write(materialEnchantLevel);
		writer.Write(materialOptionAttrs);
		writer.Write(maxHp);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetOwned = reader.ReadBoolean();
		targetEnchantLevel = reader.ReadInt32();
		targetOptionAttrs = reader.ReadPDPacketDatas<PDHeroMainGearOptionAttr>();
		materialOwned = reader.ReadBoolean();
		materialEnchantLevel = reader.ReadInt32();
		materialOptionAttrs = reader.ReadPDPacketDatas<PDHeroMainGearOptionAttr>();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
