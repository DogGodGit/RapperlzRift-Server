using ClientCommon;

namespace GameServer;

public class NationWarNpcTransmissionCommandHandler : InGameCommandHandler<NationWarNpcTransmissionCommandBody, NationWarNpcTransmissionResponseBody>
{
	public const short kResult_NoNationWar = 101;

	public const short kResult_Dead = 102;

	public const short kResult_AlreadRidingCart = 103;

	public const short kResult_NotJoinedNationWar = 104;

	public const short kResult_UnableInteractionPositionWithNationWarNPC = 105;

	public const short kResult_DeactivatedNationWarNpc = 106;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is NationContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nNpcId = m_body.npcId;
		int nTransmissionExitNo = m_body.transmissionExitNo;
		NationWar nationWar = Resource.instance.nationWar;
		NationWarNpc nationWarNpc = nationWar.GetNpc(nNpcId);
		if (nationWarNpc == null)
		{
			throw new CommandHandleException(1, "NPCID가 유효하지 않습니다. nNpcId = " + nNpcId);
		}
		NationWarTransmissionExit transmissionExit = nationWarNpc.GetTransmissionExit(nTransmissionExitNo);
		if (transmissionExit == null)
		{
			throw new CommandHandleException(1, "전송출구번호가 유효하지 않습니다. nTransmissionExitNo = " + nTransmissionExitNo);
		}
		NationInstance nationInst = m_myHero.nationInst;
		NationWarInstance nationWarInst = nationInst.nationWarInst;
		if (nationWarInst == null)
		{
			throw new CommandHandleException(101, "자신의 국가가 국가전 진행중이 아닙니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(102, "영웅이 죽은 상태입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(103, "영웅이 카트에 탑승중입니다.");
		}
		if (!nationWarInst.ContainsRealJoinNationHero(m_myHero))
		{
			throw new CommandHandleException(104, "영웅이 국가전에 참여하고 있지 않습니다.");
		}
		if (m_myHero.nationId == nationWarInst.defenseNation.id)
		{
			throw new CommandHandleException(1, "수비국가 영웅은 사용할 수 없습니다.");
		}
		if (nationWarNpc.continentId != currentPlace.continent.id && currentPlace.nationId != nationWarInst.defenseNation.id)
		{
			throw new CommandHandleException(1, "현재 장소는 해당 NPC가 있는 장소가 아닙니다.");
		}
		if (nationWarNpc.continentId != currentPlace.continent.id)
		{
			throw new CommandHandleException(1, "국가전NPC와 다른 대륙에 있습니다.");
		}
		if (!nationWarNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(105, "국가전NPC와 상호작용할 수 있는 위치가 아닙니다.");
		}
		if (nNpcId != nationWarInst.activationNpcId)
		{
			throw new CommandHandleException(106, "해당 국가전NPC는 활성화 되지 않았습니다.");
		}
		Continent targetContinent = transmissionExit.continent;
		int nTargetNationId = nationWarInst.defenseNation.id;
		Vector3 targetPosition = transmissionExit.SelectPosition();
		float fTargetRotationY = transmissionExit.SelectRotationY();
		currentPlace.Exit(m_myHero, isLogOut: false, new ContinentEnterForNationWarNpcTransmissionParam(targetContinent, nTargetNationId, targetPosition, fTargetRotationY, DateTimeUtil.currentTime));
		NationWarNpcTransmissionResponseBody resBody = new NationWarNpcTransmissionResponseBody();
		resBody.targetContinentId = targetContinent.id;
		resBody.targetNationId = nTargetNationId;
		SendResponseOK(resBody);
	}
}
