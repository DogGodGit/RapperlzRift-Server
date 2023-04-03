using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroWeeklyQuest
{
	private Hero m_hero;

	private DateTime m_weekStartDate = DateTime.MinValue.Date;

	private int m_nRoundNo;

	private Guid m_roundId = Guid.Empty;

	private WeeklyQuestMission m_roundMission;

	private int m_nRoundProgressCount;

	private HashSet<long> m_huntedMonsters = new HashSet<long>();

	private HeroWeeklyQuestRoundStatus m_roundStatus;

	public Hero hero => m_hero;

	public DateTime weekStartDate => m_weekStartDate;

	public int roundNo
	{
		get
		{
			return m_nRoundNo;
		}
		set
		{
			m_nRoundNo = value;
		}
	}

	public bool isCompleted => m_nRoundNo > Resource.instance.weeklyQuest.roundCount;

	public Guid roundId => m_roundId;

	public int roundMissionId
	{
		get
		{
			if (m_roundMission == null)
			{
				return 0;
			}
			return m_roundMission.id;
		}
	}

	public bool isRoundValid
	{
		get
		{
			if (m_roundId != Guid.Empty)
			{
				return m_roundMission != null;
			}
			return false;
		}
	}

	public WeeklyQuestMission roundMission => m_roundMission;

	public int roundProgressCount
	{
		get
		{
			return m_nRoundProgressCount;
		}
		set
		{
			m_nRoundProgressCount = value;
		}
	}

	public bool isRoundMissionCompleted => m_nRoundProgressCount >= m_roundMission.targetCount;

	public HeroWeeklyQuestRoundStatus roundStatus
	{
		get
		{
			return m_roundStatus;
		}
		set
		{
			m_roundStatus = value;
		}
	}

	public bool isRoundCreated => m_roundStatus == HeroWeeklyQuestRoundStatus.Creation;

	public bool isRoundAccepted => m_roundStatus == HeroWeeklyQuestRoundStatus.Progress;

	public HeroWeeklyQuest(Hero hero)
		: this(hero, DateTime.MinValue.Date)
	{
	}

	public HeroWeeklyQuest(Hero hero, DateTime weekStartDate)
	{
		m_hero = hero;
		m_weekStartDate = weekStartDate;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_weekStartDate = Convert.ToDateTime(dr["weekStartDate"]);
		m_nRoundNo = Convert.ToInt32(dr["roundNo"]);
		m_roundId = (Guid)dr["roundId"];
		int nRoundMissionId = Convert.ToInt32(dr["roundMissionId"]);
		m_roundMission = Resource.instance.weeklyQuest.GetMission(nRoundMissionId);
		m_nRoundProgressCount = Convert.ToInt32(dr["roundProgressCount"]);
		int nRoundStatus = Convert.ToInt32(dr["roundStatus"]);
		if (!Enum.IsDefined(typeof(HeroWeeklyQuestRoundStatus), nRoundStatus))
		{
			throw new Exception(string.Concat("상태가 유효하지 않습니다. heroId = ", m_hero.id, ", nRoundStatus = ", nRoundStatus));
		}
		m_roundStatus = (HeroWeeklyQuestRoundStatus)nRoundStatus;
	}

	public void SetNextRound()
	{
		SetRound(m_nRoundNo + 1);
	}

	public void SetRound(int nRoundNo)
	{
		m_nRoundNo = nRoundNo;
		m_roundId = Guid.NewGuid();
		m_roundMission = Resource.instance.weeklyQuest.GetPool(m_hero.level).SelectMission();
		m_nRoundProgressCount = 0;
		m_roundStatus = HeroWeeklyQuestRoundStatus.Creation;
	}

	public void RefreshCurrentRound()
	{
		m_roundId = Guid.NewGuid();
		m_roundMission = Resource.instance.weeklyQuest.GetPool(m_hero.level).SelectMission();
		m_nRoundProgressCount = 0;
		m_roundStatus = HeroWeeklyQuestRoundStatus.Creation;
	}

	public void Complete()
	{
		m_nRoundNo++;
		m_roundId = Guid.Empty;
		m_roundMission = null;
		m_nRoundProgressCount = 0;
		m_roundStatus = HeroWeeklyQuestRoundStatus.Creation;
	}

	public void IncreaseProgressCountByHuntMission(long lnMonsterInstanceId, DateTimeOffset currentTime)
	{
		if (m_huntedMonsters.Add(lnMonsterInstanceId))
		{
			IncreaseProgressCount(currentTime);
		}
	}

	public void IncreaseProgressCount(DateTimeOffset currentTime)
	{
		if (m_roundMission != null)
		{
			m_nRoundProgressCount++;
			ServerEvent.SendWeeklyQuestRoundProgressCountUpdated(m_hero.account.peer, m_roundId, m_nRoundProgressCount);
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroWeeklyQuest_ProgressCount(m_hero.id, m_nRoundProgressCount));
			dbWork.Schedule();
			if (isRoundMissionCompleted)
			{
				CompleteRound(currentTime, bSendEvent: true);
			}
		}
	}

	private void CompleteRound(DateTimeOffset currentTime, bool bSendEvent)
	{
		HashSet<InventorySlot> changedInventorySlots = new HashSet<InventorySlot>();
		long lnExpReward = 0L;
		long lnGoldReawrd = 0L;
		CompleteRound(currentTime, bSendEvent, changedInventorySlots, out lnExpReward, out lnGoldReawrd);
	}

	public void CompleteRound(DateTimeOffset currentTime, bool bSendEvent, ICollection<InventorySlot> changedInventorySlots, out long lnExpReward, out long lnGoldReward)
	{
		lnExpReward = 0L;
		lnGoldReward = 0L;
		Resource res = Resource.instance;
		WeeklyQuestRound weeklyQuestRound = res.weeklyQuest.GetRound(m_nRoundNo);
		WeeklyQuestRoundReward weeklyQuestRoundReward = weeklyQuestRound.GetReward(m_hero.level);
		if (weeklyQuestRoundReward != null)
		{
			lnExpReward = weeklyQuestRoundReward.expRewardValue;
			lnGoldReward = weeklyQuestRoundReward.goldRewardValue;
		}
		lnExpReward = (long)Math.Floor((float)lnExpReward * Cache.instance.GetWorldLevelExpFactor(m_hero.level));
		m_hero.AddExp(lnExpReward, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		m_hero.AddGold(lnGoldReward);
		ResultItemCollection round10RewardItemCollection = null;
		Mail mail = null;
		if (m_nRoundNo % 10 == 0)
		{
			round10RewardItemCollection = new ResultItemCollection();
			foreach (WeeklyQuestTenRoundReward reward in Resource.instance.weeklyQuest.tenRoundRewards)
			{
				ItemReward itemReward = reward.itemReward;
				int nRemainingItemCount = m_hero.AddItem(itemReward.item, itemReward.owned, itemReward.count, changedInventorySlots);
				if (nRemainingItemCount > 0)
				{
					if (mail == null)
					{
						mail = Mail.Create("MAIL_REWARD_N_18", "MAIL_REWARD_D_18", currentTime);
					}
					mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingItemCount, itemReward.owned));
				}
				round10RewardItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
			}
		}
		if (mail != null)
		{
			m_hero.AddMail(mail, bSendEvent: true);
		}
		Guid oldWeeklyQuestRoundId = m_roundId;
		if (m_nRoundNo < res.weeklyQuest.roundCount)
		{
			SetNextRound();
		}
		else
		{
			Complete();
		}
		if (bSendEvent)
		{
			ServerEvent.SendWeeklyQuestRoundCompletedEventBody(m_hero.account.peer, m_hero.gold, m_hero.maxGold, lnExpReward, m_hero.level, m_hero.exp, m_hero.realMaxHP, m_hero.hp, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray(), m_nRoundNo, m_roundId, roundMissionId);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_hero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_hero));
		foreach (InventorySlot slot in changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateHeroWeeklyQuest(this));
		if (mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(mail));
		}
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			if (m_roundMission != null)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeeklyQuestRoundCreationLog(m_roundId, m_hero.id, m_weekStartDate, m_nRoundNo, m_roundMission.id, currentTime));
			}
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeeklyQuestRoundRewardLog(Guid.NewGuid(), m_hero.id, oldWeeklyQuestRoundId, lnGoldReward, lnExpReward, currentTime));
			if (round10RewardItemCollection != null)
			{
				Guid round10RewardLogId = Guid.NewGuid();
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeeklyQuestTenRoundRewardLog(round10RewardLogId, m_hero.id, m_weekStartDate, m_nRoundNo - 1, currentTime));
				foreach (ResultItem resultItem in round10RewardItemCollection.resultItems)
				{
					logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeeklyQuestTenRoundRewardDetailLog(Guid.NewGuid(), round10RewardLogId, resultItem.item.id, resultItem.owned, resultItem.count));
				}
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	public PDHeroWeeklyQuest ToPDHeroWeeklyQuest()
	{
		PDHeroWeeklyQuest inst = new PDHeroWeeklyQuest();
		inst.weekStartDate = (DateTime)m_weekStartDate;
		inst.roundNo = m_nRoundNo;
		inst.roundId = (Guid)m_roundId;
		inst.roundMissionId = roundMissionId;
		inst.isRoundAccepted = isRoundAccepted;
		inst.roundProgressCount = m_nRoundProgressCount;
		return inst;
	}
}
