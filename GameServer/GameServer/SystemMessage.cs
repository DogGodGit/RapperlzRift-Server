using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class SystemMessage
{
	public const int kId_MainGearAcquirement = 1;

	public const int kId_MainGearEnchantment = 2;

	public const int kId_CreatureCardAcquirement = 3;

	public const int kId_CreatureAcquirement = 4;

	public const int kId_CreatureInjection = 5;

	public const int kId_CostumeEnchantment = 6;

	private int m_nId;

	private int m_nConditionValue;

	public int id => m_nId;

	public int conditionValue => m_nConditionValue;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["messageId"]);
		if (m_nId < 0)
		{
			SFLogUtil.Warn(GetType(), "메시지ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nConditionValue = Convert.ToInt32(dr["conditionValue"]);
		if (m_nConditionValue < 0)
		{
			SFLogUtil.Warn(GetType(), "조건값이 유효하지 않습니다. m_nId = " + m_nId + ", m_nConditionValue = " + m_nConditionValue);
		}
	}

	public bool CheckConditionValue(int nValue)
	{
		return m_nConditionValue <= nValue;
	}

	private static void SendSystemMessageAsync(PDSystemMessage systemMessage)
	{
		Global.instance.AddWork(new SFAction<PDSystemMessage>(SendSystemMessage, systemMessage));
	}

	private static void SendSystemMessage(PDSystemMessage systemMessage)
	{
		ServerEvent.SendSystemMessage(Cache.instance.GetClientPeers(Guid.Empty), systemMessage);
	}

	public static void SendMainGearAcquirment(Hero hero, int nMainGearId)
	{
		PDMainGearAcquirementSystemMessage inst = new PDMainGearAcquirementSystemMessage();
		inst.nationId = hero.nationId;
		inst.heroId = (Guid)hero.id;
		inst.heroName = hero.name;
		inst.mainGearId = nMainGearId;
		SendSystemMessageAsync(inst);
	}

	public static void SendMainGearEnchantment(Hero hero, HeroMainGear mainGear)
	{
		PDMainGearEnchantmentSystemMessage inst = new PDMainGearEnchantmentSystemMessage();
		inst.nationId = hero.nationId;
		inst.heroId = (Guid)hero.id;
		inst.heroName = hero.name;
		inst.mainGearId = mainGear.gear.id;
		inst.enchantLevel = mainGear.enchantLevel;
		SendSystemMessageAsync(inst);
	}

	public static void SendCreatureCardAcquirement(Hero hero, int nCreatureCardId)
	{
		PDCreatureCardAcquirementSystemMessage inst = new PDCreatureCardAcquirementSystemMessage();
		inst.nationId = hero.nationId;
		inst.heroId = (Guid)hero.id;
		inst.heroName = hero.name;
		inst.creatureCardId = nCreatureCardId;
		SendSystemMessageAsync(inst);
	}

	public static void SendCreatureAcquirement(Hero hero, int nCreatureId)
	{
		PDCreatureAcquirementSystemMessage inst = new PDCreatureAcquirementSystemMessage();
		inst.nationId = hero.nationId;
		inst.heroId = (Guid)hero.id;
		inst.heroName = hero.name;
		inst.creatureId = nCreatureId;
		SendSystemMessageAsync(inst);
	}

	public static void SendCreatureInjection(Hero hero, HeroCreature heroCreature)
	{
		PDCreatureInjectionSystemMessage inst = new PDCreatureInjectionSystemMessage();
		inst.nationId = hero.nationId;
		inst.heroId = (Guid)hero.id;
		inst.heroName = hero.name;
		inst.creatureId = heroCreature.creature.id;
		inst.injectionLevel = heroCreature.injectionLevel;
		SendSystemMessageAsync(inst);
	}

	public static void SendCostumeEnchantment(Hero hero, HeroCostume heroCostume)
	{
		PDCostumeEnchantmentSystemMessage inst = new PDCostumeEnchantmentSystemMessage();
		inst.nationId = hero.nationId;
		inst.heroId = (Guid)hero.id;
		inst.heroName = hero.name;
		inst.costumeId = heroCostume.costumeId;
		inst.enchantLevel = heroCostume.enchantLevel;
		SendSystemMessageAsync(inst);
	}
}
