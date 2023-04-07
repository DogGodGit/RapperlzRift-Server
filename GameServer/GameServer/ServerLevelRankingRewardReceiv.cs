using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class ServerLevelRankingRewardReceiveCommandHandler : InGameCommandHandler<ServerLevelRankingRewardReceiveCommandBody, ServerLevelRankingRewardReceiveResponseBody>
{
	public const short kResult_AlreadyReceived = 101;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		_ = m_currentTime.Date;
		DailyServerLevelRankingManager manager = DailyServerLevelRankingManager.instance;
		int nDailyServerLevelRankingNo = manager.rankingNo;
		if (m_myHero.rewardedDailyServerLevelRankingNo >= nDailyServerLevelRankingNo)
		{
			throw new CommandHandleException(101, "이미 보상을 받았습니다.");
		}
		Ranking dailyServerLevelRanking = manager.GetRankingOfHero(m_myHero.id);
		if (dailyServerLevelRanking == null)
		{
			throw new CommandHandleException(1, "일일서버레벨랭킹에 진입하지 못했습니다.");
		}
		LevelRankingReward reward = Resource.instance.GetLevelRankingReward(dailyServerLevelRanking.ranking);
		if (reward == null)
		{
			throw new CommandHandleException(1, "보상이 존재하지 않습니다.");
		}
		ItemReward itemReward = reward.itemReward;
		int nRemainingCount = m_myHero.AddItem(itemReward.item, itemReward.owned, itemReward.count, m_changedInventorySlots);
		if (nRemainingCount > 0)
		{
			m_mail = Mail.Create("MAIL_REWARD_N_16", "MAIL_REWARD_D_16", m_currentTime);
			m_mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingCount, itemReward.owned));
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		m_myHero.rewardedDailyServerLevelRankingNo = nDailyServerLevelRankingNo;
		SaveToDB();
		SaveToDB_AddLevelRankingRewardLog(nDailyServerLevelRankingNo, dailyServerLevelRanking.ranking, itemReward.item.id, itemReward.count, itemReward.owned);
		ServerLevelRankingRewardReceiveResponseBody resBody = new ServerLevelRankingRewardReceiveResponseBody();
		resBody.rewardedRankingNo = nDailyServerLevelRankingNo;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_DailyServerLevelRankingReward(m_myHero.id, m_myHero.rewardedDailyServerLevelRankingNo));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToDB_AddLevelRankingRewardLog(int nRankingNo, int nRanking, int nItemId, int nItemCount, bool bItemOwned)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddLevelRankingRewardLog(Guid.NewGuid(), m_myHero.id, nRankingNo, nRanking, nItemId, nItemCount, bItemOwned, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
