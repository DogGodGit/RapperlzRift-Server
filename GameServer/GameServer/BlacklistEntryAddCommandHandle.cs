using System;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class BlacklistEntryAddCommandHandler : InGameCommandHandler<BlacklistEntryAddCommandBody, BlacklistEntryAddResponseBody>
{
	public const short kResult_AlreadyExist = 101;

	public const short kResult_BlacklistEntryCountIsMax = 102;

	private Guid m_targetHeroId = Guid.Empty;

	private string m_sTargetName;

	private int m_nTargetNationId;

	private int m_nTargetJobId;

	private int m_nTargetLevel;

	private long m_lnTargetBattlePower;

	private bool m_bRemovedFromFriendList;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_targetHeroId = (Guid)m_body.targetHeroId;
		if (m_targetHeroId == Guid.Empty)
		{
			throw new CommandHandleException(1, "대상영웅ID가 유효하지 않습니다.");
		}
		if (m_targetHeroId == m_myHero.id)
		{
			throw new CommandHandleException(1, "자신을 블랙리스트에 추가할 수 없습니다.");
		}
		Hero target = null;
		lock (Global.syncObject)
		{
			target = Cache.instance.GetLoggedInHero(m_targetHeroId);
		}
		if (target != null)
		{
			m_sTargetName = target.name;
			m_nTargetNationId = target.nationId;
			m_nTargetJobId = target.jobId;
			m_nTargetLevel = target.level;
			m_lnTargetBattlePower = target.battlePower;
			Process();
		}
		else
		{
			SFRunnableStandaloneWork work = new SFRunnableStandaloneWork();
			work.runnable = new SFAction(GetHeroInfo);
			RunWork(work);
		}
	}

	private void GetHeroInfo()
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			DataRow drHero = GameDac.Hero_pk(conn, null, m_targetHeroId);
			if (drHero == null)
			{
				throw new CommandHandleException(1, "대상영웅이 존재하지 않습니다. m_targetHeroId = " + m_targetHeroId);
			}
			if (!Convert.ToBoolean(drHero["created"]))
			{
				throw new CommandHandleException(1, "대상영웅이 생성완료되지 않았습니다. m_targetHeroId = " + m_targetHeroId);
			}
			m_sTargetName = Convert.ToString(drHero["name"]);
			m_nTargetNationId = Convert.ToInt32(drHero["nationId"]);
			m_nTargetJobId = Convert.ToInt32(drHero["jobId"]);
			m_nTargetLevel = Convert.ToInt32(drHero["level"]);
			m_lnTargetBattlePower = Convert.ToInt64(drHero["battlePower"]);
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
		Process();
	}

	private void Process()
	{
		if (m_myHero.IsBlacklistEntry(m_targetHeroId))
		{
			throw new CommandHandleException(101, "대상영웅은 이미 블랙리스트에 존재합니다. m_targetHeroId = " + m_targetHeroId);
		}
		if (m_myHero.isBlacklistFull)
		{
			throw new CommandHandleException(102, "블랙리스트의 항목수가 최대입니다.");
		}
		BlacklistEntry entry = m_myHero.AddBlacklistEntry(m_targetHeroId, m_sTargetName, m_nTargetNationId, m_nTargetJobId, m_nTargetLevel, m_lnTargetBattlePower);
		m_bRemovedFromFriendList = m_myHero.RemoveFriend(m_targetHeroId);
		SaveToDB();
		BlacklistEntryAddResponseBody resBody = new BlacklistEntryAddResponseBody();
		resBody.newEntry = entry.ToPDBlacklistEntry();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddBlacklistEntry(m_myHero.id, m_targetHeroId));
		if (m_bRemovedFromFriendList)
		{
			dbWork.AddSqlCommand(GameDac.CSC_DeleteFriend(m_myHero.id, m_targetHeroId));
		}
		dbWork.Schedule();
	}
}
