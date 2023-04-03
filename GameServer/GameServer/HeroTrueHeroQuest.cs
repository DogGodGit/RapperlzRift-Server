using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroTrueHeroQuest
{
	private Hero m_hero;

	private Guid m_id = Guid.Empty;

	private int m_nStepNo;

	private int m_nVipLevel;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	private bool m_bCompleted;

	private Timer m_waitingTimer;

	public Hero hero => m_hero;

	public Guid id => m_id;

	public int stepNo => m_nStepNo;

	public bool completed
	{
		get
		{
			return m_bCompleted;
		}
		set
		{
			m_bCompleted = value;
		}
	}

	public int vipLevel => m_nVipLevel;

	public DateTimeOffset regTime => m_regTime;

	public bool isObjectiveCompleted => m_nStepNo > Resource.instance.GetVipLevel(m_nVipLevel).trueHeroQuestStepNo;

	public Timer waitingTimer => m_waitingTimer;

	public HeroTrueHeroQuest(Hero hero)
		: this(hero, 0, 0, DateTimeOffset.MinValue)
	{
	}

	public HeroTrueHeroQuest(Hero hero, int nStepNo, int nVipLevel, DateTimeOffset regTime)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_id = Guid.NewGuid();
		m_hero = hero;
		m_nStepNo = nStepNo;
		m_nVipLevel = nVipLevel;
		m_regTime = regTime;
		m_bCompleted = false;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["questInstanceId"];
		m_nStepNo = Convert.ToInt32(dr["stepNo"]);
		m_nVipLevel = Convert.ToInt32(dr["vipLevel"]);
		m_regTime = (DateTimeOffset)dr["regTime"];
		m_bCompleted = Convert.ToBoolean(dr["completed"]);
	}

	public void StartWaiting()
	{
		TrueHeroQuestStep trueHeroQuestStep = Resource.instance.trueHeroQuest.GetStep(m_nStepNo);
		int nDuration = trueHeroQuestStep.objectiveWaitingTime * 1000;
		if (nDuration < 0)
		{
			nDuration = 0;
		}
		m_waitingTimer = new Timer(OnWaitingTimerTick);
		m_waitingTimer.Change(nDuration, -1);
	}

	private void OnWaitingTimerTick(object state)
	{
		m_hero.AddWork(new SFAction(ProcessWaitingTimerTick), bGlobalLockRequired: false);
	}

	private void ProcessWaitingTimerTick()
	{
		if (m_waitingTimer == null)
		{
			return;
		}
		DisposeWaitingTimer();
		ContinentInstance currentPlace = m_hero.currentPlace as ContinentInstance;
		TrueHeroQuestStep trueHeroQuestStep = Resource.instance.trueHeroQuest.GetStep(m_nStepNo);
		HashSet<InventorySlot> changedInventorySlots = new HashSet<InventorySlot>();
		Mail mail = null;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		ItemReward itemReward = trueHeroQuestStep.itemReward;
		if (itemReward != null)
		{
			int nRemainingItemCount = m_hero.AddItem(itemReward.item, itemReward.owned, itemReward.count, changedInventorySlots);
			if (nRemainingItemCount > 0)
			{
				mail = Mail.Create("MAIL_REWARD_N_24", "MAIL_REWARD_D_24", currentTime);
				mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingItemCount, itemReward.owned));
				m_hero.AddMail(mail, bSendEvent: true);
			}
		}
		m_nStepNo++;
		ServerEvent.SendTrueHeroQuestStepCompledted(m_hero.account.peer, m_nStepNo, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		foreach (InventorySlot slot in changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroTrueHeroQuest_Step(m_hero.id, m_nStepNo));
		if (mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(mail));
		}
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			int nRewardItemId = itemReward?.item.id ?? 0;
			bool bRewardItemOwned = itemReward?.owned ?? false;
			int nRewardItemCount = itemReward?.count ?? 0;
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroTrueHeroQuestStepRewardLog(Guid.NewGuid(), m_hero.id, m_id, m_nStepNo, currentPlace.nationId, nRewardItemId, bRewardItemOwned, nRewardItemCount, currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void DisposeWaitingTimer()
	{
		if (m_waitingTimer != null)
		{
			m_waitingTimer.Dispose();
			m_waitingTimer = null;
		}
	}

	public void CancelWaiting(bool bSendEvent)
	{
		if (m_waitingTimer != null)
		{
			DisposeWaitingTimer();
			if (bSendEvent)
			{
				ServerEvent.SendTrueHeroQuestStepWaitingCanceled(m_hero.account.peer);
			}
		}
	}

	public void Release()
	{
		DisposeWaitingTimer();
	}

	public PDHeroTrueHeroQuest ToPDHeroTrueHeroQuest()
	{
		PDHeroTrueHeroQuest inst = new PDHeroTrueHeroQuest();
		inst.instanceId = (Guid)m_id;
		inst.stepNo = m_nStepNo;
		inst.vipLevel = m_nVipLevel;
		inst.acceptedDate = (DateTime)m_regTime.Date;
		inst.completed = m_bCompleted;
		return inst;
	}
}
