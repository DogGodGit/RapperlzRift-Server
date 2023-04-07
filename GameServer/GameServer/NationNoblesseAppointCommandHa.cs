using System;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class NationNoblesseAppointCommandHandler : InGameCommandHandler<NationNoblesseAppointCommandBody, NationNoblesseAppointResponseBody>
{
	public const short kResult_NotNoblesseAppointmentAuthority = 101;

	public const short kResult_AppointedTargetNoblesse = 102;

	public const short kResult_IsNoblesseByTargetHero = 103;

	public const short kResult_TodayTargetNoblesseAppoint = 104;

	public const short kResult_HeroNotExist = 105;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private NationNoblesse m_heroNationNoblesse;

	private int m_nTargetNoblesseId;

	private Guid m_targetHeroId = Guid.Empty;

	private NationInstance m_nationInst;

	private DataRow m_drHero;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null 입니다.");
		}
		m_nTargetNoblesseId = m_body.targetNoblesseId;
		m_targetHeroId = (Guid)m_body.targetHeroId;
		Resource res = Resource.instance;
		if (res.GetNationNoblesse(m_nTargetNoblesseId) == null)
		{
			throw new CommandHandleException(1, "목표관직ID가 유효하지 않습니다. m_nTargetNoblesseId = " + m_nTargetNoblesseId);
		}
		if (m_targetHeroId == m_myHero.id)
		{
			throw new CommandHandleException(1, "영웅ID가 자기자신 입니다.");
		}
		m_heroNationNoblesse = m_myHero.nationNoblesse;
		if (m_heroNationNoblesse == null)
		{
			throw new CommandHandleException(101, "영웅이 관직에 임명되지 않았습니다.");
		}
		if (!m_heroNationNoblesse.ContaninsAppointmentAuthority(m_nTargetNoblesseId))
		{
			throw new CommandHandleException(101, "영웅이 목표관직에 대한 임명권한이 없습니다.");
		}
		m_nationInst = m_myHero.nationInst;
		NationNoblesseInstance noblesseInst = m_nationInst.GetNoblesseInstanceByNoblesseId(m_nTargetNoblesseId);
		if (noblesseInst.heroId != Guid.Empty)
		{
			throw new CommandHandleException(102, "이미 임명된 관직입니다.");
		}
		m_currentTime = DateTimeUtil.currentTime;
		if (noblesseInst.appointmentDate == m_currentTime.Date)
		{
			throw new CommandHandleException(104, "이미 오늘 목표 관직임명을 하였습니다.");
		}
		if (m_nationInst.GetNoblesseInstanceByHeroId(m_targetHeroId) != null)
		{
			throw new CommandHandleException(103, "이미 목표영웅이 관직을 가지고 있습니다.");
		}
		Hero targetHero = Cache.instance.GetLoggedInHero(m_targetHeroId);
		if (targetHero == null)
		{
			SFRunnableStandaloneWork work = new SFRunnableStandaloneWork();
			work.runnable = new SFAction<Guid>(ProcessGetLoggedOutHeroInfo, m_targetHeroId);
			RunWork(work);
		}
		else
		{
			AppointLoggedInHero(targetHero);
		}
	}

	private void ProcessGetLoggedOutHeroInfo(Guid targetHeroId)
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			m_drHero = GameDac.Hero(conn, null, targetHeroId);
			if (m_drHero == null)
			{
				throw new CommandHandleException(105, "해당 영웅이 존재하지 않습니다. targetHeroId = " + targetHeroId);
			}
			if (!Convert.ToBoolean(m_drHero["created"]))
			{
				throw new CommandHandleException(1, "영웅 생성이 완료되지 않았습니다. targetHeroId = " + targetHeroId);
			}
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	protected override void OnWork_Success(SFWork work)
	{
		base.OnWork_Success(work);
		Hero targetHero = Cache.instance.GetLoggedInHero(m_targetHeroId);
		if (targetHero != null)
		{
			AppointLoggedInHero(targetHero);
			return;
		}
		int nNationId = Convert.ToInt32(m_drHero["nationId"]);
		if (nNationId != m_nationInst.nationId)
		{
			throw new CommandHandleException(1, "목표 영웅의 국가가 임명할 관직 국가와 다릅니다.");
		}
		Guid heroId = SFDBUtil.ToGuid(m_drHero["heroId"]);
		string sName = Convert.ToString(m_drHero["name"]);
		int nJobId = Convert.ToInt32(m_drHero["jobId"]);
		Finish(heroId, sName, nJobId);
	}

	private void AppointLoggedInHero(Hero targetHero)
	{
		if (targetHero.nationId != m_nationInst.nationId)
		{
			throw new CommandHandleException(1, "목표 영웅의 국가가 임명할 관직 국가와 다릅니다.");
		}
		HeroSynchronizer.Exec(targetHero, new SFAction<Hero>(AppointLoggedInHeroWithLock, targetHero));
	}

	private void AppointLoggedInHeroWithLock(Hero targetHero)
	{
		targetHero.SetNationNoblesse(Resource.instance.GetNationNoblesse(m_nTargetNoblesseId));
		Finish(targetHero.id, targetHero.name, targetHero.jobId);
	}

	private void Finish(Guid targetHeroId, string sName, int nJobId)
	{
		m_nationInst.AppointNoblesse(m_nTargetNoblesseId, targetHeroId, sName, nJobId, m_currentTime.Date);
		SaveToDB(targetHeroId);
		SaveToDB_Log(targetHeroId);
		SendResponseOK(null);
	}

	private void SaveToDB(Guid targetHeroId)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateNationWork(m_nationInst.nationId);
		dbWork.AddSqlCommand(GameDac.CSC_AddNationIncumbentNoblesse(m_nationInst.nationId, m_nTargetNoblesseId, targetHeroId));
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateNationNoblesseAppointment(m_nationInst.nationId, m_nTargetNoblesseId, m_currentTime));
		dbWork.Schedule();
	}

	private void SaveToDB_Log(Guid targetHeroId)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddNationNoblesseApponitmentLog(Guid.NewGuid(), m_nationInst.nationId, m_myHero.id, m_heroNationNoblesse.id, targetHeroId, m_nTargetNoblesseId, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
