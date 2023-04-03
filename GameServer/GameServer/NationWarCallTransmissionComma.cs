using System;
using ClientCommon;

namespace GameServer;

public class NationWarCallTransmissionCommandHandler : InGameCommandHandler<NationWarCallTransmissionCommandBody, NationWarCallTransmissionResponseBody>
{
	public const short kResult_NoNationWar = 101;

	public const short kResult_Dead = 102;

	public const short kResult_AlreadRidingCart = 103;

	public const short kResult_LevelUnderflowed = 104;

	public const short kResult_OverflowedLifeTime = 105;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		NationInstance nationInst = m_myHero.nationInst;
		NationWarInstance nationWarInst = nationInst.nationWarInst;
		if (nationWarInst == null)
		{
			throw new CommandHandleException(101, "자신의 국가가 국가전 진행중이 아닙니다.");
		}
		Resource res = Resource.instance;
		_ = Cache.instance;
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(102, "영웅이 죽은상태 입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(103, "영웅이 카트에 탑승중입니다.");
		}
		int nHeroLevel = m_myHero.level;
		int nPvpMinHeroLevel = res.pvpMinHeroLevel;
		if (nHeroLevel < nPvpMinHeroLevel)
		{
			throw new CommandHandleException(104, "영웅의 레벨이 낮아 해당 국가전전장이동을 할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nPvpMinHeroLevel = " + nPvpMinHeroLevel);
		}
		NationWarCall nationWarCall = m_myHero.nationInst.nationWarCall;
		if (nationWarCall == null)
		{
			throw new CommandHandleException(1, "국가전소집을 사용하지 않았습니다.");
		}
		if (nationWarCall.callerId == m_myHero.id)
		{
			throw new CommandHandleException(1, "소집자는 이용할 수 없습니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		float fElapsedTime = DateTimeUtil.GetTimeSpanSeconds(nationWarCall.regTime, currentTime);
		int nLifeTime = Resource.instance.nationWar.nationCallLifetime;
		if (fElapsedTime >= (float)nLifeTime)
		{
			throw new CommandHandleException(105, "국가전소집이동 사용가능 시간이 초과되었습니다.");
		}
		Continent targetContinent = nationWarCall.continent;
		int nTargetNationId = nationWarCall.nationId;
		Vector3 targetPosition = nationWarCall.SelectPosition();
		float fTargetRotationY = nationWarCall.rotationY;
		currentPlace.Exit(m_myHero, isLogOut: false, new ContinentEnterForNationWarCallTransmissionParam(targetContinent, nTargetNationId, targetPosition, fTargetRotationY, currentTime));
		NationWarCallTransmissionResponseBody resBody = new NationWarCallTransmissionResponseBody();
		resBody.targetContinentId = targetContinent.id;
		resBody.targetNationId = nTargetNationId;
		SendResponseOK(resBody);
	}
}
