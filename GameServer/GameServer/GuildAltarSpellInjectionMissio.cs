using System;
using ClientCommon;

namespace GameServer;

public class GuildAltarSpellInjectionMissionCancelEventHandler : InGameEventHandler<CEBGuildAltarSpellInjectionMissionCancelEventBody>
{
	protected override void HandleInGameEvent()
	{
		if (m_myHero.currentPlace is GuildTerritoryInstance)
		{
			m_myHero.CancelGuildAltarSpellInjectionMission(bSendEventToMyself: false, bSendEventToOthers: true);
		}
	}
}
public class GuildAltarSpellInjectionMissionStartCommandHandler : InGameCommandHandler<GuildAltarSpellInjectionMissionStartCommandBody, GuildAltarSpellInjectionMissionStartResponseBody>
{
	public const short kResult_Dead = 101;

	public const short kResult_AlreadyRidingMount = 102;

	public const short kResult_OtherActionPerforming = 103;

	public const short kResult_MoralPointIsMax = 104;

	public const short kResult_SpellInjectionMissionPerforming = 105;

	public const short kResult_DefenseMissionPerforming = 106;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTimeOffset.MinValue.Date;

	private GuildAltar m_altar;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		m_altar = Resource.instance.guildAltar;
		if (!(m_myHero.currentPlace is GuildTerritoryInstance))
		{
			throw new CommandHandleException(1, "현재 장소가 길드영지가 아닙니다.");
		}
		if (m_myHero.guildAltarSpellInjectionMission != null)
		{
			throw new CommandHandleException(105, "길드제단 마력주입미션을 수행중입니다.");
		}
		if (m_myHero.guildAltarDefenseMission != null)
		{
			throw new CommandHandleException(106, "길드제단 수비미션을 수행중입니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은상태 입니다.");
		}
		if (m_myHero.isRiding)
		{
			throw new CommandHandleException(102, "영웅이 탈것을 타고있는 상태입니다.");
		}
		if (m_myHero.moving)
		{
			throw new CommandHandleException(1, "영웅이 이동중입니다.");
		}
		if (m_myHero.autoHunting)
		{
			throw new CommandHandleException(1, "영웅이 자동사냥중입니다.");
		}
		HeroExclusiveAction currentExclusiveAction = m_myHero.currentExclusiveAction;
		if (currentExclusiveAction != 0)
		{
			throw new CommandHandleException(103, "영웅이 다른 행동중입니다. currentExclusiveAction = " + currentExclusiveAction);
		}
		GuildTerritoryNpc npc = m_altar.npc;
		if (!npc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		m_myHero.RefreshGuildMoralPoint(m_currentDate);
		if (m_myHero.guildMoralPoint >= m_altar.dailyHeroMaxMoralPoint)
		{
			throw new CommandHandleException(104, "금일 채울수 있는 모럴포인트가 최대입니다.");
		}
		m_myHero.StartGuildAltarSpellInjectionMission();
		SendResponseOK(null);
	}
}
