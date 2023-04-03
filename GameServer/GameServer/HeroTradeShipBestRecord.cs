using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroTradeShipBestRecord
{
	private Guid m_heroId = Guid.Empty;

	private string m_sHeroName;

	private int m_nHeroJobId;

	private int m_nHeroNationId;

	private int m_nDifficulty;

	private int m_nPoint;

	private DateTimeOffset m_updateTime = DateTimeOffset.MinValue;

	public Guid heroId => m_heroId;

	public string heroName => m_sHeroName;

	public int heroJobId => m_nHeroJobId;

	public int heroNationId => m_nHeroNationId;

	public int difficulty => m_nDifficulty;

	public int point => m_nPoint;

	public DateTimeOffset updateTime => m_updateTime;

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_heroId = SFDBUtil.ToGuid(dr["heroId"], Guid.Empty);
		m_sHeroName = Convert.ToString(dr["name"]);
		m_nHeroJobId = Convert.ToInt32(dr["jobId"]);
		m_nHeroNationId = Convert.ToInt32(dr["nationId"]);
		m_nDifficulty = Convert.ToInt32(dr["difficulty"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
		m_updateTime = SFDBUtil.ToDateTimeOffset(dr["updateTime"], DateTimeOffset.MinValue);
	}

	public void Init(HeroTradeShipPoint heroPoint)
	{
		if (heroPoint == null)
		{
			throw new ArgumentNullException("heroPoint");
		}
		Hero hero = heroPoint.hero;
		m_heroId = hero.id;
		m_sHeroName = hero.name;
		m_nHeroJobId = hero.jobId;
		m_nHeroNationId = hero.nationId;
		m_nDifficulty = heroPoint.difficulty;
		m_nPoint = heroPoint.point;
		m_updateTime = heroPoint.updateTime;
	}

	public PDHeroTradeShipBestRecord ToPDHeroTradeShipBestRecord()
	{
		PDHeroTradeShipBestRecord inst = new PDHeroTradeShipBestRecord();
		inst.heroId = (Guid)m_heroId;
		inst.heroName = m_sHeroName;
		inst.heroJobId = m_nHeroJobId;
		inst.heroNationId = m_nHeroNationId;
		inst.difficulty = m_nDifficulty;
		inst.point = m_nPoint;
		return inst;
	}
}
