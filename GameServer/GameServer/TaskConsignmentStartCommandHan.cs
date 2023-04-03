using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class TaskConsignmentStartCommandHandler : InGameCommandHandler<TaskConsignmentStartCommandBody, TaskConsignmentStartResponseBody>
{
	public const short kReuslt_NotGuildMemeber = 101;

	public const short kResult_NotEnoughVipLevel = 102;

	public const short kResult_StartedConsignment = 103;

	public const short kResult_AlreadyStartedTask = 104;

	public const short kResult_OverflowedStartCount = 105;

	public const short kResult_NotEnoughExpItem = 106;

	public const short kResult_NotEnoughMeterialItem = 107;

	private HeroTaskConsignment m_heroTaskConsignment;

	private int m_nUsedItemId;

	private int m_nUsedOwnCount;

	private int m_nUsedUnOwnCount;

	private int m_nUsedExpItemId;

	private int m_nUsedExpItemOwnCount;

	private int m_nUsedExpItemUnOwnCount;

	private HashSet<InventorySlot> m_changedInventorys = new HashSet<InventorySlot>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nConsignmentId = m_body.consignmentId;
		m_nUsedExpItemId = m_body.useExpItemId;
		if (nConsignmentId <= 0)
		{
			throw new CommandHandleException(1, "유효하지 않는 위탁ID 입니다. nConsignmentId = " + nConsignmentId);
		}
		if (m_nUsedExpItemId < 0)
		{
			throw new CommandHandleException(1, "유효하지 않는 경험치아이템ID 입니다. m_nUsedExpItemId = " + m_nUsedExpItemId);
		}
		TaskConsignment taskConsignment = Resource.instance.GetTaskConsignment(nConsignmentId);
		GuildMember guildMember = m_myHero.guildMember;
		Guild guild = null;
		if (taskConsignment == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 위탁입니다. nConsignmentId = " + nConsignmentId);
		}
		if (taskConsignment.isGuildContent && guildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어 있지 않습니다.");
		}
		if (!taskConsignment.expItemUseable && m_nUsedExpItemId > 0)
		{
			throw new CommandHandleException(1, "경험치아이템을 사용할 수 없는 위탁입니다.");
		}
		if (guildMember != null)
		{
			guild = guildMember.guild;
		}
		if (m_myHero.vipLevel.level < Resource.instance.taskConsignmentRequiredVipLevel)
		{
			throw new CommandHandleException(102, "VIP레벨이 부족합니다.");
		}
		if (m_myHero.IsConsignedTask(nConsignmentId))
		{
			throw new CommandHandleException(103, "시작중인 위탁입니다.");
		}
		if (m_myHero.IsTaskConsignmentTargetTaskStarted(nConsignmentId, currentDate))
		{
			throw new CommandHandleException(104, "이미 시작된 할일입니다.");
		}
		if (m_myHero.GetRemainingTaskConsignmentStartCount(nConsignmentId, currentDate) <= 0)
		{
			throw new CommandHandleException(105, "위탁횟수를 초과했습니다.");
		}
		if (m_nUsedExpItemId > 0)
		{
			Item expItem = Resource.instance.GetItem(m_nUsedExpItemId);
			if (expItem == null)
			{
				throw new CommandHandleException(1, "존재하지 않는 아이템입니다. m_nUsedExpItemId = " + m_nUsedExpItemId);
			}
			if (expItem.type.id != 11)
			{
				throw new CommandHandleException(1, "해당 아이템은 경험치스크롤 타입이 아닙니다. m_nUsedExpItemId = " + m_nUsedExpItemId);
			}
			if (m_myHero.GetItemCount(expItem.id) < 1)
			{
				throw new CommandHandleException(106, "사용할 경험치아이템이 부족합니다.");
			}
		}
		m_nUsedItemId = taskConsignment.requiredItemId;
		int nRequiredItemCount = taskConsignment.requiredItemCount;
		if (m_myHero.GetItemCount(m_nUsedItemId) < nRequiredItemCount)
		{
			throw new CommandHandleException(107, "재료아이템이 부족합니다.");
		}
		m_myHero.RefreshAchievementDailyPoint(currentDate);
		TodayTask todayTask = taskConsignment.todayTask;
		int nTotalAchievementPoint = 0;
		if (todayTask != null)
		{
			nTotalAchievementPoint = todayTask.achievementPoint * TaskConsignment.GetTargetTaskLimitCount(nConsignmentId);
			if (nTotalAchievementPoint > 0)
			{
				m_myHero.achievementDailyPoint.value += nTotalAchievementPoint;
			}
		}
		if (m_nUsedExpItemId > 0)
		{
			m_myHero.UseItem(m_nUsedExpItemId, bFisetUseOwn: true, 1, m_changedInventorys, out m_nUsedExpItemOwnCount, out m_nUsedExpItemUnOwnCount);
		}
		m_myHero.UseItem(m_nUsedItemId, bFisetUseOwn: true, nRequiredItemCount, m_changedInventorys, out m_nUsedOwnCount, out m_nUsedUnOwnCount);
		m_heroTaskConsignment = new HeroTaskConsignment(m_myHero, taskConsignment, m_nUsedExpItemId, m_currentTime);
		m_myHero.AddTaskConsignment(m_heroTaskConsignment);
		HeroTaskConsignmentStartCount startCount = m_myHero.GetTaskConsignmentStartCount(nConsignmentId);
		if (startCount == null)
		{
			startCount = new HeroTaskConsignmentStartCount(nConsignmentId, 0);
			m_myHero.AddTaskConsignmentStartCount(startCount);
		}
		startCount.count++;
		SaveToDB();
		SaveToLogDB(nTotalAchievementPoint);
		TaskConsignmentStartResponseBody resBody = new TaskConsignmentStartResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.achievementDailyPoint = m_myHero.achievementDailyPoint.value;
		resBody.startCount = startCount.ToPDHeroTaskConsignmentStartCount();
		resBody.taskConsignment = m_heroTaskConsignment.ToPDHeroTaskConsignment(m_currentTime);
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorys).ToArray();
		SendResponseOK(resBody);
		if (guild != null)
		{
			switch (taskConsignment.id)
			{
			case 3:
				guild.CompleteGuildDailyObjective(currentDate, 2, guildMember);
				break;
			case 4:
				guild.CompleteGuildDailyObjective(currentDate, 1, guildMember);
				break;
			}
		}
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_AchievementPoint(m_myHero.id, m_myHero.achievementDailyPoint.date, m_myHero.achievementDailyPoint.value, m_myHero.receivedAchievementRewardNo));
		foreach (InventorySlot slot in m_changedInventorys)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroTaskConsignment(m_heroTaskConsignment.instanceId, m_heroTaskConsignment.hero.id, m_heroTaskConsignment.consignment.id, m_heroTaskConsignment.usedExpItemId, m_heroTaskConsignment.regTime));
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nTotalAchievementPoint)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroTaskConsignmentLog(Guid.NewGuid(), m_myHero.id, m_heroTaskConsignment.consignment.id, m_nUsedItemId, m_nUsedOwnCount, m_nUsedUnOwnCount, m_nUsedExpItemId, m_nUsedExpItemOwnCount, m_nUsedExpItemUnOwnCount, nTotalAchievementPoint, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
