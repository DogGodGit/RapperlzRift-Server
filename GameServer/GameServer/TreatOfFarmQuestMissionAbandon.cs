using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class TreatOfFarmQuestMissionAbandonCommandHandler : InGameCommandHandler<TreatOfFarmQuestMissionAbandonCommandBody, TreatOfFarmQuestMissionAbandonResponseBody>
{
	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		HeroTreatOfFarmQuest heroTreatOfFarmQuest = m_myHero.treatOfFarmQuest;
		if (heroTreatOfFarmQuest == null)
		{
			throw new CommandHandleException(1, "농장의위협 퀘스트가 존재하지 않습니다.");
		}
		HeroTreatOfFarmQuestMission heroTreatOfFarmQuestMission = heroTreatOfFarmQuest.currentMission;
		if (heroTreatOfFarmQuestMission == null)
		{
			throw new CommandHandleException(1, "농장의위협 미션이 존재하지 않습니다.");
		}
		TreatOfFarmQuestMonsterInstance monsterInst = heroTreatOfFarmQuestMission.targetMonsterInst;
		if (monsterInst != null)
		{
			Place monsterCurrentPlace = monsterInst.currentPlace;
			lock (monsterCurrentPlace.syncObject)
			{
				monsterCurrentPlace.RemoveMonster(monsterInst, bSendEvent: true);
			}
		}
		heroTreatOfFarmQuest.FailCurrentMission(m_currentTime, bSendEvent: false);
		SendResponseOK(null);
	}
}
