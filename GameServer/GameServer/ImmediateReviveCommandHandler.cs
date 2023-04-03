using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ImmediateReviveCommandHandler : InGameCommandHandler<ImmediateReviveCommandBody, ImmediateReviveResponseBody>
{
	public const short kResult_NotDead = 101;

	public const short kResult_NotEnoughtDia = 102;

	protected override void HandleInGameCommand()
	{
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace == null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은상태가 아닙니다.");
		}
		bool bIsFreeImmediateRevival = true;
		int nPaidDia = 0;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		m_myHero.RefreshFreeImmediateRevivalDailyCount(currentDate);
		m_myHero.RefreshPaidImmediateRevivalDailyCount(currentDate);
		if (m_myHero.freeImmediateRevivalDailyCount.value >= Resource.instance.freeImmediateRevivalDailyCount)
		{
			nPaidDia = Resource.instance.GetPaidImmediateRevival(m_myHero.paidImmediateRevivalDailyCount.value + 1).requiredDia;
			if (m_myHero.dia < nPaidDia)
			{
				throw new CommandHandleException(102, "다이아가 부족합니다. myHeroDia = " + m_myHero.dia + ", nPaidDia = " + nPaidDia);
			}
			bIsFreeImmediateRevival = false;
		}
		int nRevivalType = 0;
		int nUsedOwnDia = 0;
		int nUsedUnOwnDia = 0;
		if (bIsFreeImmediateRevival)
		{
			m_myHero.freeImmediateRevivalDailyCount.value++;
			nRevivalType = 2;
		}
		else
		{
			m_myHero.UseDia(nPaidDia, currentTime, out nUsedOwnDia, out nUsedUnOwnDia);
			m_myHero.paidImmediateRevivalDailyCount.value++;
			nRevivalType = 3;
		}
		m_myHero.Revive(bSendEvent: true);
		int nFreeImmediateRevivalDailyCount = m_myHero.freeImmediateRevivalDailyCount.value;
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myHero.account.id));
		if (bIsFreeImmediateRevival)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_FreeImmediateRevivalCount(m_myHero.id, currentDate, nFreeImmediateRevivalDailyCount));
		}
		else
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, currentDate, nPaidImmediateRevivalDailyCount));
			if (nUsedOwnDia > 0)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
			}
			if (nUsedUnOwnDia > 0)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myHero.account));
			}
		}
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroRevivalLog(Guid.NewGuid(), m_myHero.id, nRevivalType, nUsedOwnDia, nUsedUnOwnDia, currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
		if (currentPlace is NationContinentInstance currentNationContinentInst)
		{
			NationInstance nationInst = m_myHero.nationInst;
			NationWarInstance nationWarInst = nationInst.nationWarInst;
			if (nationWarInst != null && currentNationContinentInst.nationId == nationWarInst.defenseNation.id && currentNationContinentInst.continent.isNationWarTarget)
			{
				m_myHero.NationWarIncreaseImmediateRevivalCount(currentTime);
			}
		}
		ImmediateReviveResponseBody resBody = new ImmediateReviveResponseBody();
		resBody.hp = m_myHero.hp;
		resBody.date = (DateTime)currentDate;
		resBody.freeImmediateRevivalDailyCount = nFreeImmediateRevivalDailyCount;
		resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		SendResponseOK(resBody);
	}
}
