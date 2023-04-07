using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class ProofOfValorRefreshCommandHandler : InGameCommandHandler<ProofOfValorRefreshCommandBody, ProofOfValorRefreshResponseBody>
{
	public const short kResult_NotEnoughtDia = 101;

	public const short kResult_DailyPaidRefreshCountOverflowed = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue;

	private bool m_bIsFreeRefresh = true;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		int nPaidDia = 0;
		ProofOfValor proofOfValor = Resource.instance.proofOfValor;
		m_myHero.RefreshDailyProofOfValorFreeRefreshCount(m_currentDate);
		m_myHero.RefreshDailyProofOfValorPaidRefreshCount(m_currentDate);
		if (m_myHero.dailyProofOfValorFreeRefreshCount.value >= proofOfValor.dailyFreeRefreshCount)
		{
			if (m_myHero.dailyProofOfValorPaidRefreshCount.value >= proofOfValor.dailyPaidRefreshCount)
			{
				throw new CommandHandleException(102, "일일유료갱신횟수가 초과되었습니다.");
			}
			nPaidDia = proofOfValor.GetPaidRefresh(m_myHero.proofOfValorPaidRefreshCount + 1).requiredDia;
			if (m_myHero.dia < nPaidDia)
			{
				throw new CommandHandleException(101, "다이아가 부족합니다. myHeroDia = " + m_myHero.dia + ", nPaidDia = " + nPaidDia);
			}
			m_bIsFreeRefresh = false;
		}
		if (m_bIsFreeRefresh)
		{
			m_myHero.dailyProofOfValorFreeRefreshCount.value++;
		}
		else
		{
			m_myHero.UseDia(nPaidDia, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
			m_myHero.dailyProofOfValorPaidRefreshCount.value++;
			m_myHero.proofOfValorPaidRefreshCount++;
		}
		m_myHero.RefreshHeroProofOfValorInstance(m_currentTime);
		SaveToDB();
		SaveToDB_Log();
		ProofOfValorRefreshResponseBody resBody = new ProofOfValorRefreshResponseBody();
		resBody.heroProofOfValorInst = m_myHero.heroProofOfValorInst.ToPDHeroProofOfValorInstance();
		resBody.date = (DateTime)m_currentDate;
		resBody.dailyProofOfValorFreeRefreshCount = m_myHero.dailyProofOfValorFreeRefreshCount.value;
		resBody.dailyProofOfValorPaidRefreshCount = m_myHero.dailyProofOfValorPaidRefreshCount.value;
		resBody.proofOfValorPaidRefreshCount = m_myHero.proofOfValorPaidRefreshCount;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myHero.account.id));
		HeroProofOfValorInstance inst = m_myHero.heroProofOfValorInst;
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroProofOfValorInstance(inst.id, inst.hero.id, inst.bossMonsterArrange.id, inst.creatureCard.id, inst.status, 0, 0, m_currentTime));
		if (m_bIsFreeRefresh)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ProofOfValorFreeRefresh(m_myHero.id, m_currentDate, m_myHero.dailyProofOfValorFreeRefreshCount.value));
		}
		else
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ProofOfValorDailyPaidRefresh(m_myHero.id, m_currentDate, m_myHero.dailyProofOfValorPaidRefreshCount.value));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ProofOfValorPaidRefreshCount(m_myHero.id, m_myHero.proofOfValorPaidRefreshCount));
			if (m_nUsedOwnDia > 0)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
			}
			if (m_nUsedUnOwnDia > 0)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myHero.account));
			}
		}
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
		int nType = (m_bIsFreeRefresh ? 1 : 2);
		logWork.AddSqlCommand(GameLogDac.CSC_AddHeroProofOfValorRefreshLog(Guid.NewGuid(), m_myHero.id, nType, m_myHero.heroProofOfValorInst.id, m_nUsedOwnDia, m_nUsedUnOwnDia, m_currentTime));
		logWork.Schedule();
	}
}
