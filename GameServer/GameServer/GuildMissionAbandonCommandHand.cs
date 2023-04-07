using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class GuildMissionAbandonCommandHandler : InGameCommandHandler<GuildMissionAbandonCommandBody, GuildMissionAbandonResponseBody>
{
	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		HeroGuildMissionQuest guildMissionQuest = m_myHero.guildMissionQuest;
		if (guildMissionQuest == null)
		{
			throw new CommandHandleException(1, "퀘스트가 존재하지 않습니다.");
		}
		HeroGuildMissionQuestMission guildMission = guildMissionQuest.currentMission;
		if (guildMission == null)
		{
			throw new CommandHandleException(1, "미션이 존재하지 않습니다.");
		}
		if (guildMission.mission.type == GuildMissionType.Summon)
		{
			GuildMissionMonsterInstance monsterInst = guildMission.monsterInst;
			Place monsterCurrentPlace = monsterInst.currentPlace;
			lock (monsterCurrentPlace.syncObject)
			{
				monsterCurrentPlace.RemoveMonster(monsterInst, bSendEvent: true);
			}
		}
		guildMissionQuest.AbandonCurrentMission(m_currentTime);
		SendResponseOK(null);
	}
}
