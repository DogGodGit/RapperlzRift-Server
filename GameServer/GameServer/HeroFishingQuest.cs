using System;
using ClientCommon;

namespace GameServer;

public class HeroFishingQuest
{
	private Hero m_hero;

	private DateTime m_date = DateTime.MinValue.Date;

	private int m_nStartCount;

	private FishingQuestBait m_bait;

	private int m_nCastingCount;

	public Hero hero => m_hero;

	public DateTime date => m_date;

	public int startCount => m_nStartCount;

	public FishingQuestBait bait => m_bait;

	public int castingCount
	{
		get
		{
			return m_nCastingCount;
		}
		set
		{
			m_nCastingCount = value;
		}
	}

	public bool isCompleted => m_nCastingCount >= Resource.instance.fishingQuest.castingCount;

	public HeroFishingQuest(Hero hero, DateTime date, int nStartCount, FishingQuestBait bait, int nCastingCount)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		if (bait == null)
		{
			throw new ArgumentNullException("bait");
		}
		m_hero = hero;
		m_date = date;
		m_nStartCount = nStartCount;
		m_bait = bait;
		m_nCastingCount = nCastingCount;
	}

	public PDHeroFishingQuest ToPDHeroFishingQuest()
	{
		PDHeroFishingQuest inst = new PDHeroFishingQuest();
		inst.baitItemId = m_bait.itemId;
		inst.castingCount = m_nCastingCount;
		return inst;
	}
}
