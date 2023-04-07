using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class NationAllianceApplicationRejectCommandHandler : InGameCommandHandler<NationAllianceApplicationRejectCommandBody, NationAllianceApplicationRejectResponseBody>
{
	public const short kResult_NotExistApplication = 101;

	public const short kResult_NationAllianceUnavailableTime = 102;

	public const short kResult_NotAuthority = 103;

	private NationAllianceApplication m_nationAllianceApplication;

	private NationInstance m_nationInst;

	private NationInstance m_targetNationInst;

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
		if (m_nationAllianceApplication.targetNationId != m_myHero.nationId)
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
		cache.RemoveNationAllianceApplication(nationAllianceApplicationId);
		m_nationInst.AddFund(m_nationAllianceApplication.fund, Guid.Empty);
		SaveToDB();
		ServerEvent.SendNationAllianceApplicationRejected(m_nationInst.GetClientPeers(Guid.Empty), nationAllianceApplicationId);
		ServerEvent.SendNationAllianceApplicationRejected(m_targetNationInst.GetClientPeers(m_myHero.id), nationAllianceApplicationId);
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		int nNationId = m_nationInst.nationId;
		int nTargetNationId = m_targetNationInst.nationId;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateNationWork(nNationId);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateNationWork(nTargetNationId));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateNationInstance_Fund(nNationId, m_nationInst.fund));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateNationAllianceApplication(m_nationAllianceApplication.id, 2, m_currentTime));
		dbWork.Schedule();
	}
}
