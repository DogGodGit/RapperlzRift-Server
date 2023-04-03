using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class StoryDungeonSweepCommandHandler : InGameCommandHandler<StoryDungeonSweepCommandBody, StoryDungeonSweepResponseBody>
{
	public const short kResult_Dead = 101;

	public const short kResult_NotEnoughSweepItem = 103;

	public const short kResult_NotEnoughStamina = 104;

	public const short kResult_EnterCountOverflowed = 105;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private StoryDungeonDifficulty m_difficulty;

	private bool m_bIsFreeSweep;

	private DateValuePair<int> m_freeSweepDailyCount;

	private List<PDItemBooty> m_pdItemBooties = new List<PDItemBooty>();

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nDungeonNo = m_body.dungeonNo;
		int nDifficulty = m_body.difficulty;
		StoryDungeon storyDungeon = Resource.instance.GetStoryDungeon(nDungeonNo);
		if (storyDungeon == null)
		{
			throw new CommandHandleException(1, "해당 던전번호는 존재하지 않습니다. nDungeonNo = " + nDungeonNo);
		}
		m_difficulty = storyDungeon.GetDifficulty(nDifficulty);
		if (m_difficulty == null)
		{
			throw new CommandHandleException(1, "해당 난이도는 존재하지 않습니다. nDifficulty = " + nDifficulty);
		}
		int nHeroClearMaxDifficulty = m_myHero.GetStoryDungeonClearMaxDifficulty(nDungeonNo);
		if (nHeroClearMaxDifficulty < storyDungeon.topDifficulty)
		{
			throw new CommandHandleException(1, "영웅이 소탕에 필요한 조건을 만족하지 못했습니다. nHeroClearMaxDifficulty = " + nHeroClearMaxDifficulty + ", storyDungeonTopDifficulty = " + storyDungeon.topDifficulty);
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은상태 입니다.");
		}
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		m_myHero.RefreshFreeSweepDailyCount(currentDate);
		m_freeSweepDailyCount = m_myHero.freeSweepDailyCount;
		if (m_freeSweepDailyCount.value < Resource.instance.dungeonFreeSweepDailyCount)
		{
			m_bIsFreeSweep = true;
		}
		int nDungeonSweepItemId = Resource.instance.dungeonSweepItemId;
		if (!m_bIsFreeSweep && m_myHero.GetItemCount(nDungeonSweepItemId) == 0)
		{
			throw new CommandHandleException(103, "소탕령이 부족합니다.");
		}
		int nRequiredStamina = storyDungeon.requiredStamina;
		if (m_myHero.stamina < nRequiredStamina)
		{
			throw new CommandHandleException(104, "스태미너가 부족합니다.");
		}
		if (m_myHero.GetStoryDungeonAvailableEnterCount(nDungeonNo, currentDate) <= 0)
		{
			throw new CommandHandleException(105, "입장횟수가 초과되었습니다.");
		}
		int nUsedOwnCount = 0;
		int nUsedUnOwnCount = 0;
		if (!m_bIsFreeSweep)
		{
			m_myHero.UseItem(nDungeonSweepItemId, bFisetUseOwn: true, 1, m_changedInventorySlots, out nUsedOwnCount, out nUsedUnOwnCount);
		}
		else
		{
			m_freeSweepDailyCount.value++;
		}
		m_myHero.UseStamina(nRequiredStamina, m_currentTime);
		StoryDungeonPlay play = m_myHero.GetOrCreateStoryDungeonPlay(storyDungeon.no, currentDate);
		play.enterCount++;
		SweepReward();
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB(nUsedOwnCount, nUsedUnOwnCount, play.enterCount);
		StoryDungeonSweepResponseBody resBody = new StoryDungeonSweepResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.stamina = m_myHero.stamina;
		resBody.playCount = play.enterCount;
		resBody.freeSweepDailyCount = m_freeSweepDailyCount.value;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.booties = m_pdItemBooties.ToArray();
		SendResponseOK(resBody);
		m_myHero.ProcessTodayTask(10, m_currentTime.Date);
		m_myHero.IncreaseOpen7DayEventProgressCount(2);
	}

	private void SweepReward()
	{
		foreach (StoryDungeonSweepReward reward in m_difficulty.sweepRewards)
		{
			Item rewardItem = reward.itemReward.item;
			int nRewardCount = reward.itemReward.count;
			bool bRewardOwned = reward.itemReward.owned;
			int nRewardItemRemainCount = m_myHero.AddItem(rewardItem, bRewardOwned, nRewardCount, m_changedInventorySlots);
			PDItemBooty pdItemBooty = new PDItemBooty();
			pdItemBooty.id = rewardItem.id;
			pdItemBooty.count = nRewardCount;
			pdItemBooty.owned = bRewardOwned;
			if (nRewardCount > 0)
			{
				m_pdItemBooties.Add(pdItemBooty);
			}
			if (nRewardItemRemainCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_REWARD_N_5", "MAIL_REWARD_D_5", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(rewardItem, nRewardItemRemainCount, bRewardOwned));
			}
		}
	}

	private void SaveToDB(int nUsedOwnSweepItemCount, int nUsedUnOwnSweepItemCount, int nEnterCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateStoryDungeonPlay(m_myHero.id, m_currentTime.Date, m_difficulty.storyDungeon.no, nEnterCount));
		if (m_bIsFreeSweep)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_FreeSweepCount(m_myHero.id, m_freeSweepDailyCount.date, m_freeSweepDailyCount.value));
		}
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_ApplyChangedInventorySlots(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
		SaveToDB_Log(nUsedOwnSweepItemCount, nUsedUnOwnSweepItemCount);
	}

	private void SaveToDB_Log(int nItemOwnCount, int nItemUnOwnCount)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, Resource.instance.dungeonSweepItemId, nItemOwnCount, nItemUnOwnCount, m_currentTime));
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddStoryDungeonPlayLog(logId, Guid.Empty, m_myHero.id, m_difficulty.storyDungeon.no, m_difficulty.difficulty, 3, m_currentTime));
			int nNo = 1;
			foreach (StoryDungeonSweepReward reward in m_difficulty.sweepRewards)
			{
				ItemReward itemReward = reward.itemReward;
				logWork.AddSqlCommand(GameLogDac.CSC_AddStoryDungeonPlayRewardLog(logId, nNo++, itemReward.item.id, itemReward.count, itemReward.owned));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
