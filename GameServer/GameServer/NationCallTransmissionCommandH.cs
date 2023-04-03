using System;
using ClientCommon;

namespace GameServer;

public class NationCallTransmissionCommandHandler : InGameCommandHandler<NationCallTransmissionCommandBody, NationCallTransmissionResponseBody>
{
	public const short kResult_Dead = 101;

	public const short kResult_AlreadyCartRiding = 102;

	public const short kResult_NotEnoughHeroLevel = 103;

	public const short kResult_NotExistNationCall = 104;

	public const short kResult_NationWar = 105;

	public const short KResult_NotEnoughHeroLevelForEnter = 106;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		long lnCallId = m_body.callId;
		if (lnCallId <= 0)
		{
			throw new CommandHandleException(1, "소집ID가 유효하지 않습니다. lnCallId = " + lnCallId);
		}
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "장소가 유효하지 않습니다.");
		}
		NationInstance nationInst = m_myHero.nationInst;
		if (nationInst.nationWarInst != null)
		{
			throw new CommandHandleException(105, "국가전이 진행중입니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은 상태입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(102, "영웅이 카트에 탑승중입니다.");
		}
		if (m_myHero.level < Resource.instance.pvpMinHeroLevel)
		{
			throw new CommandHandleException(103, "영웅의 레벨이 부족합니다.");
		}
		NationCall nationCall = nationInst.GetNationCall(lnCallId);
		if (nationCall == null)
		{
			throw new CommandHandleException(104, "국가소집이 존재하지 않습니다.");
		}
		if (nationCall.callerId == m_myHero.id)
		{
			throw new CommandHandleException(1, "소집자는 이용할 수 없는 명령입니다.");
		}
		if (nationCall.continent.requiredHeroLevel > m_myHero.level)
		{
			throw new CommandHandleException(106, "대상 대륙에 입장하기 위한 영웅레벨이 부족합니다.");
		}
		Continent targetContinent = nationCall.continent;
		int nTargetNationId = nationCall.nationId;
		Vector3 targetPosition = nationCall.SelectPosition();
		float fTargetRotationY = nationCall.rotationY;
		currentPlace.Exit(m_myHero, isLogOut: false, new ContinentEnterForNationCallTransmissionParam(targetContinent, nTargetNationId, targetPosition, fTargetRotationY, m_currentTime));
		NationCallTransmissionResponseBody resBody = new NationCallTransmissionResponseBody();
		resBody.targetContinentId = targetContinent.id;
		resBody.targetNationId = nTargetNationId;
		SendResponseOK(resBody);
	}
}
