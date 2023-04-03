using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class RestRewardReceiveFreeCommandHandler : InGameCommandHandler<RestRewardReceiveFreeCommandBody, RestRewardReceiveFreeResponseBody>
{
	public const short kResult_LevelUnderflowed = 101;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		Resource res = Resource.instance;
		int nHeroLevel = m_myHero.level;
		int nRequiredHeroLevel = res.restRewardRequiredHeroLevel;
		if (nHeroLevel < nRequiredHeroLevel)
		{
			throw new CommandHandleException(101, "영웅의 레벨이 낮아 휴식보상을 받을 수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroLevel = " + nRequiredHeroLevel);
		}
		int nRestTime = m_myHero.restTime;
		RestRewardTime targetRestTime = Resource.instance.GetTargetRestRewardTime(nRestTime);
		if (targetRestTime == null)
		{
			throw new CommandHandleException(1, "해당 휴식시간에 대한 보상이 존재하지 않습니다. nRestTime = " + nRestTime);
		}
		int nLastRestTime = res.lastRestRewardTime.restTime;
		Job job = m_myHero.job;
		JobLevelMaster jobLevelMaster = job.GetLevel(m_myHero.level).master;
		long lnRewardExp = jobLevelMaster.restMaxExpRewardValue;
		long lnResultRewardExp = (long)((float)lnRewardExp * (float)targetRestTime.restTime / (float)nLastRestTime);
		m_myHero.AddExp(lnResultRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		m_myHero.restTime = 0;
		SaveToDB(jobLevelMaster.level, nRestTime, lnResultRewardExp, 0L, 0, 0);
		RestRewardReceiveFreeResponseBody resBody = new RestRewardReceiveFreeResponseBody();
		resBody.acquiredExp = lnResultRewardExp;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nOldLevel, int nOldRestTime, long lnRewardExp, long lnUsedGold, int nUsedOwnDia, int nUsedUnOwnDia)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_RestTime(m_myHero.id, 0));
		dbWork.Schedule();
		SaveToDB_AddRestRewardReceiveLog(nOldLevel, nOldRestTime, lnRewardExp, 1, lnUsedGold, nUsedOwnDia, nUsedUnOwnDia);
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
