using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class ContinentExitForUndergroundMazeEnterCommandHandler : InGameCommandHandler<ContinentExitForUndergroundMazeEnterCommandBody, ContinentExitForUndergroundMazeEnterResponseBody>
{
	public const short kResult_NotEnoughLevel = 103;

	public const short kResult_Dead = 104;

	public const short kResult_PlayTimeOverflowed = 106;

	public const short kResult_AlreadyConsigned = 107;

	public const short kResult_NotClearedMainQuest = 108;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nFloor = m_body.floor;
		UndergroundMaze undergroundMaze = Resource.instance.undergroundMaze;
		UndergroundMazeEntrance entrance = undergroundMaze.GetEntrance(nFloor);
		UndergroundMazeFloor floor = undergroundMaze.GetFloor(nFloor);
		if (floor == null)
		{
			throw new CommandHandleException(1, "해당 지하미로층이 존재하지 않습니다. nFloor = " + nFloor);
		}
		if (entrance == null)
		{
			throw new CommandHandleException(1, "해당 지하미로층입구가 존재하지 않습니다. nFloor = " + nFloor);
		}
		if (m_myHero.nationId != currentPlace.nationId)
		{
			throw new CommandHandleException(1, "자신의 국가에서만 입장할 수 있습니다.");
		}
		int nHeroLevel = m_myHero.level;
		if (undergroundMaze.requiredConditionType == 1)
		{
			int nRequiredHeroLevel = undergroundMaze.requiredHeroLevel;
			if (nHeroLevel < nRequiredHeroLevel)
			{
				throw new CommandHandleException(103, "영웅의 레벨이 낮아 지하미로에 입장할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroLevel = " + nRequiredHeroLevel);
			}
		}
		else if (!m_myHero.IsMainQuestCompleted(undergroundMaze.requiredMainQuestNo))
		{
			throw new CommandHandleException(108, "입장에 필요한 메인퀘스트를 클리어하지 않았습니다.");
		}
		int nEntranceRequiredHeroLevel = entrance.requiredHeroLevel;
		if (nHeroLevel < nEntranceRequiredHeroLevel)
		{
			throw new CommandHandleException(103, "영웅의 레벨이 낮아 해당 지하미로층에 입장할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nEntranceRequiredHeroLevel = " + nEntranceRequiredHeroLevel);
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(104, "영웅이 죽은상태 입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(1, "영웅이 카트에 탑승중입니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		if (m_myHero.GetRemainingTaskConsignmentStartCount(1, currentTime.Date) <= 0)
		{
			throw new CommandHandleException(107, "이미 위탁한 할일 입니다.");
		}
		m_myHero.RefreshUndergroundMazeDailyPlayTime(currentTime.Date);
		if (m_myHero.undergroundMazePlayTime >= (float)undergroundMaze.limitTime)
		{
			throw new CommandHandleException(106, "영웅의 플레이 시간이 초과되었습니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new UndergroundMazeEnterParam(floor, currentTime));
		SendResponseOK(null);
	}
}
