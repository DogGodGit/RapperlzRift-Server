using ClientCommon;

namespace GameServer;

public class RuinsReclaimRewardObjectInteractionStartCommandHandler : InGameCommandHandler<RuinsReclaimRewardObjectInteractionStartCommandBody, RuinsReclaimRewardObjectInteractionStartResponseBody>
{
	public const short kResult_NotStatusPlaying = 101;

	public const short kResult_NotInteractionStep = 102;

	public const short kResult_Interaction = 103;

	public const short kResult_Dead = 104;

	public const short kResult_UnableInteractionPositionWithObject = 105;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is RuinsReclaimInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		long lnInstanceId = m_body.instanceId;
		if (currentPlace.status != 2)
		{
			throw new CommandHandleException(101, "현재 상태에서 사용할 수 없는 명령입니다.");
		}
		RuinsReclaim ruinsReclaim = currentPlace.ruinsReclaim;
		RuinsReclaimStep step = ruinsReclaim.GetStep(currentPlace.stepNo);
		if (step.type != 2)
		{
			throw new CommandHandleException(102, "현재 상호작용단계가 아닙니다.");
		}
		RuinsReclaimRewardObjectInstance objectInst = currentPlace.GetRewardObject(lnInstanceId);
		if (objectInst == null)
		{
			throw new CommandHandleException(1, "오브젝트 인스턴스ID가 유효하지 않습니다. lnInstanceId = " + lnInstanceId);
		}
		if (objectInst.interactionHero != null)
		{
			throw new CommandHandleException(103, "오브젝트가 상호작용중입니다");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(104, "영웅이 죽은 상태입니다.");
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
		if (!objectInst.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(105, "해당 오브젝트와 상호작용할 수없는 위치입니다.");
		}
		m_myHero.StartRuinsReclaimObjectInteraction(objectInst);
		SendResponseOK(null);
	}
}
