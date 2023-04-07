using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class NationWarDeclarationCommandHandler : InGameCommandHandler<NationWarDeclarationCommandBody, NationWarDeclarationResponseBody>
{
	public const short kResult_NotAuthorityDeclaration = 101;

	public const short kResult_NotEnabledDeclaration_MyNation = 102;

	public const short kResult_NotEnabledDeclaration_TargetNation = 103;

	public const short kResult_NotEnabledDayOfWeek = 104;

	public const short kResult_NotEnabledDayCount = 105;

	public const short kResult_NotEnabledTime = 106;

	public const short kResult_NotEnoughNationFund = 107;

	public const short kResult_OverFlowedDeclarationCount = 108;

	public const short kResult_Alliance = 109;

	private NationInstance m_nationInst;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null 입니다.");
		}
		int nTargetNationId = m_body.targetNationId;
		Resource res = Resource.instance;
		Nation targetNation = res.GetNation(nTargetNationId);
		if (targetNation == null)
		{
			throw new CommandHandleException(1, "해당국가ID가 유효하지 않습니다. nTargetNationId = " + nTargetNationId);
		}
		int nMyNationId = m_myHero.nationId;
		if (nTargetNationId == nMyNationId)
		{
			throw new CommandHandleException(1, "자신의 국가에 국가전을 선포할 수 없습니다.");
		}
		NationNoblesse nationNoblesse = m_myHero.nationNoblesse;
		if (nationNoblesse == null)
		{
			throw new CommandHandleException(101, "현재 관직을 가지고 있지 않습니다.");
		}
		if (!nationNoblesse.nationWarDeclarationEnabled)
		{
			throw new CommandHandleException(101, "국가전선포 권한이 없습니다.");
		}
		m_nationInst = m_myHero.nationInst;
		NationAlliance nationAlliance = m_nationInst.alliance;
		if (nationAlliance != null && nationAlliance.IsAlliance(nMyNationId, nTargetNationId))
		{
			throw new CommandHandleException(109, "동맹에게 국가전을 선포할 수 없습니다.");
		}
		NationWarManager nationWarManager = Cache.instance.nationWarManager;
		if (nationWarManager.ContainsNationWarDeclarationByNationId(nMyNationId))
		{
			throw new CommandHandleException(102, "이미 자신의 국가가 국가전을 선포 또는 선포당한 상태입니다.");
		}
		if (nationWarManager.ContainsNationWarDeclarationByNationId(nTargetNationId))
		{
			throw new CommandHandleException(103, "이미 해당국가가 국가전을 선포 또는 선포당한 상태입니다.");
		}
		NationWar nationWar = res.nationWar;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		if ((currentDate - Cache.instance.serverOpenDate).TotalDays < (double)nationWar.declarationAvailableServerOpenCount)
		{
			throw new CommandHandleException(105, "국가전포를 할 수 없는 서버오픈일수 입니다.");
		}
		if (!nationWar.ContainsAvailableDayOfWeek(currentDate.DayOfWeek))
		{
			throw new CommandHandleException(104, "국가전 가능 요일이 아닙니다.");
		}
		if (!nationWar.IsEnabledNationWarDeclaration(currentTime))
		{
			throw new CommandHandleException(106, "국가전선포를 할 수 없는 시간입니다.");
		}
		int nDeclarationRequredNationFund = nationWar.declarationRequredNationFund;
		if (m_nationInst.fund < nDeclarationRequredNationFund)
		{
			throw new CommandHandleException(107, "국고자금이 부족합니다.");
		}
		m_nationInst.RefreshWeeklyNationWarDeclarationCount(currentDate);
		DateValuePair<int> weeklyNationWarDeclarationCount = m_nationInst.weeklyNationWarDeclarationCount;
		if (weeklyNationWarDeclarationCount.value >= nationWar.weeklyDeclarationMaxCount)
		{
			throw new CommandHandleException(108, "국가전선포 최대횟수를 초과하였습니다.");
		}
		m_nationInst.UseFund(nDeclarationRequredNationFund, m_myHero.id);
		weeklyNationWarDeclarationCount.value++;
		NationAllianceApplication myNationSentNationAllianceApplication = Cache.instance.GetNationAllianceApplication_ByNationIdAndTargetNationId(nMyNationId, nTargetNationId);
		if (myNationSentNationAllianceApplication != null)
		{
			Cache.instance.CancelNationAllianceApplication(myNationSentNationAllianceApplication, nMyNationId, currentTime);
		}
		NationAllianceApplication targetNationSentNationAllianceApplication = Cache.instance.GetNationAllianceApplication_ByNationIdAndTargetNationId(nTargetNationId, nMyNationId);
		if (targetNationSentNationAllianceApplication != null)
		{
			Cache.instance.CancelNationAllianceApplication(targetNationSentNationAllianceApplication, nMyNationId, currentTime);
		}
		NationWarDeclaration declaration = new NationWarDeclaration();
		declaration.Init(m_nationInst.nation, targetNation, currentTime);
		nationWarManager.AddNationWarDeclaration(declaration);
		SaveToDB(declaration);
		SaveToDB_Log(declaration, nDeclarationRequredNationFund);
		ServerEvent.SendNationWarDeclaration(Cache.instance.GetClientPeers(m_myHero.id), declaration.ToPDNationWarDeclaration());
		NationWarDeclarationResponseBody resBody = new NationWarDeclarationResponseBody();
		resBody.declaration = declaration.ToPDNationWarDeclaration();
		resBody.weeklyNationWarDeclarationCount = weeklyNationWarDeclarationCount.value;
		resBody.nationFund = m_nationInst.fund;
		SendResponseOK(resBody);
	}

	private void SaveToDB(NationWarDeclaration declaration)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateNationWork(declaration.nationId);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateNationWork(declaration.targetNationId));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateNationInstance_Fund(m_nationInst.nationId, m_nationInst.fund));
		dbWork.AddSqlCommand(GameDac.CSC_AddNationWarDeclaration(declaration.id, declaration.nationId, declaration.targetNationId, 0, declaration.regTime));
		dbWork.Schedule();
	}

	private void SaveToDB_Log(NationWarDeclaration declaration, int nUsedNationFund)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddNationWarDeclarationLog(Guid.NewGuid(), 2, declaration.id, declaration.nationId, m_myHero.id, declaration.targetNationId, nUsedNationFund, declaration.regTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
