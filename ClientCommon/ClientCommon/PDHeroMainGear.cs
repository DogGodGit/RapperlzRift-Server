using System;

namespace ClientCommon;

public class PDHeroMainGear : PDPacketData
{
	public Guid id;

	public int mainGearId;

	public int enchantLevel;

	public PDHeroMainGear()
	{
	}

	public PDHeroMainGear(Guid id, int nMainGearId, int nEnchantLevel)
	{
		this.id = id;
		mainGearId = nMainGearId;
		enchantLevel = nEnchantLevel;
	}

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(mainGearId);
		writer.Write(enchantLevel);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		mainGearId = reader.ReadInt32();
		enchantLevel = reader.ReadInt32();
	}
}
