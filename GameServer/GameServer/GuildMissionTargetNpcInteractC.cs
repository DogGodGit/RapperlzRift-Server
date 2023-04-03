using System;
using ClientCommon;

namespace GameServer;

public class GuildMissionTargetNpcInteractCommandHandler : InGameCommandHandler<GuildMissionTargetNpcInteractCommandBody, GuildMissionTargetNpcInteractResponseBody>
{
	public const short kResult_UnableInteractionPositionWithTargetNPC = 101;

	private HeroGuildMissionQuestMission m_heroGuildMission;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재장소에선 사용할 수 없는 명령입니다.");
		}
		HeroGuildMissionQuest heroGuildMissionQuest = m_myHero.guildMissionQuest;
		if (heroGuildMissionQuest == null)
		{
			throw new CommandHandleException(1, "영웅길드미션퀘스트가 존재하지 않습니다.");
		}
		m_heroGuildMission = heroGuildMissionQuest.currentMission;
		if (m_heroGuildMission == null)
		{
			throw new CommandHandleException(1, "영웅길드미션이 존재하지 않습니다.");
		}
		if (m_heroGuildMission.mission.type != GuildMissionType.Find)
		{
			throw new CommandHandleException(101, "해당 미션은 NPC찾기가 아닙니다.");
		}
		Npc targetNpc = m_heroGuildMission.mission.targetNpc;
		if (!currentPlace.IsSame(targetNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "현재 장소에 없는 NPC입니다.");
		}
		if (!targetNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(101, "해당 NPC와 상호작용할 수 없는 거리입니다.");
		}
		heroGuildMissionQuest.ProgressCurrentMission(m_currentTime);
		SendResponseOK(null);
	}
}
