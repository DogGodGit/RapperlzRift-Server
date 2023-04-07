using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class ContinentTransmissionCommandHandler : InGameCommandHandler<ContinentTransmissionCommandBody, ContinentTransmissionResponseBody>
{
	public const short kResult_OutOfTransmissionRange = 101;

	public const short kResult_LevelUnderflowed = 102;

	public const short kResult_Dead = 103;

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
		int nNpcId = m_body.npcId;
		int nExitNo = m_body.exitNo;
		Npc npc = Resource.instance.GetNpc(nNpcId);
		if (npc == null)
		{
			throw new CommandHandleException(1, "NPC가 존재하지 않습니다. nNpcId = " + nNpcId);
		}
		if (npc.type != 2)
		{
			throw new CommandHandleException(1, "해당 NPC가 대륙전송타입의 NPC가 아닙니다.");
		}
		ContinentTransmissionExit continentTransmissionExit = npc.GetContinentTransmissionExit(nExitNo);
		if (continentTransmissionExit == null)
		{
			throw new CommandHandleException(1, "출구가 존재하지 않습니다. nNpcId = " + nNpcId + ", nExitNo = " + nExitNo);
		}
		if (!currentPlace.IsSame(npc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "NPC가 있는 장소가 아닙니다.");
		}
		if (!npc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(101, "입장할 수 없는 위치입니다.");
		}
		int nHeroLevel = m_myHero.level;
		int nRequiredHeroLevel = continentTransmissionExit.continent.requiredHeroLevel;
		if (nHeroLevel < nRequiredHeroLevel)
		{
			throw new CommandHandleException(102, "영웅의 레벨이 낮아 해당 대륙전송을 할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroLevel = " + nRequiredHeroLevel);
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(103, "영웅이 죽은상태 입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(1, "영웅이 카트에 탑승중입니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new ContinentEnterForContinentTransmissionParam(continentTransmissionExit));
		SendResponseOK(null);
	}
}
