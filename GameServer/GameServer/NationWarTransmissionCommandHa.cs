using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class NationWarTransmissionCommandHandler : InGameCommandHandler<NationWarTransmissionCommandBody, NationWarTransmissionResponseBody>
{
	public const short kResult_NoNationWar = 101;

	public const short kResult_Dead = 102;

	public const short kResult_AlreadRidingCart = 103;

	public const short kResult_NotOccupationArea = 105;

	public const short kResult_NotEnoughtDia = 106;

	public const short kResult_LevelUnderflowed = 107;

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
		int m_nTargetAreaMonsterArrangeId = m_body.targetAreaMonsterArrangeId;
		Resource res = Resource.instance;
		NationWar nationWar = res.nationWar;
		NationWarMonsterArrange monsterArrange = nationWar.GetMonsterArrange(m_nTargetAreaMonsterArrangeId);
		if (monsterArrange == null)
		{
			throw new CommandHandleException(1, "몬스터배치가 존재하지 않습니다. m_nTargetAreaMonsterArrangeId = " + m_nTargetAreaMonsterArrangeId);
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
		int nHeroLevel = m_myHero.level;
		int nPvpMinHeroLevel = res.pvpMinHeroLevel;
		if (nHeroLevel < nPvpMinHeroLevel)
		{
			throw new CommandHandleException(107, "영웅의 레벨이 낮아 해당 국가전전장이동을 할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nPvpMinHeroLevel = " + nPvpMinHeroLevel);
		}
		int nNationId = m_myHero.nationId;
		NationWarMonsterInstance targetMonsterInst = nationWarInst.GetMonster(m_nTargetAreaMonsterArrangeId);
		if (targetMonsterInst != null)
		{
			if (targetMonsterInst.nationId != nNationId)
			{
				throw new CommandHandleException(105, "아군 점력지역이 아닙니다(1).");
			}
		}
		else if (nNationId == nationWarInst.defenseNation.id)
		{
			throw new CommandHandleException(105, "아군 점력지역이 아닙니다(2).");
		}
		DateTimeOffset m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		m_myHero.RefreshDailyNationWarFreeTransmissionCount(currentDate);
		m_myHero.RefreshDailyNationWarPaidTransmissionCount(currentDate);
		DateValuePair<int> dailyNationWarFreeTransmissionCount = m_myHero.dailyNationWarFreeTransmissionCount;
		DateValuePair<int> dailyNationWarPaidTransmissionCount = m_myHero.dailyNationWarPaidTransmissionCount;
		if (dailyNationWarFreeTransmissionCount.value >= nationWar.freeTransmissionCount)
		{
			int nPaidDia = nationWar.GetPaidTransmission(dailyNationWarPaidTransmissionCount.value + 1).requiredDia;
			if (m_myHero.dia < nPaidDia)
			{
				throw new CommandHandleException(106, "다이아가 부족합니다. myHeroDia = " + m_myHero.dia + ", nPaidDia = " + nPaidDia);
			}
		}
		Continent targetContinent = monsterArrange.continent;
		int nTargetNationId = nationWarInst.defenseNation.id;
		Vector3 targetPosition = monsterArrange.SelectTransmissionPosition();
		float fTargetRotationY = monsterArrange.SelectTransmissionRotationY();
		currentPlace.Exit(m_myHero, isLogOut: false, new ContinentEnterForNationWarTransmissionParam(targetContinent, nTargetNationId, targetPosition, fTargetRotationY, m_currentTime, nationWarInst.declaration.id, m_nTargetAreaMonsterArrangeId));
		NationWarTransmissionResponseBody resBody = new NationWarTransmissionResponseBody();
		resBody.targetContinentId = targetContinent.id;
		resBody.targetNationId = nTargetNationId;
		SendResponseOK(resBody);
	}
}
