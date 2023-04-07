using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class ProofOfValorAbandonCommandHandler : InGameCommandHandler<ProofOfValorAbandonCommandBody, ProofOfValorAbandonResponseBody>
{
	public const short kResult_NotStatusPlayWaitingOrPlaying = 101;

	private ProofOfValorInstance m_currentPlace;

	private HeroProofOfValorInstance m_heroProofOfValorInst;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private long m_lnRewardExp;

	private int m_nRewardSoulPowder;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentPlace = m_myHero.currentPlace as ProofOfValorInstance;
		if (m_currentPlace == null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_currentPlace.status != 1 && m_currentPlace.status != 2)
		{
			throw new CommandHandleException(101, "현재 상태에서 실행할 수 없는 명령입니다.");
		}
		m_currentPlace.Finish(5);
		m_heroProofOfValorInst = m_myHero.heroProofOfValorInst;
		m_heroProofOfValorInst.status = 4;
		m_heroProofOfValorInst.level = m_myHero.level;
		m_endTime = m_currentPlace.endTime;
		ProofOfValor proofOfValor = Resource.instance.proofOfValor;
		ProofOfValorReward reward = proofOfValor.GetReward(m_myHero.level);
		if (reward != null)
		{
			m_lnRewardExp = reward.failureExpRewardValue;
			m_lnRewardExp = (long)Math.Floor((float)m_lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
			m_myHero.AddExp(m_lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		}
		m_nRewardSoulPowder = proofOfValor.failureRewardSoulPowder;
		m_myHero.AddSoulPowder(m_nRewardSoulPowder);
		SaveToDB();
		SaveToDB_Log();
		m_myHero.CreateHeroProofOfValorInstance(m_endTime, bIsRefreshPaidCount: true);
		if (m_myHero.isDead)
		{
			m_myHero.Revive(bSendEvent: false);
		}
		else
		{
			m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		}
		m_currentPlace.Exit(m_myHero, isLogOut: false, null);
		ProofOfValorAbandonResponseBody resBody = new ProofOfValorAbandonResponseBody();
		resBody.previousContinentId = m_myHero.previousContinentId;
		resBody.previousNationId = m_myHero.previousNationId;
		resBody.acquiredExp = m_lnRewardExp;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.soulPowder = m_myHero.soulPowder;
		resBody.heroProofOfValorInst = m_myHero.heroProofOfValorInst.ToPDHeroProofOfValorInstance();
		resBody.proofOfValorPaidRefreshCount = m_myHero.proofOfValorPaidRefreshCount;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroProofOfValorInstance(m_heroProofOfValorInst.id, m_heroProofOfValorInst.status, m_heroProofOfValorInst.level, m_currentPlace.playTime, m_endTime));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SoulPowder(m_myHero.id, m_myHero.soulPowder));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroProofOfValorRewardLog(Guid.NewGuid(), m_myHero.id, m_heroProofOfValorInst.id, 3, 0, m_nRewardSoulPowder, m_lnRewardExp, 0, m_endTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}
}
