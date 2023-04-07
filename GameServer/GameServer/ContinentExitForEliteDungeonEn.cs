using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class ContinentExitForEliteDungeonEnterCommandHandler : InGameCommandHandler<ContinentExitForEliteDungeonEnterCommandBody, ContinentExitForEliteDungeonEnterResponseBody>
{
	public const short kResult_Dead = 101;

	public const short kResult_AlreadyRidingCart = 102;

	public const short kResult_NotEnoughStamina = 103;

	public const short kResult_EnterCountOverflowed = 104;

	public const short kResult_NotEnoughLevel = 105;

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
		int nEliteMonsterMasterId = m_body.eliteMonsterMasterId;
		Resource res = Resource.instance;
		EliteMonsterMaster eliteMonsterMaster = res.GetEliteMonsterMaster(nEliteMonsterMasterId);
		if (eliteMonsterMaster == null)
		{
			throw new CommandHandleException(1, "정예몬스터마스터ID가 유효하지 않습니다. nEliteMonsterMasterId = " + nEliteMonsterMasterId);
		}
		int nEliteMonsterKillApplicationRequiredHeroLevel = res.eliteMonsterKillApplicationRequiredHeroLevel;
		if (m_myHero.level < nEliteMonsterKillApplicationRequiredHeroLevel)
		{
			throw new CommandHandleException(105, "영웅 레벨이 부족합니다. heroLevel = " + m_myHero.level + ", nEliteMonsterKillApplicationRequiredHeroLevel = " + nEliteMonsterKillApplicationRequiredHeroLevel);
		}
		if (m_myHero.nationId != currentPlace.nationId)
		{
			throw new CommandHandleException(1, "자신의 국가에서만 입장할 수 있습니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은상태 입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(102, "영웅이 카트에 탑승중입니다.");
		}
		EliteDungeon eliteDungeon = Resource.instance.eliteDungeon;
		if (m_myHero.stamina < eliteDungeon.requiredStamina)
		{
			throw new CommandHandleException(103, "스태미너가 부족합니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		if (m_myHero.GetEliteDungeonAvailableEnterCount(currentTime) <= 0)
		{
			throw new CommandHandleException(104, "입장횟수가 초과되었습니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new EliteDungeonEnterParam(eliteMonsterMaster, currentTime));
		SendResponseOK(null);
	}
}
