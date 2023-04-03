using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class NationWarConvergingAttackCommandHandler : InGameCommandHandler<NationWarConvergingAttackCommandBody, NationWarConvergingAttackResponseBody>
{
	public const short kResult_NoNationWar = 101;

	public const short kResult_NotJoinedNationWar = 102;

	public const short kResult_NotAuthorityConvergingAttack = 103;

	public const short kResult_OverFlowedConvergignAttackCount = 105;

	public const short kResult_NotExpiredCoolTime = 106;

	private DateValuePair<int> m_dailyConvergingAttackCount;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is NationContinentInstance))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nTargetMonsterArrangeId = m_body.targetMonsterArrangeId;
		NationWar nationWar = Resource.instance.nationWar;
		if (nationWar.GetMonsterArrange(nTargetMonsterArrangeId) == null)
		{
			throw new CommandHandleException(1, "목표 몬스터배치ID가 유효하지 않습니다. nTargetMonsterArrangeId = " + nTargetMonsterArrangeId);
		}
		_ = Cache.instance;
		NationInstance myHeroNationInst = m_myHero.nationInst;
		NationWarInstance nationWarInst = myHeroNationInst.nationWarInst;
		if (nationWarInst == null)
		{
			throw new CommandHandleException(101, "자신의 국가가 국가전 진행중이 아닙니다.");
		}
		NationNoblesse nationNoblesse = m_myHero.nationNoblesse;
		if (nationNoblesse == null)
		{
			throw new CommandHandleException(103, "집중공격 권한이 없습니다.");
		}
		if (!nationNoblesse.nationWarConvergingAttackEnabled)
		{
			throw new CommandHandleException(103, "집중공격 권한이 없습니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		myHeroNationInst.RefreshDailyConvergingAttackCount(currentDate);
		m_dailyConvergingAttackCount = myHeroNationInst.dailyConvergingAttackCount;
		if (m_dailyConvergingAttackCount.value >= nationWar.convergingAttackCount)
		{
			throw new CommandHandleException(105, "집중공격 최대횟수를 초과하였습니다.");
		}
		float fRemainingCoolTime = myHeroNationInst.GetNationWarConvergingAttackRemainingCoolTime(currentTime);
		if (fRemainingCoolTime > 0f)
		{
			throw new CommandHandleException(106, "집중공격 쿨타임이 만료되지 않았습니다. fRemainingCoolTime = " + fRemainingCoolTime);
		}
		m_dailyConvergingAttackCount.value++;
		myHeroNationInst.RegistNationWarConvergingAttack(m_myHero, nTargetMonsterArrangeId, currentTime);
		SaveToDB();
		SaveToDB_Log(myHeroNationInst.nationWarConvergingAttack);
		NationWarConvergingAttackResponseBody resBody = new NationWarConvergingAttackResponseBody();
		resBody.date = (DateTime)m_dailyConvergingAttackCount.date;
		resBody.dailyNationWarConvergingAttackCount = m_dailyConvergingAttackCount.value;
		resBody.nationWarConvergingAttackReminaingCoolTime = myHeroNationInst.GetNationWarConvergingAttackRemainingCoolTime(currentTime);
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_Nation);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_myHero.id));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateNationInstance_ConvergingAttackDateCount(m_myHero.nationId, m_dailyConvergingAttackCount.date, m_dailyConvergingAttackCount.value));
		dbWork.Schedule();
	}

	private void SaveToDB_Log(NationWarConvergingAttack nationWarConvergingAttack)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddNationWarConvergingAttackLog(Guid.NewGuid(), nationWarConvergingAttack.nationWarInst.declaration.id, m_myHero.id, nationWarConvergingAttack.targetArrangeId, nationWarConvergingAttack.regTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
