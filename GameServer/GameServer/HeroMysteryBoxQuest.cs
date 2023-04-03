using System;
using System.Data;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroMysteryBoxQuest
{
	private Guid m_id = Guid.Empty;

	private Hero m_hero;

	private int m_nPickCount;

	private int m_nPickedBoxGrade;

	private Timer m_pickTimer;

	public Guid id => m_id;

	public Hero hero => m_hero;

	public int pickCount => m_nPickCount;

	public int pickedBoxGrade => m_nPickedBoxGrade;

	public bool isPicking => m_pickTimer != null;

	public HeroMysteryBoxQuest(Hero hero)
	{
		m_id = Guid.NewGuid();
		m_hero = hero;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["instanceId"];
		m_nPickCount = Convert.ToInt32(dr["count"]);
		m_nPickedBoxGrade = Convert.ToInt32(dr["grade"]);
	}

	public void StartPick()
	{
		int nDuration = Resource.instance.mysteryBoxQuest.interactionDuration * 1000;
		if (nDuration < 0)
		{
			nDuration = 0;
		}
		m_pickTimer = new Timer(OnPickTimerTick);
		m_pickTimer.Change(nDuration, -1);
		if (m_hero.currentPlace != null)
		{
			ServerEvent.SendHeroMysteryBoxPickStarted(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.id);
		}
	}

	private void OnPickTimerTick(object state)
	{
		m_hero.AddWork(new SFAction(ProcessPickTimerTick), bGlobalLockRequired: false);
	}

	private void ProcessPickTimerTick()
	{
		if (isPicking)
		{
			DisposePickTimer();
			m_nPickCount++;
			m_nPickedBoxGrade = PickBoxGrade();
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateMysteryBoxQuest_Pick(m_id, m_nPickCount, m_nPickedBoxGrade));
			dbWork.Schedule();
			ServerEvent.SendMysteryBoxPickCompleted(m_hero.account.peer, m_nPickCount, m_nPickedBoxGrade);
			if (m_hero.currentPlace != null)
			{
				ServerEvent.SendHeroMysteryBoxPickCompleted(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.id, m_nPickedBoxGrade);
			}
		}
	}

	private int PickBoxGrade()
	{
		MysteryBoxQuest quest = Resource.instance.mysteryBoxQuest;
		MysteryBoxGradePool pool = quest.defaultBoxGradePool;
		if (m_nPickCount >= quest.vipBoostMinPickCount)
		{
			pool = m_hero.vipLevel.mysteryBoxBoostGradePool;
		}
		MysteryBoxGradePoolEntry entry = null;
		if (pool != null)
		{
			entry = pool.SelectEntry();
		}
		return entry?.grade ?? 1;
	}

	public void CancelPick(bool bSendEventToOwner, bool bSendEventToOthers)
	{
		if (isPicking)
		{
			DisposePickTimer();
			if (bSendEventToOwner)
			{
				ServerEvent.SendMysteryBoxPickCanceled(m_hero.account.peer);
			}
			if (bSendEventToOthers && m_hero.currentPlace != null)
			{
				ServerEvent.SendHeroMysteryBoxPickCanceled(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.id);
			}
		}
	}

	private void DisposePickTimer()
	{
		if (m_pickTimer != null)
		{
			m_pickTimer.Dispose();
			m_pickTimer = null;
		}
	}

	public void Release()
	{
		DisposePickTimer();
	}

	public PDHeroMysteryBoxQuest ToPDHeroMysteryBoxQuest()
	{
		PDHeroMysteryBoxQuest inst = new PDHeroMysteryBoxQuest();
		inst.pickCount = m_nPickCount;
		inst.pickedBoxGrade = m_nPickedBoxGrade;
		return inst;
	}
}
