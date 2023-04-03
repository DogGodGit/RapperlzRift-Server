using System;
using System.Data;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroGuildFarmQuest
{
	public const int kStatus_Started = 0;

	public const int kStatus_Completed = 1;

	public const int kStatus_Abandoned = 2;

	public const int kStatus_Failed = 3;

	private Guid m_id = Guid.Empty;

	private Hero m_hero;

	private bool m_bIsObjectiveCompleted;

	private Timer m_interactionTimer;

	public Guid id => m_id;

	public Hero hero => m_hero;

	public bool isObjectiveCompleted => m_bIsObjectiveCompleted;

	public bool isInteracting => m_interactionTimer != null;

	public HeroGuildFarmQuest(Hero hero)
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
		m_bIsObjectiveCompleted = Convert.ToBoolean(dr["objectiveCompleted"]);
	}

	public void StartInteraction()
	{
		int nDuration = Resource.instance.guildFarmQuest.interactionDuration * 1000;
		if (nDuration < 0)
		{
			nDuration = 0;
		}
		m_interactionTimer = new Timer(OnInteractionTimerTick);
		m_interactionTimer.Change(nDuration, -1);
		if (m_hero.currentPlace != null)
		{
			ServerEvent.SendHeroGuildFarmQuestInteractionStarted(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.id);
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
			m_bIsObjectiveCompleted = true;
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroGuildFarmQuest_ObjectiveCompletion(m_id));
			dbWork.Schedule();
			ServerEvent.SendGuildFarmQuestInteractionCompleted(m_hero.account.peer);
			if (m_hero.currentPlace != null)
			{
				ServerEvent.SendHeroGuildFarmQuestInteractionCompleted(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.id);
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
				ServerEvent.SendGuildFarmQuestInteractionCanceled(m_hero.account.peer);
			}
			if (bSendEventToOthers && m_hero.currentPlace != null)
			{
				ServerEvent.SendHeroGuildFarmQuestInteractionCanceled(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.id);
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

	public PDHeroGuildFarmQuest ToPDHeroGuildFarmQuest()
	{
		PDHeroGuildFarmQuest inst = new PDHeroGuildFarmQuest();
		inst.isObjectiveCompleted = m_bIsObjectiveCompleted;
		return inst;
	}
}
