using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class NationAllianceApplyCommandHandler : InGameCommandHandler<NationAllianceApplyCommandBody, NationAllianceApplyResponseBody>
{
	public const short kResult_NationAllianceUnavailableTime = 101;

	public const short kResult_NotAuthority = 102;

	public const short kResult_ExistMyNationAlliance = 103;

	public const short kResult_ExistTargetNationAlliance = 104;

	public const short kResult_TopRankedMyNation = 105;

	public const short kResult_TopRankedTargetNation = 106;

	public const short kResult_SentNationAllianceApplication = 107;

	public const short kResult_ReceivedNationAllianceApplication = 108;

	public const short kResult_NotEnoughFund = 109;

	public const short kResult_NationWarDeclaration = 110;

	private NationAllianceApplication m_nationAllianceApplication;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null 입니다.");
		}
		int nTargetNationId = m_body.targetNationId;
		Cache cache = Cache.instance;
		NationInstance targetNationInst = cache.GetNationInstance(nTargetNationId);
		if (targetNationInst == null)
		{
			throw new CommandHandleException(1, "목표국가가 존재하지 않습니다. nTargetNationId = " + nTargetNationId);
		}
		int nNationId = m_myHero.nationId;
		if (nTargetNationId == nNationId)
		{
			throw new CommandHandleException(1, "자신의 국가에 국가동맹을 신청할 수 없습니다.");
		}
		if (Resource.instance.IsNationAllianceUnavailableTime(m_currentTime))
		{
			throw new CommandHandleException(101, "현재 국가동맹관련 명령을 수행할 수 없는 시간입니다.");
		}
		NationNoblesse nationNoblesse = m_myHero.nationNoblesse;
		nationNoblesse = m_myHero.nationNoblesse;
		if (nationNoblesse == null)
		{
			throw new CommandHandleException(102, "영웅이 관직에 임명되지 않았습니다.");
		}
		if (!nationNoblesse.nationAllianceEnabled)
		{
			throw new CommandHandleException(102, "영웅이 국가동맹신청 권한이 없습니다.");
		}
		NationInstance nationInst = m_myHero.nationInst;
		if (nationInst.alliance != null)
		{
			throw new CommandHandleException(103, "영웅의국가가 동맹중입니다.");
		}
		if (targetNationInst.alliance != null)
		{
			throw new CommandHandleException(104, "목표국가가 동맹중입니다.");
		}
		if (nationInst.nationPowerRanking.ranking == 1)
		{
			throw new CommandHandleException(105, "영웅국가의 국력랭킹이 높아 신청할 수 없습니다.");
		}
		if (targetNationInst.nationPowerRanking.ranking == 1)
		{
			throw new CommandHandleException(106, "목표국가의 국력랭킹이 높아 신청할 수 없습니다.");
		}
		if (cache.GetNationAllianceApplication_ByNationIdAndTargetNationId(nNationId, nTargetNationId) != null)
		{
			throw new CommandHandleException(107, "이미 목표국가에 동맹을 신청했습니다.");
		}
		if (cache.GetNationAllianceApplication_ByNationIdAndTargetNationId(nTargetNationId, nNationId) != null)
		{
			throw new CommandHandleException(108, "이미 목표국가의 동맹을 신청받았습니다.");
		}
		NationWarManager nationWarManager = Cache.instance.nationWarManager;
		if (nationWarManager.ContainsNationWarDeclarationByNationIdAndTargetNationId(nNationId, nTargetNationId))
		{
			throw new CommandHandleException(110, "목표국가와 국가전이 선포되어 있는 상태입니다.");
		}
		if (nationWarManager.ContainsNationWarDeclarationByNationIdAndTargetNationId(nTargetNationId, nNationId))
		{
			throw new CommandHandleException(110, "목표국가와 국가전이 선포되어 있는 상태입니다.");
		}
		int nNationAllianceRequiredFund = Resource.instance.nationAllianceRequiredFund;
		if (nationInst.fund < nNationAllianceRequiredFund)
		{
			throw new CommandHandleException(109, "국고자금이 부족합니다.");
		}
		nationInst.UseFund(nNationAllianceRequiredFund, m_myHero.id);
		m_nationAllianceApplication = new NationAllianceApplication();
		m_nationAllianceApplication.Init(m_myHero.nationInst, targetNationInst, nNationAllianceRequiredFund, m_currentTime);
		cache.AddNationAllianceApplication(m_nationAllianceApplication);
		SaveToDB();
		ServerEvent.SendNationAllianceApplied(nationInst.GetClientPeers(m_myHero.id), m_nationAllianceApplication.ToPDNationAllianceApplication());
		ServerEvent.SendNationAllianceApplied(m_nationAllianceApplication.targetNationInst.GetClientPeers(Guid.Empty), m_nationAllianceApplication.ToPDNationAllianceApplication());
		NationAllianceApplyResponseBody resBody = new NationAllianceApplyResponseBody();
		resBody.fund = nationInst.fund;
		resBody.application = m_nationAllianceApplication.ToPDNationAllianceApplication();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateNationWork(m_nationAllianceApplication.nationId);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateNationWork(m_nationAllianceApplication.targetNationId));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateNationInstance_Fund(m_nationAllianceApplication.nationId, m_nationAllianceApplication.nationInst.fund));
		dbWork.AddSqlCommand(GameDac.CSC_AddNationAllianceApplication(m_nationAllianceApplication.id, m_nationAllianceApplication.nationId, m_nationAllianceApplication.targetNationId, m_nationAllianceApplication.fund, 0, m_nationAllianceApplication.regTime));
		dbWork.Schedule();
	}
}
