using ClientCommon;

namespace GameServer;

public class GroggyMonsterItemStealStartCommandHandler : InGameCommandHandler<GroggyMonsterItemStealStartCommandBody, GroggyMonsterItemStealStartResponseBody>
{
	public const short kResult_Dead = 101;

	public const short kResult_AlreadyRidingCart = 102;

	public const short kResult_UnableStealPosition = 103;

	protected override void HandleInGameCommand()
	{
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace == null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		long lnTargetMonsterInstanceId = m_body.targetMonsterInstanceId;
		MonsterInstance monsterInst = currentPlace.GetMonster(lnTargetMonsterInstanceId);
		if (monsterInst == null)
		{
			throw new CommandHandleException(1, "목표몬스터인스턴스ID가 유효하지 않습니다. lnTargetMonsterInstanceId = " + lnTargetMonsterInstanceId);
		}
		if (monsterInst.targetTamer != m_myHero)
		{
			throw new CommandHandleException(1, "목표몬스터의 테이머가 아닙니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은 상태입니다.");
		}
		if (m_myHero.isRiding)
		{
			throw new CommandHandleException(1, "영웅이 탈것을 타고있는 상태입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(102, "영웅이 카트에 탑승중입니다.");
		}
		if (m_myHero.moving)
		{
			throw new CommandHandleException(1, "영웅이 이동중입니다.");
		}
		if (m_myHero.autoHunting)
		{
			throw new CommandHandleException(1, "영웅이 자동사냥중입니다.");
		}
		HeroExclusiveAction currentExclusiveAction = m_myHero.currentExclusiveAction;
		if (currentExclusiveAction != 0)
		{
			throw new CommandHandleException(1, "영웅이 다른 행동중입니다. currentExclusiveAction = " + currentExclusiveAction);
		}
		if (!monsterInst.IsStealEnabledPosition(m_myHero.position))
		{
			throw new CommandHandleException(103, "훔치기를 할 수 없는 위치입니다.");
		}
		m_myHero.StartGroggyMonsterItemSteal(monsterInst);
		SendResponseOK(null);
	}
}
