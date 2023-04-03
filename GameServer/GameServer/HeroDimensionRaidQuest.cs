using System;
using System.Data;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroDimensionRaidQuest
{
	public const int kStatus_Started = 0;

	public const int kStatus_Completed = 1;

	public const int kStatus_Abandoned = 2;

	private Guid m_id = Guid.Empty;

	private Hero m_hero;

	private int m_nStep;

	private Timer m_interactionTimer;

	public Guid id => m_id;

	public Hero hero => m_hero;

	public int step => m_nStep;

	public bool isObjectiveCompleted => m_nStep <= 0;

	public bool isInteracting => m_interactionTimer != null;

	public HeroDimensionRaidQuest(Hero hero, int nStep)
	{
		m_id = Guid.NewGuid();
		m_hero = hero;
		m_nStep = nStep;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["instanceId"];
		m_nStep = Convert.ToInt32(dr["step"]);
	}

	public void StartInteraction()
	{
		int nDuration = Resource.instance.dimensionRaidQuest.targetInteractionDuration * 1000;
		if (nDuration < 0)
		{
			nDuration = 0;
		}
		m_interactionTimer = new Timer(OnInteractionTimerTick);
		m_interactionTimer.Change(nDuration, -1);
		if (m_hero.currentPlace != null)
		{
			ServerEvent.SendHeroDimensionRaidInteractionStarted(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.id);
		}
	}

	private void OnInteractionTimerTick(object state)
	{
		m_hero.AddWork(new SFAction(ProcessInteractionTimerTick), bGlobalLockRequired: false);
	}

	private void ProcessInteractionTimerTick()
	{
		if (isInteracting)
		{
			DisposeInteractionTimer();
			m_nStep++;
			if (m_nStep > Resource.instance.dimensionRaidQuest.lastStep.step)
			{
				m_nStep = 0;
			}
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateDimensionRaidQuest_Step(m_id, m_nStep));
			dbWork.Schedule();
			ServerEvent.SendDimensionRaidInteractionCompleted(m_hero.account.peer, m_nStep);
			if (m_hero.currentPlace != null)
			{
				ServerEvent.SendHeroDimensionRaidInteractionCompleted(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.id);
			}
		}
	}

	public void CancelInteraction(bool bSendEventToOwner, bool bSendEventToOthers)
	{
		if (isInteracting)
		{
			DisposeInteractionTimer();
			if (bSendEventToOwner)
			{
				ServerEvent.SendDimensionRaidInteractionCanceled(m_hero.account.peer);
			}
			if (bSendEventToOthers && m_hero.currentPlace != null)
			{
				ServerEvent.SendHeroDimensionRaidInteractionCanceled(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.id);
			}
		}
	}

	private void DisposeInteractionTimer()
	{
		if (m_interactionTimer != null)
		{
			m_interactionTimer.Dispose();
			m_interactionTimer = null;
		}
	}

	public void Release()
	{
		DisposeInteractionTimer();
	}

	public PDHeroDimensionRaidQuest ToPDHeroDimensionRaidQuest()
	{
		PDHeroDimensionRaidQuest inst = new PDHeroDimensionRaidQuest();
		inst.step = m_nStep;
		return inst;
	}
}
