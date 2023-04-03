using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CreatureCardLuckyShop5TimePickCommandHandler : InGameCommandHandler<CreatureCardLuckyShop5TimePickCommandBody, CreatureCardLuckyShop5TimePickResponseBody>
{
	public const short kResult_NotEnoughDia = 101;

	public const short kResult_OverflowedPickCount = 102;

	private HashSet<HeroCreatureCard> m_changedHeroCreatureCards = new HashSet<HeroCreatureCard>();

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		CreatureCardLuckyShop creatureCardLuckyShop = Resource.instance.creatureCardLuckyShop;
		int nRequiredDia = creatureCardLuckyShop.pick5TimeDia;
		if (m_myHero.dia < nRequiredDia)
		{
			throw new CommandHandleException(101, "다이아가 부족합니다.");
		}
		m_myHero.RefreshCreatureCardLuckyShopPickCount(currentDate);
		if (m_myHero.creatureCardLuckyShopPick5TimeCount >= m_myHero.vipLevel.luckyShopPickMaxCount)
		{
			throw new CommandHandleException(102, "뽑기횟수가 최대횟수를 넘어갑니다.");
		}
		List<CreatureCardLuckyShopNormalPoolEntry> normalEntries = creatureCardLuckyShop.SelectNormalEntries(5 - creatureCardLuckyShop.pick5TimeSpecialPickCount);
		List<int> exclusiveCreatureCardIds = new List<int>();
		foreach (CreatureCardLuckyShopNormalPoolEntry normalEntry in normalEntries)
		{
			HeroCreatureCard heroCreatureCard2 = m_myHero.IncreaseCreatureCardCount(normalEntry.creatureCard);
			m_changedHeroCreatureCards.Add(heroCreatureCard2);
			exclusiveCreatureCardIds.Add(normalEntry.creatureCard.id);
		}
		List<CreatureCardLuckyShopSpecialPoolEntry> specialEntries = creatureCardLuckyShop.SelectSpecialEntries(exclusiveCreatureCardIds, creatureCardLuckyShop.pick5TimeSpecialPickCount);
		foreach (CreatureCardLuckyShopSpecialPoolEntry specialEntry in specialEntries)
		{
			HeroCreatureCard heroCreatureCard = m_myHero.IncreaseCreatureCardCount(specialEntry.creatureCard);
			m_changedHeroCreatureCards.Add(heroCreatureCard);
		}
		long lnRewardGold = creatureCardLuckyShop.pick5TimeGoldRewardValue;
		m_myHero.UseDia(nRequiredDia, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
		m_myHero.AddGold(lnRewardGold);
		m_myHero.creatureCardLuckyShopPick5TimeCount++;
		SaveToDB();
		SaveToLogDB(lnRewardGold);
		CreatureCardLuckyShop5TimePickResponseBody resBody = new CreatureCardLuckyShop5TimePickResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.pick5TimeCount = m_myHero.creatureCardLuckyShopPick5TimeCount;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		resBody.gold = m_myHero.gold;
		resBody.maxGold = m_myHero.maxGold;
		resBody.changedCreatureCards = HeroCreatureCard.ToPDHeroCreatureCards(m_changedHeroCreatureCards).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_CreatureCardLuckyShopPickCount(m_myHero.id, m_myHero.creatureCardLuckyShopPickDate, m_myHero.creatureCardLuckyShopFreePickCount, m_myHero.creatureCardLuckyShopPick1TimeCount, m_myHero.creatureCardLuckyShopPick5TimeCount));
		foreach (HeroCreatureCard heroCreatureCard in m_changedHeroCreatureCards)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateHeroCreatureCard(heroCreatureCard));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB(long lnRewardGold)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardLuckyShopPickLog(logId, m_myHero.id, 3, m_nUsedOwnDia, m_nUsedUnOwnDia, lnRewardGold, m_currentTime));
			foreach (HeroCreatureCard heroCreatureCard in m_changedHeroCreatureCards)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardLuckyShopPickDetailLog(Guid.NewGuid(), logId, heroCreatureCard.card.id));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
