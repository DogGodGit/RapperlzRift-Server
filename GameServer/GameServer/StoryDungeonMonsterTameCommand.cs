using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class StoryDungeonMonsterTameCommandHandler : InGameCommandHandler<StoryDungeonMonsterTameCommandBody, StoryDungeonMonsterTameResponseBody>
{
	public const short kResult_UnableTamingPosition = 101;

	public const short kResult_TamingMonster = 102;

	public const short kResult_Dead = 103;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is StoryDungeonInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		long lnMonsterInstanceId = m_body.monsterInstanceId;
		if (!(currentPlace.GetMonster(lnMonsterInstanceId) is StoryDungeonMonsterInstance tamingMonster))
		{
			throw new CommandHandleException(1, "테이밍몬스터ID가 유효하지 않습니다.");
		}
		if (tamingMonster.monsterType != 3)
		{
			throw new CommandHandleException(1, "해당몬스터가 테이밍몬스터가 아닙니다.");
		}
		if (tamingMonster.targetTamer != m_myHero)
		{
			throw new CommandHandleException(1, "목표몬스터의 테이머가 아닙니다.");
		}
		if (!tamingMonster.IsTamingEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(101, "테이밍을 할 수 없는 위치입니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(103, "영웅이 죽은 상태입니다.");
		}
		if (m_myHero.isMonsterTaming)
		{
			throw new CommandHandleException(102, "영웅이 몬스터테이밍 중입니다.");
		}
		m_myHero.TameMonster(tamingMonster);
		tamingMonster.tamer = m_myHero;
		StoryDungeon storyDungeon = currentPlace.storyDungeon;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		currentPlace.ChangeHeroPositionAndRotation(m_myHero, storyDungeon.tamingPosition, storyDungeon.tamingYRotation, bSendInterestTargetChangeEvent: true, currentTime);
		StoryDungeonMonsterTameResponseBody resBody = new StoryDungeonMonsterTameResponseBody();
		resBody.position = m_myHero.position;
		resBody.rotationY = m_myHero.rotationY;
		SendResponseOK(resBody);
		currentPlace.OnTameMonster(tamingMonster);
	}
}
