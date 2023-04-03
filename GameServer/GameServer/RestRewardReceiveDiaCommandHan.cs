using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class RestRewardReceiveDiaCommandHandler : InGameCommandHandler<RestRewardReceiveDiaCommandBody, RestRewardReceiveDiaResponseBody>
{
	public const short kResult_NotEnoughDia = 101;

	public const short kResult_LevelUnderflowed = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		Resource res = Resource.instance;
		int nHeroLevel = m_myHero.level;
		int nRequiredHeroLevel = res.restRewardRequiredHeroLevel;
		if (nHeroLevel < nRequiredHeroLevel)
		{
			throw new CommandHandleException(102, "영웅의 레벨이 낮아 휴식보상을 받을 수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroLevel = " + nRequiredHeroLevel);
		}
		int nRestTime = m_myHero.restTime;
		RestRewardTime targetRestRewardTime = Resource.instance.GetTargetRestRewardTime(nRestTime);
		if (targetRestRewardTime == null)
		{
			throw new CommandHandleException(1, "해당 휴식시간에 대한 보상이 존재하지 않습니다. nRestTime = " + nRestTime);
		}
		int nPriceDia = targetRestRewardTime.requiredDia;
		if (m_myHero.dia < nPriceDia)
		{
			throw new CommandHandleException(101, "다이아가 부족합니다.");
		}
		int nLastRestTime = res.lastRestRewardTime.restTime;
		Job job = m_myHero.job;
		JobLevelMaster jobLevelMaster = job.GetLevel(m_myHero.level).master;
		long lnRewardExp = jobLevelMaster.restMaxExpRewardValue;
		long lnResultRewardExp = (long)((float)lnRewardExp * (float)targetRestRewardTime.restTime / (float)nLastRestTime);
		lnResultRewardExp = (long)((float)(lnResultRewardExp * res.restRewardDiaReceiveExpRate) / 10000f);
		int nUsedOwnDia = 0;
		int nUsedUnOwnDia = 0;
		m_myHero.UseDia(nPriceDia, m_currentTime, out nUsedOwnDia, out nUsedUnOwnDia);
		m_myHero.AddExp(lnResultRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		m_myHero.restTime = 0;
		SaveToDB(jobLevelMaster.level, nRestTime, lnResultRewardExp, 0L, nUsedOwnDia, nUsedUnOwnDia);
		RestRewardReceiveDiaResponseBody resBody = new RestRewardReceiveDiaResponseBody();
		resBody.acquiredExp = lnResultRewardExp;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nOldLevel, int nOldRestTime, long lnRewardExp, long lnUsedGold, int nUsedOwnDia, int nUsedUnOwnDia)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_RestTime(m_myHero.id, 0));
		dbWork.Schedule();
		SaveToDB_AddRestRewardReceiveLog(nOldLevel, nOldRestTime, lnRewardExp, 3, lnUsedGold, nUsedOwnDia, nUsedUnOwnDia);
	}

	private void SaveToDB_AddRestRewardReceiveLog(int nOldLevel, int nOldRestTime, long lnRewardExp, int nType, long lnUsedGold, int nUsedOwnDia, int nUsedUnOwnDia)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroRestRewardLog(Guid.NewGuid(), m_myHero.id, nOldLevel, nOldRestTime, lnRewardExp, nType, lnUsedGold, nUsedOwnDia, nUsedUnOwnDia, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
