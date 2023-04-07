using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class GuildDonateCommnadHandler : InGameCommandHandler<GuildDonateCommandBody, GuildDonateResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_DailyDonationCountOverflowed = 102;

	public const short kResult_NotEnoughGold = 103;

	public const short kResult_NotEnoughDia = 104;

	private Guild m_myGuild;

	private GuildMember m_myGuildMember;

	private long m_lnUsedGold;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

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
		int nEntryId = m_body.entryId;
		if (nEntryId <= 0)
		{
			throw new CommandHandleException(1, "유효하지 않는 항목ID입니다. nEntryId = " + nEntryId);
		}
		GuildDonationEntry donationEntry = Resource.instance.GetGuildDonationEntry(nEntryId);
		if (donationEntry == null)
		{
			throw new CommandHandleException(1, "기부항목이 존재하지 않습니다. nEntryId = " + nEntryId);
		}
		m_myGuildMember = m_myHero.guildMember;
		if (m_myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되지 않았습니다.");
		}
		m_myGuild = m_myGuildMember.guild;
		m_myHero.RefreshDailyGuildDonationCount(currentDate);
		DateValuePair<int> dailyGuildDonationCount = m_myHero.dailyGuildDonationCount;
		if (dailyGuildDonationCount.value >= m_myHero.vipLevel.guildDonationMaxCount)
		{
			throw new CommandHandleException(102, "일일기부횟수가 최대입니다.");
		}
		switch (donationEntry.moneyType)
		{
		case GuildDonationEntryMoneyType.Gold:
		{
			long lnPrice = donationEntry.moneyAmount;
			if (m_myHero.gold < lnPrice)
			{
				throw new CommandHandleException(103, "골드가 부족합니다.");
			}
			m_myHero.UseGold(lnPrice);
			m_lnUsedGold = lnPrice;
			break;
		}
		case GuildDonationEntryMoneyType.Dia:
		{
			int nPrice = (int)donationEntry.moneyAmount;
			if (m_myHero.dia < nPrice)
			{
				throw new CommandHandleException(104, "다이아가 부족합니다.");
			}
			m_myHero.UseDia(nPrice, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
			break;
		}
		}
		int nRewardGuildFund = donationEntry.fundRewardValue;
		int nRewardGuildContributionPoint = donationEntry.contributionPointRewardValue;
		m_myGuild.AddFund(nRewardGuildFund, m_myHero.id);
		m_myHero.AddGuildContributionPoint(nRewardGuildContributionPoint);
		dailyGuildDonationCount.value++;
		SaveToDB();
		SaveToGameLogDB(nEntryId, nRewardGuildContributionPoint, nRewardGuildFund);
		GuildDonateResponseBody resBody = new GuildDonateResponseBody();
		resBody.date = (DateTime)dailyGuildDonationCount.date;
		resBody.dailyDonationCount = dailyGuildDonationCount.value;
		resBody.gold = m_myHero.gold;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		resBody.totalContributionPoint = m_myHero.totalGuildContributionPoint;
		resBody.contributionPoint = m_myHero.guildContributionPoint;
		resBody.fund = m_myGuild.fund;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_myGuild.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_myHero.id));
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_GuildDonationDateCount(m_myHero.id, m_myHero.dailyGuildDonationCount.date, m_myHero.dailyGuildDonationCount.value));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_GuildContributionPoint(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_Fund(m_myGuild.id, m_myGuild.fund));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nEntryId, int nRewardGuildContributionPoint, int nRewardGuildFund)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildDonationLog(Guid.NewGuid(), m_myGuild.id, m_myHero.id, nEntryId, m_lnUsedGold, m_nUsedOwnDia, m_nUsedUnOwnDia, nRewardGuildContributionPoint, nRewardGuildFund, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
