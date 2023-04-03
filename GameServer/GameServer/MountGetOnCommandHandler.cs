using ClientCommon;

namespace GameServer;

public class MountGetOnCommandHandler : InGameCommandHandler<MountGetOnCommandBody, MountGetOnResponseBody>
{
	public const short kResult_Dead = 101;

	public const short kResult_BattleMode = 102;

	public const short kResult_AlreadyRidingMount = 103;

	protected override void HandleInGameCommand()
	{
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace == null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!currentPlace.mountRidingEnabled)
		{
			throw new CommandHandleException(1, "현재 장소에서는 탈것을 탈 수 없습니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은 상태입니다.");
		}
		if (m_myHero.isBattleMode)
		{
			throw new CommandHandleException(102, "영웅이 전투상태입니다.");
		}
		if (m_myHero.isRiding)
		{
			throw new CommandHandleException(103, "영웅이 탈것에 탑승 중 입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(1, "영웅이 카트에 탑승중입니다.");
		}
		HeroExclusiveAction currentExclusiveAction = m_myHero.currentExclusiveAction;
		if (currentExclusiveAction != 0)
		{
			throw new CommandHandleException(1, "영웅이 다른 행동중입니다. currentExclusiveAction = " + currentExclusiveAction);
		}
		if (m_myHero.isTransformMonster)
		{
			throw new CommandHandleException(1, "몬스터 변신중에는 사용할 수 없는 명령입니다.");
		}
		if (m_myHero.equippedMount == null)
		{
			throw new CommandHandleException(1, "현재 출전하고 있는 탈것이 없습니다.");
		}
		m_myHero.GetOnMount();
		SendResponseOK(null);
	}
}
