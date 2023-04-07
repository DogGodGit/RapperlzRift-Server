using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class NationTransmissionCommandHandler : InGameCommandHandler<NationTransmissionCommandBody, NationTransmissionResponseBody>
{
	public const short kResult_OutOfTransmissionRange = 101;

	public const short kResult_LevelUnderflowed = 102;

	public const short kResult_Dead = 103;

	public const short kResult_ProgressingNationWarByTargetNation = 104;

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
		int nNationId = m_body.nationId;
		Npc npc = Resource.instance.GetNpc(nNpcId);
		if (npc == null)
		{
			throw new CommandHandleException(1, "NPC가 존재하지 않습니다. nNpcId = " + nNpcId);
		}
		if (npc.type != 3)
		{
			throw new CommandHandleException(1, "해당 NPC가 국가전송타입의 NPC가 아닙니다.");
		}
		Nation nation = Resource.instance.GetNation(nNationId);
		if (nation == null)
		{
			throw new CommandHandleException(1, "해당 국가가 존재하지 않습니다. nNationId = " + nNationId);
		}
		if (currentPlace.continent.id != npc.continent.id)
		{
			throw new CommandHandleException(1, "입장할 수 없는 장소입니다.");
		}
		if (nNationId == currentPlace.nationId)
		{
			throw new CommandHandleException(1, "이미 해당 국가에 있습니다.");
		}
		if (!npc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(101, "입장할 수 없는 위치입니다.");
		}
		NationWarInstance nationWarInst = Cache.instance.GetNationInstance(nNationId).nationWarInst;
		if (nationWarInst != null && nationWarInst.defenseNation.id == nNationId && m_myHero.nationId != nationWarInst.offenseNation.id && m_myHero.nationId != nationWarInst.defenseNation.id)
		{
			throw new CommandHandleException(104, "목표 국가가 국가전 진행중이므로 입장할 수 없습니다.");
		}
		int nHeroLevel = m_myHero.level;
		int nRequiredHeroLevel = Resource.instance.nationTransmissionRequiredHeroLevel;
		if (nHeroLevel < nRequiredHeroLevel)
		{
			throw new CommandHandleException(102, "영웅의 레벨이 낮아 국가전송을 할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroLevel = " + nRequiredHeroLevel);
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(103, "영웅이 죽은상태 입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(1, "영웅이 카트에 탑승중입니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new ContinentEnterForNationTransmissionParam(nation, currentPlace.continent));
		SendResponseOK(null);
	}
}
