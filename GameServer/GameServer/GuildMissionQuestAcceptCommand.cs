using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class GuildMissionQuestAcceptCommandHandler : InGameCommandHandler<GuildMissionQuestAcceptCommandBody, GuildMissionQuestAcceptResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_UnableInteractionPositionWithStartNPC = 102;

	public const short kResult_AlreadyConsigned = 103;

	private HeroGuildMissionQuest m_heroGuildMissionQuest;

	private HeroGuildMissionQuestMission m_heroGuildMissionQuestMission;

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
		if (m_myHero.GetRemainingTaskConsignmentStartCount(4, currentDate) <= 0)
		{
			throw new CommandHandleException(103, "이미 위탁을 한 할일입니다.");
		}
		m_heroGuildMissionQuest = m_myHero.guildMissionQuest;
		if (m_heroGuildMissionQuest != null && m_heroGuildMissionQuest.date == currentDate)
		{
			throw new CommandHandleException(1, "이미 진행중인 퀘스트가 존재합니다.");
		}
		m_heroGuildMissionQuest = new HeroGuildMissionQuest(m_myHero, myGuildMember.guild.id, m_currentTime.Date);
		GuildMission guildMission = guildMissionQuest.SelectMission();
		m_heroGuildMissionQuestMission = new HeroGuildMissionQuestMission(m_heroGuildMissionQuest, guildMission, myGuildMember.guild.id);
		if (guildMission.type == GuildMissionType.Summon)
		{
			GuildMissionMonsterInstance monsterInst = new GuildMissionMonsterInstance();
			monsterInst.Init(currentPlace, guildMission.targetSummonMonsterArrange, guildMission.SelectSummonMonsterPosition(m_myHero.position), guildMission.targetSummonMonsterKillLimitTime, m_myHero.id, m_myHero.name, m_heroGuildMissionQuestMission.id, m_currentTime);
			currentPlace.SpawnMonster(monsterInst, m_currentTime);
			m_heroGuildMissionQuestMission.monsterInst = monsterInst;
			m_heroGuildMissionQuestMission.spawnedMonsterContinentId = currentPlace.continent.id;
		}
		m_heroGuildMissionQuest.SetCurrentMission(m_heroGuildMissionQuestMission);
		m_myHero.guildMissionQuest = m_heroGuildMissionQuest;
		SaveToDB();
		GuildMissionQuestAcceptResponseBody resBody = new GuildMissionQuestAcceptResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.quest = m_heroGuildMissionQuest.ToPDHeroGuildMissionQuest();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroGuildMissionQuest(m_heroGuildMissionQuest.id, m_heroGuildMissionQuest.hero.id, m_heroGuildMissionQuest.guildId, m_currentTime));
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroGuildMissionQuestMission(m_heroGuildMissionQuestMission.id, m_heroGuildMissionQuestMission.quest.id, m_heroGuildMissionQuestMission.guildId, m_heroGuildMissionQuestMission.mission.id, m_heroGuildMissionQuestMission.spawnedMonsterContinentId, m_currentTime));
		dbWork.Schedule();
	}
}
