using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ArtifactRoomInitCommandHandler : InGameCommandHandler<ArtifactRoomInitCommandBody, ArtifactRoomInitResponseBody>
{
	public const short kResult_Sweeping = 101;

	public const short kResult_InitCountOverflowed = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_myHero.artifactRoomSweepStartTime.HasValue)
		{
			throw new CommandHandleException(101, "소탕중에는 초기화할 수 없습니다.");
		}
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		m_myHero.RefreshAartifactRoomDailyInitCount(currentDate);
		if (m_myHero.artifactRoomCurrentFloor <= 1)
		{
			throw new CommandHandleException(1, "이미 고대유물의방층이 초기상태 입니다.");
		}
		if (m_myHero.GetArtifactRoomAvailableInitCount(currentDate) <= 0)
		{
			throw new CommandHandleException(102, "초기화횟수가 초과되었습니다.");
		}
		DateValuePair<int> artifactRoomDailyInitCount = m_myHero.artifactRoomDailyInitCount;
		artifactRoomDailyInitCount.value++;
		m_myHero.InitArtifactRoomFloor();
		SaveToDB(artifactRoomDailyInitCount.date, artifactRoomDailyInitCount.value);
		ArtifactRoomInitResponseBody resBody = new ArtifactRoomInitResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.currentFloor = m_myHero.artifactRoomCurrentFloor;
		resBody.dailyInitCount = artifactRoomDailyInitCount.value;
		SendResponseOK(resBody);
	}

	private void SaveToDB(DateTime date, int nCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ArtifactRoomInit(m_myHero.id, date, nCount, m_myHero.artifactRoomCurrentFloor));
		dbWork.Schedule();
		SaveToDB_Log();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddArtifactRoomInitLog(Guid.NewGuid(), m_myHero.id, m_myHero.artifactRoomBestFloor, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
