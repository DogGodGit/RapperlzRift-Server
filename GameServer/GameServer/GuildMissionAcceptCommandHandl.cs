using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class GuildMissionAcceptCommandHandler : InGameCommandHandler<GuildMissionAcceptCommandBody, GuildMissionAcceptResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_UnableInteractionPositionWithStartNPC = 102;

	public const short kResult_CurrentDateNotEqualToQuestStartDate = 103;

	private HeroGuildMissionQuestMission m_heroGuildMission;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		GuildMember myGuildMember = m_myHero.guildMember;
		if (myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어있지 않습니다.");
		}
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재장소에선 사용할 수 없는 명령입니다.");
		}
		GuildMissionQuest guildMissionQuest = Resource.instance.guildMissionQuest;
		Npc startNpc = guildMissionQuest.startNpc;
		if (!currentPlace.IsSame(startNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "시작 NPC가 있는 장소가 아닙니다.");
		}
		if (!startNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(102, "시작 NPC와 상호작용할 수 있는 위치가 아닙니다.");
		}
		HeroGuildMissionQuest heroGuildMissionQuest = m_myHero.guildMissionQuest;
		if (heroGuildMissionQuest == null)
		{
			throw new CommandHandleException(1, "퀘스트가 존재하지 않습니다.");
		}
		if (heroGuildMissionQuest.date != currentDate)
		{
			throw new CommandHandleException(103, "현재 날짜가 퀘스트시작날짜와 다릅니다.");
		}
		if (heroGuildMissionQuest.completed)
		{
			throw new CommandHandleException(1, "이미 퀘스트를 완료했습니다.");
		}
		if (heroGuildMissionQuest.currentMission != null)
		{
			throw new CommandHandleException(1, "진행중인 퀘스트가 존재합니다.");
		}
		GuildMission guildMission = guildMissionQuest.SelectMission();
		m_heroGuildMission = new HeroGuildMissionQuestMission(heroGuildMissionQuest, guildMission, myGuildMember.guild.id);
		if (guildMission.type == GuildMissionType.Summon)
		{
			GuildMissionMonsterInstance monsterInst = new GuildMissionMonsterInstance();
			monsterInst.Init(currentPlace, guildMission.targetSummonMonsterArrange, guildMission.SelectSummonMonsterPosition(m_myHero.position), guildMission.targetSummonMonsterKillLimitTime, m_myHero.id, m_myHero.name, m_heroGuildMission.id, m_currentTime);
			currentPlace.SpawnMonster(monsterInst, m_currentTime);
			m_heroGuildMission.monsterInst = monsterInst;
			m_heroGuildMission.spawnedMonsterContinentId = currentPlace.continent.id;
		}
		heroGuildMissionQuest.SetCurrentMission(m_heroGuildMission);
		SaveToDB();
		GuildMissionAcceptResponseBody resBody = new GuildMissionAcceptResponseBody();
		resBody.mission = m_heroGuildMission.ToPDHeroGuildMission();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroGuildMissionQuestMission(m_heroGuildMission.id, m_heroGuildMission.quest.id, m_heroGuildMission.guildId, m_heroGuildMission.mission.id, m_heroGuildMission.spawnedMonsterContinentId, m_currentTime));
		dbWork.Schedule();
	}
}
