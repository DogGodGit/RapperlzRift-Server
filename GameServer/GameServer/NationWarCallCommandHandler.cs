using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class NationWarCallCommandHandler : InGameCommandHandler<NationWarCallCommandBody, NationWarCallResponseBody>
{
	public const short kResult_NoNationWar = 101;

	public const short kResult_NotJoinedNationWar = 102;

	public const short kResult_NotAuthorityNationWarCall = 103;

	public const short kResult_OverFlowedNationWarCallCount = 104;

	public const short kResult_NotExpiredCoolTime = 105;

	private DateValuePair<int> m_dailyNationWarCallCount;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is NationContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		NationInstance nationInst = m_myHero.nationInst;
		NationWarInstance nationWarInst = nationInst.nationWarInst;
		if (nationWarInst == null)
		{
			throw new CommandHandleException(101, "자신의 국가가 국가전 진행중이 아닙니다.");
		}
		if (!nationWarInst.ContainsRealJoinNationHero(m_myHero))
		{
			throw new CommandHandleException(1, "영웅이 국가전에 참여하고 있지 않습니다.");
		}
		NationNoblesse nationNoblesse = m_myHero.nationNoblesse;
		if (nationNoblesse == null)
		{
			throw new CommandHandleException(103, "국가전소집 권한이 없습니다.");
		}
		if (!nationNoblesse.nationWarCallEnabled)
		{
			throw new CommandHandleException(103, "국가전소집 권한이 없습니다.");
		}
		if (currentPlace.nationId != nationWarInst.defenseNation.id)
		{
			throw new CommandHandleException(1, "수비국가에서만 사용할 수 있는 명령입니니다.");
		}
		if (!currentPlace.continent.isNationWarTarget)
		{
			throw new CommandHandleException(1, "현재 대륙이 국가전 대상대륙이 아닙니다.");
		}
		NationWar nationWar = Resource.instance.nationWar;
		NationInstance myHeroNationInst = m_myHero.nationInst;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		myHeroNationInst.RefreshDailyNationWarCallCount(currentDate);
		m_dailyNationWarCallCount = myHeroNationInst.dailyNationWarCallCount;
		if (m_dailyNationWarCallCount.value >= nationWar.nationCallCount)
		{
			throw new CommandHandleException(104, "국가전소집 최대횟수를 초과하였습니다.");
		}
		float fRemainingCoolTime = nationInst.GetNationWarCallRemainingCoolTime(currentTime);
		if (fRemainingCoolTime > 0f)
		{
			throw new CommandHandleException(105, "국가전소집 쿨타임이 만료되지 않았습니다. fRemainingCoolTime = " + fRemainingCoolTime);
		}
		m_dailyNationWarCallCount.value++;
		nationInst.RegistNationWarCall(m_myHero, currentTime);
		SaveToDB();
		SaveToDB_Log(nationInst.nationWarCall);
		NationWarCallResponseBody resBody = new NationWarCallResponseBody();
		resBody.date = (DateTime)m_dailyNationWarCallCount.date;
		resBody.dailyNationWarCallCount = m_dailyNationWarCallCount.value;
		resBody.nationWarCallRemainingCoolTime = nationInst.GetNationWarCallRemainingCoolTime(currentTime);
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_Nation);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_myHero.id));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateNationInstance_NationWarCallDateCount(m_myHero.nationId, m_dailyNationWarCallCount.date, m_dailyNationWarCallCount.value));
		dbWork.Schedule();
	}

	private void SaveToDB_Log(NationWarCall nationWarCall)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddNationWarCallLog(Guid.NewGuid(), nationWarCall.nationWarInst.declaration.id, m_myHero.id, nationWarCall.continent.id, nationWarCall.position.x, nationWarCall.position.y, nationWarCall.position.z, nationWarCall.regTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
