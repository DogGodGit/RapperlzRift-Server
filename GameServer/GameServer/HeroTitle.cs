using System;
using System.Data;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroTitle
{
	private Hero m_hero;

	private Title m_title;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private Timer m_lifetimeTimer;

	private bool m_bReleased;

	public Hero hero => m_hero;

	public Title title => m_title;

	public DateTimeOffset startTime => m_startTime;

	public Timer lifetimeTimer => m_lifetimeTimer;

	public bool released => m_bReleased;

	public bool isDisplayed => this == m_hero.displayTitle;

	public bool isActivated => this == m_hero.activationTitle;

	public HeroTitle(Hero hero)
	{
		m_hero = hero;
	}

	public HeroTitle(Hero hero, Title title, DateTimeOffset startTime)
	{
		m_hero = hero;
		m_title = title;
		m_startTime = startTime;
		Init_LifetimeTimer(startTime);
	}

	public HeroTitle(Hero hero, Title title)
	{
		m_hero = hero;
		m_title = title;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nTitleId = Convert.ToInt32(dr["titleId"]);
		m_title = Resource.instance.GetTitle(nTitleId);
		if (m_title == null)
		{
			throw new Exception("존재하지 않는 칭호입니다. nTitleId = " + nTitleId);
		}
		m_startTime = (DateTimeOffset)dr["startTime"];
	}

	public void Init_LifetimeTimer(DateTimeOffset time)
	{
		int nLifeTime = m_title.lifetime;
		if (nLifeTime > 0)
		{
			float fElapsedTime = (float)(time - m_startTime).TotalSeconds;
			float fRemainingTime = (float)nLifeTime - fElapsedTime;
			if (fRemainingTime > 0f)
			{
				m_lifetimeTimer = new Timer(OnLifetimeTimerTick);
				m_lifetimeTimer.Change((int)(fRemainingTime * 1000f), -1);
			}
			else
			{
				m_hero.DeleteTitle(this, bInit: true);
			}
		}
	}

	private void OnLifetimeTimerTick(object state)
	{
		m_hero.AddWork(new SFAction(ProcessLifetimeTimerTick), bGlobalLockRequired: false);
	}

	private void ProcessLifetimeTimerTick()
	{
		if (!m_bReleased)
		{
			m_hero.DeleteTitle(this, bInit: false);
		}
	}

	private void DisposeLifetimeTimer()
	{
		if (m_lifetimeTimer != null)
		{
			m_lifetimeTimer.Dispose();
			m_lifetimeTimer = null;
		}
	}

	public void Release()
	{
		if (!m_bReleased)
		{
			DisposeLifetimeTimer();
			m_bReleased = true;
		}
	}

	public float GetRemainingTime(DateTimeOffset time)
	{
		int nLifeTime = m_title.lifetime;
		if (nLifeTime <= 0)
		{
			return 0f;
		}
		float fElapsedTime = (float)(time - m_startTime).TotalSeconds;
		return Math.Max((float)nLifeTime - fElapsedTime, 0f);
	}

	public PDHeroTitle ToPDHeroTitle(DateTimeOffset time)
	{
		PDHeroTitle inst = new PDHeroTitle();
		inst.titleId = m_title.id;
		inst.remainingTime = GetRemainingTime(time);
		return inst;
	}
}
