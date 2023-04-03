using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroCostume
{
	private Hero m_hero;

	private Costume m_costume;

	private int m_nCostumeEffectId;

	private int m_nEnchantLevel;

	private int m_nLuckyValue;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	public Hero hero => m_hero;

	public Costume costume => m_costume;

	public int costumeId => m_costume.id;

	public int costumeEffectId
	{
		get
		{
			return m_nCostumeEffectId;
		}
		set
		{
			m_nCostumeEffectId = value;
		}
	}

	public int enchantLevel
	{
		get
		{
			return m_nEnchantLevel;
		}
		set
		{
			m_nEnchantLevel = value;
		}
	}

	public int luckyValue
	{
		get
		{
			return m_nLuckyValue;
		}
		set
		{
			m_nLuckyValue = value;
		}
	}

	public DateTimeOffset regTime => m_regTime;

	public bool isEquipped => m_hero.equippedCostumeId == m_costume.id;

	public HeroCostume(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		Resource res = Resource.instance;
		int nCostumeId = Convert.ToInt32(dr["costumeId"]);
		if (nCostumeId > 0)
		{
			m_costume = res.GetCostume(nCostumeId);
			if (m_costume == null)
			{
				throw new Exception("코스튬이 존재하지 않습니다. nCostumeId = " + nCostumeId);
			}
			m_nCostumeEffectId = Convert.ToInt32(dr["costumeEffectId"]);
			if (m_nCostumeEffectId > 0)
			{
				if (res.GetCostumeEffect(m_nCostumeEffectId) == null)
				{
					throw new Exception("코스튬효과가 존재하지 않습니다. m_nCostumeEffectId = " + m_nCostumeEffectId);
				}
			}
			else if (m_nCostumeEffectId < 0)
			{
				throw new Exception("코스튬효과ID가 유효하지 않습니다. m_nCostumeEffectId = " + m_nCostumeEffectId);
			}
			m_nEnchantLevel = Convert.ToInt32(dr["enchantLevel"]);
			m_nLuckyValue = Convert.ToInt32(dr["luckyValue"]);
			m_regTime = SFDBUtil.ToDateTimeOffset(dr["regTime"], DateTimeOffset.MinValue);
			return;
		}
		throw new Exception("코스튬ID 가유효하지 않습니다. nCostumeId = " + nCostumeId);
	}

	public void Init(Costume costume, DateTimeOffset regTime)
	{
		if (costume == null)
		{
			throw new ArgumentNullException("costume");
		}
		m_costume = costume;
		m_regTime = regTime;
	}

	public void Init(Costume costume, int nCostumeEffectId)
	{
		if (costume == null)
		{
			throw new ArgumentNullException("costume");
		}
		m_costume = costume;
		m_nCostumeEffectId = nCostumeEffectId;
	}

	public float GetRemainingTime(DateTimeOffset time)
	{
		if (m_costume.isUnlimit)
		{
			return 0f;
		}
		DateTimeOffset periodLimitTime = m_regTime.AddDays(m_costume.periodLimitDay);
		return Math.Max(DateTimeUtil.GetTimeSpanSeconds(time, periodLimitTime), 0f);
	}

	public PDHeroCostume ToPDHeroCostume(DateTimeOffset time)
	{
		PDHeroCostume inst = new PDHeroCostume();
		inst.costumeId = m_costume.id;
		inst.costumeEffectId = m_nCostumeEffectId;
		inst.enchantLevel = m_nEnchantLevel;
		inst.luckyValue = m_nLuckyValue;
		inst.remainingTime = GetRemainingTime(time);
		return inst;
	}
}
