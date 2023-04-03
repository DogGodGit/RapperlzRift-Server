using ClientCommon;

namespace GameServer;

public class ContinentExitForArtifactRoomEnterCommandHandler : InGameCommandHandler<ContinentExitForArtifactRoomEnterCommandBody, ContinentExitForArtifactRoomEnterResponseBody>
{
	public const short kResult_NotEnoughHeroLevel = 101;

	public const short kResult_ClearedTopFloor = 102;

	public const short kResult_LevelUnderflowed = 103;

	public const short kResult_Sweeping = 104;

	public const short kResult_Dead = 105;

	public const short kResult_NotClearedMainQuest = 106;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_myHero.nationId != currentPlace.nationId)
		{
			throw new CommandHandleException(1, "자신의 국가에서만 입장할 수 있습니다.");
		}
		ArtifactRoom artifactRoom = Resource.instance.artifactRoom;
		if (artifactRoom.requiredConditionType == 1)
		{
			if (m_myHero.level < artifactRoom.requiredHeroLevel)
			{
				throw new CommandHandleException(101, "영웅레벨이 부족합니다.");
			}
		}
		else if (!m_myHero.IsMainQuestCompleted(artifactRoom.requiredMainQuestNo))
		{
			throw new CommandHandleException(106, "입장에 필요한 메인퀘스트를 클리어하지 않았습니다.");
		}
		int nCurrentFloor = m_myHero.artifactRoomCurrentFloor;
		if (nCurrentFloor > artifactRoom.topFloor)
		{
			throw new CommandHandleException(102, "이미 고대유물의방 최고층을 클리어 하였습니다.");
		}
		ArtifactRoomFloor floor = artifactRoom.GetFloor(nCurrentFloor);
		if (m_myHero.level < floor.requiredHeroLevel)
		{
			throw new CommandHandleException(103, "영웅의 레벨이 낮아 목표층에 입장할 수 없습니다.");
		}
		if (m_myHero.artifactRoomSweepStartTime.HasValue)
		{
			throw new CommandHandleException(104, "소탕중에는 입장할 수 없습니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(105, "영웅이 죽은상태 입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(1, "영웅이 카트에 탑승중입니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new ArtifactRoomEnterParam(DateTimeUtil.currentTime));
		SendResponseOK(null);
	}
}
