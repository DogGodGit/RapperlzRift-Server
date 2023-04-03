using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class FieldOfHonorRankingRewardReceiveCommandHandler : InGameCommandHandler<FieldOfHonorRankingRewardReceiveCommandBody, FieldOfHonorRankingRewardReceiveResponseBody>
{
	public const short kResult_NotRegistrationRanking = 101;

	public const short kResult_ReceivedReward = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private int m_nDailyFieldOfHonorRankingNo;

	private List<PDItemBooty> m_booties = new List<PDItemBooty>();

	private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

	private Mail m_mail;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다");
		}
		int nRanking = DailyFieldOfHonorRankingManager.instance.GetRankingOfHero(m_myHero.id)?.ranking ?? 0;
		if (nRanking <= 0)
		{
			throw new CommandHandleException(101, "미등록 랭킹은 보상을 받을 수 없습니다.");
		}
		DailyFieldOfHonorRankingManager manager = DailyFieldOfHonorRankingManager.instance;
		int nRewardedDailyFieldOfHonorRankingNo = m_myHero.rewardedDailyFieldOfHonorRankingNo;
		m_nDailyFieldOfHonorRankingNo = manager.rankingNo;
		if (nRewardedDailyFieldOfHonorRankingNo >= m_nDailyFieldOfHonorRankingNo)
		{
			throw new CommandHandleException(102, "이미 보상을 받았습니다. nRewardedDailyFieldOfHonorRankingNo = " + nRewardedDailyFieldOfHonorRankingNo + ", m_nDailyFieldOfHonorRankingNo = " + m_nDailyFieldOfHonorRankingNo);
		}
		m_currentTime = DateTimeUtil.currentTime;
		FieldOfHonor fieldOfHonor = Resource.instance.fieldOfHonor;
		List<ItemReward> itemRewards = fieldOfHonor.GetRankingReward(nRanking);
		new List<PDItemBooty>();
		foreach (ItemReward itemReward in itemRewards)
		{
			Item item = itemReward.item;
			int nCount = itemReward.count;
			bool bOwned = itemReward.owned;
			int nRewardItemRemainCount = m_myHero.AddItem(item, bOwned, nCount, m_changedInventorySlots);
			PDItemBooty booty = new PDItemBooty();
			booty.id = item.id;
			booty.count = nCount;
			booty.owned = bOwned;
			m_booties.Add(booty);
			if (nRewardItemRemainCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_REWARD_N_3", "MAIL_REWARD_D_3", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(item, nRewardItemRemainCount, bOwned));
			}
		}
		m_myHero.rewardedDailyFieldOfHonorRankingNo = m_nDailyFieldOfHonorRankingNo;
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB();
		SaveToDB_Log(nRanking);
		FieldOfHonorRankingRewardReceiveResponseBody resBody = new FieldOfHonorRankingRewardReceiveResponseBody();
		resBody.rewardedRankingNo = m_nDailyFieldOfHonorRankingNo;
		resBody.booties = m_booties.ToArray();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_RewardedDailyFieldOfHonorRankingNo(m_myHero.id, m_myHero.rewardedDailyFieldOfHonorRankingNo));
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToDB_Log(int nRanking)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddFieldOfHonorRankingRewardLog(logId, m_myHero.id, m_nDailyFieldOfHonorRankingNo, nRanking, m_currentTime));
			foreach (PDItemBooty booty in m_booties)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddFieldOfHonorRankingRewardDetailLog(Guid.NewGuid(), logId, booty.id, booty.count, booty.owned, m_currentTime));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
