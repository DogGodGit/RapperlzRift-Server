using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CreatureCardCollectionActivateCommandHandler : InGameCommandHandler<CreatureCardCollectionActivateCommandBody, CreatureCardCollectionActivateResponseBody>
{
	public const short kResult_CollectionNotCompleted = 101;

	private List<HeroCreatureCard> m_changedHeroCreatureCards = new List<HeroCreatureCard>();

	private HeroCreatureCardCollection m_addedHeroCreatureCardCollection;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nCollectionId = m_body.collectionId;
		if (nCollectionId <= 0)
		{
			throw new CommandHandleException(1, "컬렉션ID가 유효하지 않습니다. nCollectionId = " + nCollectionId);
		}
		if (m_myHero.IsCreatureCardCollectionActivated(nCollectionId))
		{
			throw new CommandHandleException(1, "이미 활성화된 크리처카드컬렉션입니다. nCollectionId = " + nCollectionId);
		}
		CreatureCardCollection collection = Resource.instance.GetCreatureCardCollection(nCollectionId);
		if (collection == null)
		{
			throw new CommandHandleException(1, "크리처카드컬렉션이 존재하지 않습니다. nCollectionId = " + nCollectionId);
		}
		foreach (CreatureCardCollectionEntry entry in collection.entries)
		{
			HeroCreatureCard card2 = m_myHero.GetCreatureCard(entry.card.id);
			if (card2 == null)
			{
				throw new CommandHandleException(101, "컬렉션이 완성되지 않았습니다. nCollectionId = " + nCollectionId + ", creatureCardId = " + entry.card.id);
			}
			m_changedHeroCreatureCards.Add(card2);
		}
		foreach (HeroCreatureCard card in m_changedHeroCreatureCards)
		{
			card.count--;
			if (card.count <= 0)
			{
				m_myHero.RemoveCreatureCard(card.card.id);
			}
		}
		m_addedHeroCreatureCardCollection = new HeroCreatureCardCollection(m_myHero, collection);
		m_myHero.AddActivatedCreatureCardCollection(m_addedHeroCreatureCardCollection);
		m_myHero.AddCreatureCardFamePoint(collection.grade.collectionFamePoint, m_currentTime);
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB();
		SaveToGameLogDB();
		CreatureCardCollectionActivateResponseBody resBody = new CreatureCardCollectionActivateResponseBody();
		resBody.changedCreatureCards = HeroCreatureCard.ToPDHeroCreatureCards(m_changedHeroCreatureCards).ToArray();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.creatureCardCollectionFamePoint = m_myHero.creatureCardCollectionFamePoint;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_CreatureCardCollectionFamePoint(m_myHero));
		foreach (HeroCreatureCard card in m_changedHeroCreatureCards)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteHeroCreatureCard(card));
		}
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroCreatureCardCollection(m_addedHeroCreatureCardCollection.hero.id, m_addedHeroCreatureCardCollection.collection.id));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardCollectionActivationLog(logId, m_myHero.id, m_addedHeroCreatureCardCollection.collection.id, m_currentTime));
			foreach (HeroCreatureCard card in m_changedHeroCreatureCards)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardCollectionActivationDetailLog(Guid.NewGuid(), logId, card.card.id));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
