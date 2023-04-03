using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class BountyHunterQuestAbandonCommandHandler : InGameCommandHandler<BountyHunterQuestAbandonCommandBody, BountyHunterQuestAbandonResponseBody>
{
	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		HeroBountyHunterQuest heroBountyHunterQuest = m_myHero.bountyHunterQuest;
		if (heroBountyHunterQuest == null)
		{
			throw new CommandHandleException(1, "현상금사냥꾼퀘스트가 존재하지 않습니다.");
		}
		m_myHero.bountyHunterQuest = null;
		SaveToDB(heroBountyHunterQuest);
		SendResponseOK(null);
	}

	private void SaveToDB(HeroBountyHunterQuest heroBountyHunterQuest)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateBountyHunterQuest_Status(heroBountyHunterQuest.id, 2, m_currentTime));
		dbWork.Schedule();
	}
}
