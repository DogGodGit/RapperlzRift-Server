using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroProspectQuest
{
	private Guid m_instanceId = Guid.Empty;

	private Guid m_ownerId = Guid.Empty;

	private string m_sOwnerName;

	private int m_nOwnerJobId;

	private int m_nOwnerLevel;

	private Guid m_targetId = Guid.Empty;

	private string m_sTargetName;

	private int m_nTargetJobId;

	private int m_nTargetLevel;

	private BlessingTargetLevel m_blessingTargetLevel;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	private HeroProspectQuestStatus m_status;

	private DateTimeOffset? m_statusUpdateTime = null;

	private bool m_bOwnerRewarded;

	private bool m_bTargetRewarded;

	private Timer m_limitTimeTimer;

	private bool m_bReleased;

	public Guid instanceId => m_instanceId;

	public Guid ownerId => m_ownerId;

	public string ownerName => m_sOwnerName;

	public int ownerJobId => m_nOwnerJobId;

	public int ownerLevel => m_nOwnerLevel;

	public Guid targetId => m_targetId;

	public string targetName => m_sTargetName;

	public int targetJobId => m_nTargetJobId;

	public int targetLevel => m_nTargetLevel;

	public BlessingTargetLevel blessingTargetLevel => m_blessingTargetLevel;

	public DateTimeOffset regTime => m_regTime;

	public HeroProspectQuestStatus status => m_status;

	public DateTimeOffset? statusUpdateTime => m_statusUpdateTime;

	public bool ownerRewarded
	{
		get
		{
			return m_bOwnerRewarded;
		}
		set
		{
			m_bOwnerRewarded = value;
		}
	}

	public bool targetRewarded
	{
		get
		{
			return m_bTargetRewarded;
		}
		set
		{
			m_bTargetRewarded = value;
		}
	}

	public bool isAllRewarded
	{
		get
		{
			if (m_bOwnerRewarded)
			{
				return m_bTargetRewarded;
			}
			return false;
		}
	}

	public bool isProgressing => m_status == HeroProspectQuestStatus.Progressing;

	public bool isCompleted => m_status == HeroProspectQuestStatus.Completed;

	public bool isFailed => m_status == HeroProspectQuestStatus.Failed;

	public void Init(DataRow dr, DateTimeOffset currentTime)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		Init((Guid)dr["questInstanceId"], (Guid)dr["ownerId"], Convert.ToString(dr["ownerName"]), Convert.ToInt32(dr["ownerJobId"]), Convert.ToInt32(dr["ownerLevel"]), (Guid)dr["targetId"], Convert.ToString(dr["targetName"]), Convert.ToInt32(dr["targetJobId"]), Convert.ToInt32(dr["targetLevel"]), Resource.instance.GetBlessingTargetLevel(Convert.ToInt32(dr["targetLevelId"])), (DateTimeOffset)dr["regTime"], (HeroProspectQuestStatus)Convert.ToInt32(dr["status"]), SFDBUtil.ToNullableDateTimeOffset(dr["statusUpdateTime"]), Convert.ToBoolean(dr["ownerRewarded"]), Convert.ToBoolean(dr["targetRewarded"]), currentTime, bSaveToDBWhenStatusChanged: true);
	}

	public void Init(Hero owner, Hero target, BlessingTargetLevel blessingTargetLevel, DateTimeOffset currentTime)
	{
		Init(Guid.NewGuid(), owner.id, owner.name, owner.jobId, owner.level, target.id, target.name, target.jobId, target.level, blessingTargetLevel, currentTime, HeroProspectQuestStatus.Progressing, null, bOwnerRewarded: false, bTargetRewarded: false, currentTime, bSaveToDBWhenStatusChanged: false);
	}

	private void Init(Guid instanceId, Guid ownerId, string sOwnerName, int nOwnerJobId, int nOwnerLevel, Guid targetId, string sTargetName, int nTargetJobId, int nTargetLevel, BlessingTargetLevel blessingTargetLevel, DateTimeOffset regTime, HeroProspectQuestStatus status, DateTimeOffset? statusUpdateTime, bool bOwnerRewarded, bool bTargetRewarded, DateTimeOffset currentTime, bool bSaveToDBWhenStatusChanged)
	{
		if (blessingTargetLevel == null)
		{
			throw new ArgumentNullException("blessingTargetLevel");
		}
		m_instanceId = instanceId;
		m_ownerId = ownerId;
		m_sOwnerName = sOwnerName;
		m_nOwnerJobId = nOwnerJobId;
		m_nOwnerLevel = nOwnerLevel;
		m_targetId = targetId;
		m_sTargetName = sTargetName;
		m_nTargetJobId = nTargetJobId;
		m_nTargetLevel = nTargetLevel;
		m_blessingTargetLevel = blessingTargetLevel;
		m_regTime = regTime;
		m_status = status;
		m_statusUpdateTime = statusUpdateTime;
		m_bOwnerRewarded = bOwnerRewarded;
		m_bTargetRewarded = bTargetRewarded;
		if (m_nTargetLevel >= m_blessingTargetLevel.prospectQuestObjectiveLevel)
		{
			Complete(currentTime, bSaveToDBWhenStatusChanged, bSendEvent: false);
			return;
		}
		int nDuration = (int)Math.Floor(GetRemainingTime(currentTime) * 1000f);
		if (nDuration > 0)
		{
			m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
			m_limitTimeTimer.Change(nDuration, -1);
		}
		else
		{
			Fail(currentTime, bSaveToDBWhenStatusChanged, bSendEvent: false);
		}
	}

	private void DisposeLimitTimeTimer()
	{
		if (m_limitTimeTimer != null)
		{
			m_limitTimeTimer.Dispose();
			m_limitTimeTimer = null;
		}
	}

	private void OnLimitTimeTimerTick(object state)
	{
		Global.instance.AddWork(new SFAction(ProcessLimitTimeTimerTick));
	}

	private void ProcessLimitTimeTimerTick()
	{
		if (!m_bReleased)
		{
			Fail(DateTimeUtil.currentTime, bSaveToDB: true, bSendEvent: true);
		}
	}

	public float GetRemainingTime(DateTimeOffset time)
	{
		return (float)Math.Max((double)m_blessingTargetLevel.prospectQuestObjectiveLimitTime - (time - m_regTime).TotalSeconds, 0.0);
	}

	private void Complete(DateTimeOffset time, bool bSaveToDB, bool bSendEvent)
	{
		if (!isProgressing)
		{
			return;
		}
		m_status = HeroProspectQuestStatus.Completed;
		m_statusUpdateTime = time;
		DisposeLimitTimeTimer();
		if (bSaveToDB)
		{
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_ProspectQuest);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroProspectQuest_Status(m_instanceId, (int)m_status, m_statusUpdateTime));
			dbWork.Schedule();
		}
		if (!bSendEvent)
		{
			return;
		}
		Hero owner = Cache.instance.GetLoggedInHero(m_ownerId);
		if (owner != null)
		{
			lock (owner.syncObject)
			{
				owner.OnOwnerProspectQuestCompleted(m_instanceId);
			}
		}
		Hero target = Cache.instance.GetLoggedInHero(m_targetId);
		if (target != null)
		{
			lock (target.syncObject)
			{
				target.OnTargetProspectQuestCompleted(m_instanceId);
			}
		}
	}

	private void Fail(DateTimeOffset time, bool bSaveToDB, bool bSendEvent)
	{
		if (!isProgressing)
		{
			return;
		}
		m_status = HeroProspectQuestStatus.Failed;
		m_statusUpdateTime = time;
		DisposeLimitTimeTimer();
		if (bSaveToDB)
		{
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_ProspectQuest);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroProspectQuest_Status(m_instanceId, (int)m_status, m_statusUpdateTime));
			dbWork.Schedule();
		}
		if (bSendEvent)
		{
			Hero owner = Cache.instance.GetLoggedInHero(m_ownerId);
			if (owner != null)
			{
				lock (owner.syncObject)
				{
					owner.OnOwnerProspectQuestFailed(m_instanceId);
				}
			}
			Hero target = Cache.instance.GetLoggedInHero(m_targetId);
			if (target != null)
			{
				lock (target.syncObject)
				{
					target.OnTargetProspectQuestFailed(m_instanceId);
				}
			}
		}
		Cache.instance.RemoveProspectQuest(m_instanceId);
	}

	public void OnTargetLevelUp(int nLevel)
	{
		Global.instance.AddWork(new SFAction<int>(UpdateTargetLevel, nLevel));
	}

	private void UpdateTargetLevel(int nTargetLevel)
	{
		if (!m_bReleased)
		{
			m_nTargetLevel = nTargetLevel;
			Hero owner = Cache.instance.GetLoggedInHero(m_ownerId);
			if (owner != null)
			{
				ServerEvent.SendOwnerProspectQuestTargetLevelUpdated(owner.account.peer, m_instanceId, m_nTargetLevel);
			}
			if (m_nTargetLevel >= m_blessingTargetLevel.prospectQuestObjectiveLevel)
			{
				Complete(DateTimeUtil.currentTime, bSaveToDB: true, bSendEvent: true);
			}
		}
	}

	public void Release()
	{
		if (!m_bReleased)
		{
			DisposeLimitTimeTimer();
			m_bReleased = true;
		}
	}

	public PDHeroProspectQuest ToPDHeroProspectQuest(DateTimeOffset time)
	{
		PDHeroProspectQuest inst = new PDHeroProspectQuest();
		inst.instanceId = (Guid)m_instanceId;
		inst.ownerId = (Guid)m_ownerId;
		inst.ownerName = m_sOwnerName;
		inst.ownerJobId = m_nOwnerJobId;
		inst.ownerLevel = m_nOwnerLevel;
		inst.targetId = (Guid)m_targetId;
		inst.targetName = m_sTargetName;
		inst.targetJobId = m_nTargetJobId;
		inst.targetLevel = m_nTargetLevel;
		inst.blessingTargetLevelId = m_blessingTargetLevel.id;
		inst.remainingTime = GetRemainingTime(time);
		inst.isCompleted = isCompleted;
		return inst;
	}

	public static List<PDHeroProspectQuest> ToPDHeroProspectQuests(IEnumerable<HeroProspectQuest> quests, DateTimeOffset time)
	{
		List<PDHeroProspectQuest> results = new List<PDHeroProspectQuest>();
		foreach (HeroProspectQuest quest in quests)
		{
			results.Add(quest.ToPDHeroProspectQuest(time));
		}
		return results;
	}
}
