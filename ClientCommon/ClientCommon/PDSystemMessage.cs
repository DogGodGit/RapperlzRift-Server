using System;

namespace ClientCommon;

public abstract class PDSystemMessage : PDPacketData
{
	public const int kId_MainGearAcquirement = 1;

	public const int kId_MainGearEnchantment = 2;

	public const int kId_CreatureCardAcquirement = 3;

	public const int kId_CreatureAcquirement = 4;

	public const int kId_CreatureInjection = 5;

	public const int kId_CostumeEnchantment = 6;

	public int nationId;

	public Guid heroId;

	public string heroName;

	public abstract int id { get; }

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(nationId);
		writer.Write(heroId);
		writer.Write(heroName);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		nationId = reader.ReadInt32();
		heroId = reader.ReadGuid();
		heroName = reader.ReadString();
	}

	public static PDSystemMessage Create(int nId)
	{
		return nId switch
		{
			1 => new PDMainGearAcquirementSystemMessage(), 
			2 => new PDMainGearEnchantmentSystemMessage(), 
			3 => new PDCreatureCardAcquirementSystemMessage(), 
			4 => new PDCreatureAcquirementSystemMessage(), 
			5 => new PDCreatureInjectionSystemMessage(), 
			6 => new PDCostumeEnchantmentSystemMessage(), 
			_ => null, 
		};
	}
}
