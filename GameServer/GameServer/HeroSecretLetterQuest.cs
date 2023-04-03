using System;
using System.Data;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroSecretLetterQuest
{
	private Guid m_id = Guid.Empty;

	private Hero m_hero;

	private int m_nTargetNationId;

	private int m_nPickCount;

	private int m_nPickedLetterGrade;

	private Timer m_pickTimer;

	public Guid id => m_id;

	public Hero hero => m_hero;

	public int targetNationId => m_nTargetNationId;

	public int pickCount => m_nPickCount;

	public int pickedLetterGrade => m_nPickedLetterGrade;

	public bool isPicking => m_pickTimer != null;

	public HeroSecretLetterQuest(Hero hero, int nTargetNationId)
	{
		m_id = Guid.NewGuid();
		m_hero = hero;
		m_nTargetNationId = nTargetNationId;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["instanceId"];
		m_nTargetNationId = Convert.ToInt32(dr["targetNationId"]);
		m_nPickCount = Convert.ToInt32(dr["count"]);
		m_nPickedLetterGrade = Convert.ToInt32(dr["grade"]);
	}

	public void StartPick()
	{
		int nDuration = Resource.instance.secretLetterQuest.interactionDuration * 1000;
		if (nDuration < 0)
		{
			nDuration = 0;
		}
		m_pickTimer = new Timer(OnPickTimerTick);
		m_pickTimer.Change(nDuration, -1);
		if (m_hero.currentPlace != null)
		{
			ServerEvent.SendHeroSecretLetterPickStarted(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.id);
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
			m_nPickedLetterGrade = PickLetterGrade();
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateSecretLetterQuest_Pick(m_id, m_nPickCount, m_nPickedLetterGrade));
			dbWork.Schedule();
			ServerEvent.SendSecretLetterPickCompleted(m_hero.account.peer, m_nPickCount, m_nPickedLetterGrade);
			if (m_hero.currentPlace != null)
			{
				ServerEvent.SendHeroSecretLetterPickCompleted(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.id, m_nPickedLetterGrade);
			}
		}
	}

	private int PickLetterGrade()
	{
		SecretLetterQuest quest = Resource.instance.secretLetterQuest;
		SecretLetterGradePool pool = quest.defaultLetterGradePool;
		if (m_nPickCount >= quest.vipBoostMinPickCount)
		{
			pool = m_hero.vipLevel.secretLetterBoostGradePool;
		}
		SecretLetterGradePoolEntry entry = null;
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
				ServerEvent.SendSecretLetterPickCanceled(m_hero.account.peer);
			}
			if (bSendEventToOthers && m_hero.currentPlace != null)
			{
				ServerEvent.SendHeroSecretLetterPickCanceled(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.id);
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

	public PDHeroSecretLetterQuest ToPDHeroSecretLetterQuest()
	{
		PDHeroSecretLetterQuest inst = new PDHeroSecretLetterQuest();
		inst.targetNationId = m_nTargetNationId;
		inst.pickCount = m_nPickCount;
		inst.pickedLetterGrade = m_nPickedLetterGrade;
		return inst;
	}
}
