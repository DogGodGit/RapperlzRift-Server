using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class NationAllianceApplicationAcceptCommandHandler : InGameCommandHandler<NationAllianceApplicationAcceptCommandBody, NationAllianceApplicationAcceptResponseBody>
{
	public const short kResult_NotExistApplication = 101;

	public const short kResult_NationAllianceUnavailableTime = 102;

	public const short kResult_NotAuthority = 103;

	public const short kResult_NotEnoughFund = 104;

	private NationAllianceApplication m_nationAllianceApplication;

	private NationInstance m_nationInst;

	private NationInstance m_targetNationInst;

	private NationAlliance m_nationAlliance;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null 입니다.");
		}
		Guid nationAllianceApplicationId = (Guid)m_body.applicationId;
		Cache cache = Cache.instance;
		m_nationAllianceApplication = cache.GetNationAllianceApplication(nationAllianceApplicationId);
		if (m_nationAllianceApplication == null)
		{
			throw new CommandHandleException(101, "목표 국가동맹신청이 존재하지 않습니다. nationAllianceApplicationId = " + nationAllianceApplicationId);
		}
		int nTargetNationId = m_nationAllianceApplication.targetNationId;
		if (nTargetNationId != m_myHero.nationId)
		{
			throw new CommandHandleException(1, "영웅의 국가가 신청받은 국가동맹신청이 아닙니다. nationAllianceApplicationId = " + nationAllianceApplicationId);
		}
		m_nationInst = m_nationAllianceApplication.nationInst;
		m_targetNationInst = m_nationAllianceApplication.targetNationInst;
		if (Resource.instance.IsNationAllianceUnavailableTime(m_currentTime))
		{
			throw new CommandHandleException(102, "현재 국가동맹관련 명령을 수행할 수 없는 시간입니다.");
		}
		NationNoblesse nationNoblesse = m_myHero.nationNoblesse;
		nationNoblesse = m_myHero.nationNoblesse;
		if (nationNoblesse == null)
		{
			throw new CommandHandleException(103, "영웅이 관직에 임명되지 않았습니다.");
		}
		if (!nationNoblesse.nationAllianceEnabled)
		{
			throw new CommandHandleException(103, "영웅이 국가동맹신청 권한이 없습니다.");
		}
		long lnNationAllianceRequiredFund = Resource.instance.nationAllianceRequiredFund;
		if (m_targetNationInst.fund < lnNationAllianceRequiredFund)
		{
			throw new CommandHandleException(104, "국가동맹에 필요한 국고자금이 부족합니다.");
		}
		m_targetNationInst.UseFund(lnNationAllianceRequiredFund, m_myHero.id);
		cache.RemoveNationAllianceApplication(nationAllianceApplicationId);
		cache.CancelNationAllianceApplicationOfNationInstance(m_nationInst, m_currentTime, Guid.Empty);
		cache.CancelNationAllianceApplicationOfNationInstance(m_targetNationInst, m_currentTime, m_myHero.id);
		ServerEvent.SendNationAllianceApplicationAccepted(m_nationInst.GetClientPeers(Guid.Empty), m_nationAllianceApplication.id);
		ServerEvent.SendNationAllianceApplicationAccepted(m_targetNationInst.GetClientPeers(m_myHero.id), m_nationAllianceApplication.id);
		m_nationAlliance = new NationAlliance();
		m_nationAlliance.Init(m_currentTime);
		m_nationAlliance.AddNationInstance(m_nationInst);
		m_nationAlliance.AddNationInstance(m_targetNationInst);
		cache.ConcludeNationAlliance(m_nationAlliance, m_myHero.id);
		SaveToDB();
		SaveToDB_Log();
		NationAllianceApplicationAcceptResponseBody resBody = new NationAllianceApplicationAcceptResponseBody();
		resBody.nationAlliance = m_nationAlliance.ToPDNationAlliance(m_currentTime);
		resBody.fund = m_targetNationInst.fund;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		int nNationId = m_nationInst.nationId;
		int nTargetNationId = m_targetNationInst.nationId;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateNationWork(nNationId);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateNationWork(nTargetNationId));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateNationInstance_Fund(nTargetNationId, m_targetNationInst.fund));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateNationAllianceApplication(m_nationAllianceApplication.id, 1, m_currentTime));
		dbWork.AddSqlCommand(GameDac.CSC_AddAlliance(m_nationAlliance.id, m_nationAlliance.regTime));
		foreach (int nAllianceNationId in m_nationAlliance.nationInsts.Keys)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateNationInstance_Alliance(nAllianceNationId, m_nationAlliance.id));
		}
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddNationAllianceLog(m_nationAllianceApplication.id, m_nationAllianceApplication.nationId, m_nationAllianceApplication.fund, m_nationAllianceApplication.targetNationId, Resource.instance.nationAllianceRequiredFund, m_nationAlliance.id, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex);
		}
	}
}
