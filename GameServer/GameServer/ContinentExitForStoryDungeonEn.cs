using System;
using ClientCommon;

namespace GameServer;

public class ContinentExitForStoryDungeonEnterCommandHandler : InGameCommandHandler<ContinentExitForStoryDungeonEnterCommandBody, ContinentExitForStoryDungeonEnterResponseBody>
{
	public const short kResult_LevelUnderflowed = 101;

	public const short kResult_LevelOverflowed = 102;

	public const short kResult_Dead = 103;

	public const short kResult_NotEnoughStamina = 105;

	public const short kResult_EnterCountOverflowed = 106;

	public const short kResult_NotClearedMainQuest = 107;

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
		int nDungeonNo = m_body.dungeonNo;
		int nDifficulty = m_body.difficulty;
		StoryDungeon storyDungeon = Resource.instance.GetStoryDungeon(nDungeonNo);
		if (storyDungeon == null)
		{
			throw new CommandHandleException(1, "해당 챕터번호는 존재하지 않습니다. nDungeonNo = " + nDungeonNo);
		}
		StoryDungeonDifficulty difficulty = storyDungeon.GetDifficulty(nDifficulty);
		if (difficulty == null)
		{
			throw new CommandHandleException(1, "해당 난이도는 존재하지 않습니다. nDifficulty = " + nDifficulty);
		}
		if (m_myHero.nationId != currentPlace.nationId)
		{
			throw new CommandHandleException(1, "자신의 국가에서만 입장할 수 있습니다.");
		}
		if (storyDungeon.requiredConditionType == 2)
		{
			int nHeroLevel = m_myHero.level;
			int nRequiredHeroMinLevel = storyDungeon.requiredHeroMinLevel;
			int nRequiredHeroMaxLevel = storyDungeon.requiredHeroMaxLevel;
			if (nHeroLevel < nRequiredHeroMinLevel)
			{
				throw new CommandHandleException(101, "영웅의 레벨이 낮아 해당 던전에 입장할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroMinLevel = " + nRequiredHeroMinLevel);
			}
			if (nHeroLevel > nRequiredHeroMaxLevel)
			{
				throw new CommandHandleException(102, "영웅의 레벨이 높아 해당 던전에 입장할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroMaxLevel = " + nRequiredHeroMaxLevel);
			}
		}
		else if (!m_myHero.IsMainQuestCompleted(storyDungeon.requiredMainQuestNo))
		{
			throw new CommandHandleException(107, "입장에 필요한 메인퀘스트를 클리어하지 않았습니다.");
		}
		int nHeroClearMaxDifficulty = m_myHero.GetStoryDungeonClearMaxDifficulty(nDungeonNo);
		if (nDifficulty > nHeroClearMaxDifficulty + 1)
		{
			throw new CommandHandleException(1, "영웅이 해당 난이도 입장에 필요한 조건을 만족하지 못했습니다. nDifficulty = " + nDifficulty + ", nHeroClearMaxDifficulty = " + nHeroClearMaxDifficulty);
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(103, "영웅이 죽은상태 입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(1, "영웅이 카트에 탑승중입니다.");
		}
		if (m_myHero.stamina < storyDungeon.requiredStamina)
		{
			throw new CommandHandleException(105, "스태미너가 부족합니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		if (m_myHero.GetStoryDungeonAvailableEnterCount(nDungeonNo, currentDate) <= 0)
		{
			throw new CommandHandleException(106, "입장횟수가 초과되었습니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new StoryDungeonEnterParam(difficulty, currentTime));
		SendResponseOK(null);
	}
}
