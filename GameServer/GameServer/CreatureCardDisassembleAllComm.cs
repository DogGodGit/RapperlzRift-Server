using System;
using System.Collections.Generic;
using System.Linq;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class CreatureCardDisassembleAllCommandHandler : InGameCommandHandler<CreatureCardDisassembleAllCommandBody, CreatureCardDisassembleAllResponseBody>
{
	private class HeroCreatureCardDisassembleAllDetailLog
	{
		public int creatureCardId;

		public int count;

		public int acquriedSoulPowder;

		public HeroCreatureCardDisassembleAllDetailLog(int nCreatureCardId, int nCount, int nAcquriedSoulPowder)
		{
			creatureCardId = nCreatureCardId;
			count = nCount;
			acquriedSoulPowder = nAcquriedSoulPowder;
		}
	}

	private List<HeroCreatureCard> m_changedCreatureCards = new List<HeroCreatureCard>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private List<HeroCreatureCardDisassembleAllDetailLog> m_detailLogs = new List<HeroCreatureCardDisassembleAllDetailLog>();

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nCategoryId = m_body.categoryId;
		if (nCategoryId < 0)
		{
			throw new CommandHandleException(1, "카테고리ID가 유효하지 않습니다. nCategoryId = " + nCategoryId);
		}
		int nTotalAddedSoulPowder = 0;
		foreach (HeroCreatureCard heroCreatureCard in m_myHero.creatureCards.Values.ToList())
		{
			CreatureCard creatureCard = heroCreatureCard.card;
			if (nCategoryId != 0 && nCategoryId != creatureCard.categoryId)
			{
				continue;
			}
			int nSurplusCount = heroCreatureCard.GetSurplusCount();
			if (nSurplusCount > 0)
			{
				heroCreatureCard.count -= nSurplusCount;
				if (heroCreatureCard.count <= 0)
				{
					m_myHero.RemoveCreatureCard(heroCreatureCard.card.id);
				}
				m_changedCreatureCards.Add(heroCreatureCard);
				int nRewardSoulPowder = creatureCard.grade.disassembleSoulPowder * nSurplusCount;
				m_myHero.AddSoulPowder(nRewardSoulPowder);
				nTotalAddedSoulPowder += nRewardSoulPowder;
				m_detailLogs.Add(new HeroCreatureCardDisassembleAllDetailLog(creatureCard.id, nSurplusCount, nRewardSoulPowder));
			}
		}
		if (m_changedCreatureCards.Count > 0)
		{
			SaveToDB();
			SaveToGameLogDB();
		}
		CreatureCardDisassembleAllResponseBody resBody = new CreatureCardDisassembleAllResponseBody();
		resBody.soulPowder = m_myHero.soulPowder;
		resBody.changedCreatureCards = HeroCreatureCard.ToPDHeroCreatureCards(m_changedCreatureCards).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SoulPowder(m_myHero.id, m_myHero.soulPowder));
		foreach (HeroCreatureCard card in m_changedCreatureCards)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteHeroCreatureCard(card));
		}
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardDisassembleAllLog(logId, m_myHero.id, m_currentTime));
			foreach (HeroCreatureCardDisassembleAllDetailLog detailLog in m_detailLogs)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardDisassembleAllDetailLog(Guid.NewGuid(), logId, detailLog.creatureCardId, detailLog.count, detailLog.acquriedSoulPowder));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
