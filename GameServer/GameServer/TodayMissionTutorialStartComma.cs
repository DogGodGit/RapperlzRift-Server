using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class TodayMissionTutorialStartCommandHandler : InGameCommandHandler<TodayMissionTutorialStartCommandBody, TodayMissionTutorialStartResponseBody>
{
	protected override void HandleInGameCommand()
	{
		if (m_myHero.todayMissionTutorialStarted)
		{
			throw new CommandHandleException(1, "오늘의미션 튜토리얼을 이미 시작했습니다.");
		}
		m_myHero.todayMissionTutorialStarted = true;
		SaveToDB();
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_TodayMissionTurialStarted(m_myHero.id));
		dbWork.Schedule();
	}
}
