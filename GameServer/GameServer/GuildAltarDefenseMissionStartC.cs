using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildAltarDefenseMissionStartCommandHandler : InGameCommandHandler<GuildAltarDefenseMissionStartCommandBody, GuildAltarDefenseMissionStartResponseBody>
{
	public const short kResult_MoralPointIsMax = 101;

	public const short kResult_SpellInjectionMissionPerforming = 102;

	public const short kResult_DefenseMissionPerforming = 103;

	public const short kResult_CoolTimeNotElapsed = 104;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTimeOffset.MinValue.Date;

	private GuildAltar m_altar;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		m_altar = Resource.instance.guildAltar;
		if (!(m_myHero.currentPlace is GuildTerritoryInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소가 길드영지가 아닙니다.");
		}
		if (m_myHero.guildAltarSpellInjectionMission != null)
		{
			throw new CommandHandleException(102, "길드제단 마력주입미션을 수행중입니다.");
		}
		if (m_myHero.guildAltarDefenseMission != null)
		{
			throw new CommandHandleException(103, "길드제단 수비미션을 수행중입니다.");
		}
		GuildTerritoryNpc npc = m_altar.npc;
		if (!npc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		m_myHero.RefreshGuildMoralPoint(m_currentDate);
		if (m_myHero.guildMoralPoint >= m_altar.dailyHeroMaxMoralPoint)
		{
			throw new CommandHandleException(101, "금일 채울수 있는 모럴포인트가 최대입니다.");
		}
		if (!m_myHero.IsGuildAltarDefenseMissionCoolTimeElapsed(m_currentTime))
		{
			throw new CommandHandleException(104, "쿨타임이 경과되지 않았습니다.");
		}
		GuildAltarMonsterInstance monsterInst = new GuildAltarMonsterInstance();
		monsterInst.Init(currentPlace, m_myHero);
		currentPlace.SpawnMonster(monsterInst, m_currentTime);
		m_myHero.StartGuildAltarDefenseMission(monsterInst, m_currentTime);
		SaveToDB();
		GuildAltarDefenseMissionStartResponseBody resBody = new GuildAltarDefenseMissionStartResponseBody();
		resBody.monsterInstanceId = monsterInst.instanceId;
		resBody.monsterPosition = monsterInst.position;
		resBody.remainingCoolTime = m_myHero.GetGuildAltarDefenseMissionRemainingCoolTime(m_currentTime);
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_GuildAltarDefenseStartTime(m_myHero.id, m_myHero.guildAltarDefenseMissionStartTime));
		dbWork.Schedule();
	}
}
