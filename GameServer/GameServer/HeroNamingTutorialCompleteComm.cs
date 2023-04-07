using System;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class HeroNamingTutorialCompleteCommandHandler : LobbyCommandHandler<HeroNamingTutorialCompleteCommandBody, HeroNamingTutorialCompleteResponseBody>
{
	public const short kResult_HeroNotExist = 101;

	public const short kResult_AlreadyCompleted = 102;

	private Guid m_heroId = Guid.Empty;

	protected override void HandleLobbyCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "바디가 null입니다.");
		}
		m_heroId = (Guid)m_body.heroId;
		if (m_heroId == Guid.Empty)
		{
			throw new CommandHandleException(1, "영웅ID가 유효하지 않습니다.");
		}
		SFRunnableStandaloneWork work = new SFRunnableStandaloneWork();
		work.runnable = new SFAction(Process);
		RunWork(work);
	}

	private void Process()
	{
		SqlConnection conn = null;
		SqlTransaction trans = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			trans = conn.BeginTransaction();
			DataRow drHero = GameDac.Hero_x(conn, trans, m_heroId);
			if (drHero == null)
			{
				throw new CommandHandleException(101, "해당 영웅이 존재하지 않습니다. id = " + m_heroId);
			}
			Guid accountId = (Guid)drHero["accountId"];
			bool bNamingTutorialCompleted = Convert.ToBoolean(drHero["namingTutorialCompleted"]);
			if (accountId != m_myAccount.id)
			{
				throw new CommandHandleException(1, "나의 영웅이 아닙니다. id = " + m_heroId);
			}
			if (bNamingTutorialCompleted)
			{
				throw new CommandHandleException(102, "이미 이름짓기튜토리얼을 완료했습니다. id = " + m_heroId);
			}
			if (GameDac.UpdateHero_CompleteNamingTutorial(conn, trans, m_heroId) != 0)
			{
				throw new CommandHandleException(1, "영웅 수정(이름짓기튜토리얼완료) 실패. id = " + m_heroId);
			}
			SFDBUtil.Commit(ref trans);
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Rollback(ref trans);
			SFDBUtil.Close(ref conn);
		}
	}

	protected override void OnWork_Success(SFWork work)
	{
		base.OnWork_Success(work);
		SendResponseOK(null);
	}
}
