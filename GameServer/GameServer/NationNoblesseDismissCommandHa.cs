using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class NationNoblesseDismissCommandHandler : InGameCommandHandler<NationNoblesseDismissCommandBody, NationNoblesseDismissResponseBody>
{
	public const short kResult_NotNoblesseAppointmentAuthority = 101;

	public const short kResult_NotAppointTargetNoblesse = 102;

	private NationInstance m_nationInst;

	private int m_nTargetNobalessId;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null 입니다.");
		}
		m_nTargetNobalessId = m_body.targetNoblesseId;
		Resource res = Resource.instance;
		if (res.GetNationNoblesse(m_nTargetNobalessId) == null)
		{
			throw new CommandHandleException(1, "목표관직ID가 유효하지 않습니다. m_nTargetNobalessId = " + m_nTargetNobalessId);
		}
		NationNoblesse heroNationNoblesse = m_myHero.nationNoblesse;
		if (heroNationNoblesse == null)
		{
			throw new CommandHandleException(101, "영웅이 관직에 임명되지 않았습니다.");
		}
		if (!heroNationNoblesse.ContaninsAppointmentAuthority(m_nTargetNobalessId))
		{
			throw new CommandHandleException(101, "영웅이 목표관직에 대한 임명권한이 없습니다.");
		}
		m_nationInst = m_myHero.nationInst;
		NationNoblesseInstance noblesseInst = m_nationInst.GetNoblesseInstanceByNoblesseId(m_nTargetNobalessId);
		if (noblesseInst.heroId == Guid.Empty)
		{
			throw new CommandHandleException(102, "아직 임명되지 않은 관직입니다.");
		}
		Hero targetHero = Cache.instance.GetLoggedInHero(noblesseInst.heroId);
		if (targetHero != null)
		{
			HeroSynchronizer.Exec(targetHero, new SFAction<Hero>(ProcessForLoggedInHero, targetHero));
		}
		else
		{
			Process();
		}
	}

	private void ProcessForLoggedInHero(Hero targetHero)
	{
		targetHero.SetNationNoblesse(null);
		Process();
	}

	private void Process()
	{
		m_nationInst.DismissNoblesse(m_nTargetNobalessId);
		SaveToDB();
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateNationWork(m_nationInst.nationId);
		dbWork.AddSqlCommand(GameDac.CSC_DeleteNationIncumbentNoblesse(m_nationInst.nationId, m_nTargetNobalessId));
		dbWork.Schedule();
	}
}
