using ClientCommon;

namespace GameServer;

public class NationWarReviveCommandHandler : InGameCommandHandler<NationWarReviveCommandBody, NationWarReviveResponseBody>
{
	public const short kResult_NotDead = 101;

	public const short kResult_NotJoinedNationWar = 102;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is NationContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		NationInstance nationInst = m_myHero.nationInst;
		NationWarInstance nationWarInst = nationInst.nationWarInst;
		if (nationWarInst == null)
		{
			throw new CommandHandleException(1, "자신의 국가가 국가전 진행중이 아닙니다.");
		}
		if (!m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은상태가 아닙니다.");
		}
		if (!nationWarInst.ContainsRealJoinNationHero(m_myHero))
		{
			throw new CommandHandleException(102, "영웅이 국가전에 참여하고 있지 않습니다.");
		}
		if (m_myHero.nationId == nationWarInst.defenseNation.id)
		{
			throw new CommandHandleException(1, "수비국가 영웅은 사용할 수 없습니다.");
		}
		if (currentPlace.nationId != nationWarInst.defenseNation.id)
		{
			throw new CommandHandleException(1, "수비국가에서만 사용할 수 있는 명령입니니다.");
		}
		if (!currentPlace.continent.isNationWarTarget)
		{
			throw new CommandHandleException(1, "현재 대륙이 국가전 대상대륙이 아닙니다.");
		}
		NationWar nationWar = Resource.instance.nationWar;
		NationWarRevivalPoint targetRevivalPoint = null;
		foreach (NationWarRevivalPoint revivalPoint in nationWar.revivalPoints.Values)
		{
			bool bIsTargetRevivalPoint = true;
			foreach (NationWarRevivalPointActivationCondition activationCondition in revivalPoint.activationConditions.Values)
			{
				NationWarMonsterArrange monsterArrange = activationCondition.monsterArrange;
				NationWarMonsterInstance monsterInst = nationWarInst.GetMonster(monsterArrange.id);
				if (monsterInst != null && (monsterArrange.type != 2 || monsterInst.nationId != nationWarInst.offenseNation.id))
				{
					bIsTargetRevivalPoint = false;
					break;
				}
			}
			if (bIsTargetRevivalPoint)
			{
				if (targetRevivalPoint == null)
				{
					targetRevivalPoint = revivalPoint;
				}
				else if (revivalPoint.priority < targetRevivalPoint.priority)
				{
					targetRevivalPoint = revivalPoint;
				}
			}
		}
		if (targetRevivalPoint == null)
		{
			throw new CommandHandleException(1, "국가전부활포인트가 존재하지 않습니다.");
		}
		Continent targetContinent = targetRevivalPoint.continent;
		Vector3 targetPosition = targetRevivalPoint.SelectPosition();
		float fTargetRotationY = targetRevivalPoint.SelectRotationY();
		currentPlace.Exit(m_myHero, isLogOut: false, new ContinentEnterForNationWarReviveParam(targetContinent, currentPlace.nationId, targetPosition, fTargetRotationY, DateTimeUtil.currentTime));
		NationWarReviveResponseBody resBody = new NationWarReviveResponseBody();
		resBody.revivalTargetContinentId = targetContinent.id;
		resBody.revivalTargetNationId = currentPlace.nationId;
		SendResponseOK(resBody);
	}
}
