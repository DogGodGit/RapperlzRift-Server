using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class StaminaBuyCommandHandler : InGameCommandHandler<StaminaBuyCommandBody, StaminaBuyResponseBody>
{
	public const short kResult_DailyBuyCountOverflowed = 101;

	public const short kResult_NotEnoughDia = 102;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		m_myHero.RefreshDailyStaminaBuyCount(currentDate);
		DateValuePair<int> dailyStaminaBuyCount = m_myHero.dailyStaminaBuyCount;
		if (dailyStaminaBuyCount.value >= m_myHero.vipLevel.staminaBuyMaxCount)
		{
			throw new CommandHandleException(101, "일일횟수가 최대횟수를 넘어갑니다.");
		}
		if (dailyStaminaBuyCount.value >= Resource.instance.lastStaminaBuyCount.buyCount)
		{
			throw new CommandHandleException(1, "최대스태미너구매횟수 입니다.");
		}
		StaminaBuyCount staminaBuyCount = Resource.instance.GetStaminaBuyCount(dailyStaminaBuyCount.value + 1);
		if (staminaBuyCount == null)
		{
			throw new CommandHandleException(1, "존재하지 않은 스태미너구매횟수입니다.");
		}
		int nRequiredDia = staminaBuyCount.requiredDia;
		if (m_myHero.dia < nRequiredDia)
		{
			throw new CommandHandleException(102, "다이아가 부족합니다.");
		}
		m_myHero.UseDia(nRequiredDia, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
		int nOldStamina = m_myHero.stamina;
		m_myHero.AddStamina(staminaBuyCount.stamina, bOverflowEnabled: true);
		dailyStaminaBuyCount.value = staminaBuyCount.buyCount;
		SaveToDB();
		SaveToGameLogDB(nOldStamina);
		StaminaBuyResponseBody resBody = new StaminaBuyResponseBody();
		resBody.date = (DateTime)dailyStaminaBuyCount.date;
		resBody.dailyBuyCount = dailyStaminaBuyCount.value;
		resBody.stamina = m_myHero.stamina;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		SendResponseOK(resBody);
		m_myHero.ProcessOrdealQuestMissions(OrdealQuestMissionType.BuyStamina, 1, m_currentTime);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Stamina(m_myHero.id, m_myHero.stamina));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_StaminaBuyDateCount(m_myHero.id, m_myHero.dailyStaminaBuyCount.date, m_myHero.dailyStaminaBuyCount.value));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nOldStamina)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddStaminaBuyLog(Guid.NewGuid(), m_myHero.id, nOldStamina, m_myHero.stamina, m_myHero.dailyStaminaBuyCount.value, m_nUsedOwnDia, m_nUsedUnOwnDia, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
