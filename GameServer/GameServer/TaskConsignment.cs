using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TaskConsignment
{
	public const int kId_UndergroundMaze = 1;

	public const int kId_CreatureFarmQuest = 2;

	public const int kId_GuildHuntingQuest = 3;

	public const int kId_GuildMissionQuest = 4;

	public const int kId_FrozenWar = 5;

	private int m_nId;

	private int m_nTargetStartCount;

	private int m_nRequiredItemId;

	private int m_nRequiredItemCount;

	private int m_nCompletionRequiredTime;

	private int m_nImmediateCompletionRequiredGold;

	private int m_nImmediateCompletionRequiredGoldReduceInterval;

	private TodayTask m_todayTask;

	private GuildContributionPointReward m_guildContributionPointReward;

	private GuildBuildingPointReward m_guildBuildingPointReward;

	private GuildFundReward m_guildFundReward;

	private Dictionary<int, TaskConsignmentExpReward> m_expRewards = new Dictionary<int, TaskConsignmentExpReward>();

	private List<TaskConsignmentItemReward> m_itemRewards = new List<TaskConsignmentItemReward>();

	public int id => m_nId;

	public int targetStartCount => m_nTargetStartCount;

	public int requiredItemId => m_nRequiredItemId;

	public int requiredItemCount => m_nRequiredItemCount;

	public int completionRequiredTime => m_nCompletionRequiredTime;

	public int immediateCompletionRequiredGold => m_nImmediateCompletionRequiredGold;

	public int immediateCompletionRequiredGoldReduceInterval => m_nImmediateCompletionRequiredGoldReduceInterval;

	public float immediateCompletionIntervalGold => (float)(m_nImmediateCompletionRequiredGold * m_nImmediateCompletionRequiredGoldReduceInterval) / (float)m_nCompletionRequiredTime;

	public TodayTask todayTask => m_todayTask;

	public GuildContributionPointReward guildContributionPointReward => m_guildContributionPointReward;

	public int guildContributionPointRewardValue
	{
		get
		{
			if (m_guildContributionPointReward == null)
			{
				return 0;
			}
			return guildContributionPointReward.value;
		}
	}

	public GuildBuildingPointReward guildBuildingPointReward => m_guildBuildingPointReward;

	public int guildBuildingPointRewardValue
	{
		get
		{
			if (m_guildBuildingPointReward == null)
			{
				return 0;
			}
			return m_guildBuildingPointReward.value;
		}
	}

	public GuildFundReward guildFundReward => m_guildFundReward;

	public int guildFundRewardValue
	{
		get
		{
			if (m_guildFundReward == null)
			{
				return 0;
			}
			return m_guildFundReward.value;
		}
	}

	public List<TaskConsignmentItemReward> itemRewards => m_itemRewards;

	public bool expItemUseable => m_nId == 1;

	public bool isGuildContent
	{
		get
		{
			if (m_nId != 3)
			{
				return m_nId == 4;
			}
			return true;
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["consignmentId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "위탁ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nTargetStartCount = GetTargetStartCount(m_nId);
		m_nRequiredItemId = Convert.ToInt32(dr["requiredItemId"]);
		if (m_nRequiredItemId > 0 && Resource.instance.GetItem(m_nRequiredItemId) == null)
		{
			SFLogUtil.Warn(GetType(), "필요아이템이 존재하지 않습니다. m_nId = " + m_nId + ", m_nRequiredItemId = " + m_nRequiredItemId);
		}
		m_nRequiredItemCount = Convert.ToInt32(dr["requiredItemCount"]);
		if (m_nRequiredItemCount < 0)
		{
			SFLogUtil.Warn(GetType(), "필요아이템수량이 유효하지 않습니다. m_nId = " + m_nId + ", m_nRequiredItemCount = " + m_nRequiredItemCount);
		}
		m_nCompletionRequiredTime = Convert.ToInt32(dr["completionRequiredTime"]);
		if (m_nCompletionRequiredTime < 0)
		{
			SFLogUtil.Warn(GetType(), "완료필요시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nCompletionRequiredTime = " + m_nCompletionRequiredTime);
		}
		m_nImmediateCompletionRequiredGold = Convert.ToInt32(dr["immediateCompletionRequiredGold"]);
		if (m_nImmediateCompletionRequiredGold < 0)
		{
			SFLogUtil.Warn(GetType(), "즉시완료요구골드가 유효하지 않습니다. m_nId = " + m_nId + ", m_nImmediateCompletionRequiredGold = " + m_nImmediateCompletionRequiredGold);
		}
		m_nImmediateCompletionRequiredGoldReduceInterval = Convert.ToInt32(dr["immediateCompletionRequiredGoldReduceInterval"]);
		if (m_nImmediateCompletionRequiredGoldReduceInterval < 0)
		{
			SFLogUtil.Warn(GetType(), "즉시완료요구골드감소간격이 유효하지 않습니다. m_nId = " + m_nId + ", m_nImmediateCompletionRequiredGoldReduceInterval = " + m_nImmediateCompletionRequiredGoldReduceInterval);
		}
		int nTodayTaskId = Convert.ToInt32(dr["todayTaskId"]);
		if (nTodayTaskId > 0)
		{
			m_todayTask = Resource.instance.GetTodayTask(nTodayTaskId);
			if (m_todayTask == null)
			{
				SFLogUtil.Warn(GetType(), "오늘의할일이 존재하지 않습니다. m_nId = " + m_nId + ", nTodayTaskId = " + nTodayTaskId);
			}
		}
		long lnGuildContributionPointRewardId = Convert.ToInt64(dr["rewardGuildContributionPointRewardId"]);
		if (lnGuildContributionPointRewardId > 0)
		{
			m_guildContributionPointReward = Resource.instance.GetGuildContributionPointReward(lnGuildContributionPointRewardId);
			if (m_guildContributionPointReward == null)
			{
				SFLogUtil.Warn(GetType(), "길드공헌도보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnGuildContributionPointRewardId = " + lnGuildContributionPointRewardId);
			}
		}
		long lnGuildBuildingPointRewardId = Convert.ToInt64(dr["rewardGuildBuildingPointRewardId"]);
		if (lnGuildBuildingPointRewardId > 0)
		{
			m_guildBuildingPointReward = Resource.instance.GetGuildBuildingPointReward(lnGuildBuildingPointRewardId);
			if (m_guildBuildingPointReward == null)
			{
				SFLogUtil.Warn(GetType(), "길드건설도보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnGuildBuildingPointRewardId = " + lnGuildBuildingPointRewardId);
			}
		}
		long lnGuildFundRewardId = Convert.ToInt64(dr["rewardGuildFundRewardId"]);
		if (lnGuildFundRewardId > 0)
		{
			m_guildFundReward = Resource.instance.GetGuildFundReward(lnGuildFundRewardId);
			if (m_guildFundReward == null)
			{
				SFLogUtil.Warn(GetType(), "길드자금보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnGuildFundRewardId = " + lnGuildFundRewardId);
			}
		}
	}

	public void AddExpReward(TaskConsignmentExpReward expReward)
	{
		if (expReward == null)
		{
			throw new ArgumentNullException("expReward");
		}
		m_expRewards.Add(expReward.level, expReward);
	}

	public TaskConsignmentExpReward GetExpReward(int nLevel)
	{
		if (!m_expRewards.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddItemReward(TaskConsignmentItemReward itemReward)
	{
		if (itemReward == null)
		{
			throw new ArgumentNullException("itemReward");
		}
		m_itemRewards.Add(itemReward);
	}

	public TaskConsignmentItemReward GetItemReward(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex > m_itemRewards.Count)
		{
			return null;
		}
		return m_itemRewards[nIndex];
	}

	public static int GetTargetStartCount(int nTaskConsignmentId)
	{
		return nTaskConsignmentId switch
		{
			1 => 1, 
			2 => 1, 
			3 => 1, 
			4 => 1, 
			5 => 0, 
			_ => 0, 
		};
	}

	public static int GetTargetTaskLimitCount(int nTaskConsignmentId)
	{
		return nTaskConsignmentId switch
		{
			1 => 1, 
			2 => Resource.instance.creatureFarmQuest.limitCount, 
			3 => Resource.instance.guildHuntingQuest.limitCount, 
			4 => Resource.instance.guildMissionQuest.limitCount, 
			5 => 0, 
			_ => 0, 
		};
	}
}
