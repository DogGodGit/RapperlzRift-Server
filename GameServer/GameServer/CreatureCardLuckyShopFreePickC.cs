using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class CreatureCardLuckyShopFreePickCommandHandler : InGameCommandHandler<CreatureCardLuckyShopFreePickCommandBody, CreatureCardLuckyShopFreePickResponseBody>
{
	public const short kResult_NotElapsedTime = 101;

	public const short kResult_OverflowedFreePickCount = 102;

	private HeroCreatureCard m_heroCreatureCard;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		if (m_myHero.GetRemainingCreatureCardLuckyShopFreePickTime(m_currentTime) > 0f)
		{
			throw new CommandHandleException(101, "아직 무료뽑기대기시간이 경과하지 않았습니다.");
		}
		CreatureCardLuckyShop creatureCardLuckyShop = Resource.instance.creatureCardLuckyShop;
		m_myHero.RefreshCreatureCardLuckyShopPickCount(currentDate);
		if (m_myHero.creatureCardLuckyShopFreePickCount >= creatureCardLuckyShop.freePickCount)
		{
			throw new CommandHandleException(102, "무료뽑기횟수가 최대횟수를 넘어갑니다.");
		}
		CreatureCardLuckyShopNormalPoolEntry normalPickEntry = creatureCardLuckyShop.SelectNormalEnrty();
		long lnRewardGold = creatureCardLuckyShop.pick1TimeGoldRewardValue;
		if (normalPickEntry != null)
		{
			m_heroCreatureCard = m_myHero.IncreaseCreatureCardCount(normalPickEntry.creatureCard);
		}
		m_myHero.AddGold(lnRewardGold);
		m_myHero.creatureCardLuckyShopFreePickTime = m_currentTime;
		m_myHero.creatureCardLuckyShopFreePickCount++;
		SaveToDB();
		SaveToLogDB(lnRewardGold);
		CreatureCardLuckyShopFreePickResponseBody resBody = new CreatureCardLuckyShopFreePickResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.freePickCount = m_myHero.creatureCardLuckyShopFreePickCount;
		resBody.freePickRemainingTime = m_myHero.GetRemainingCreatureCardLuckyShopFreePickTime(m_currentTime);
		resBody.gold = m_myHero.gold;
		resBody.maxGold = m_myHero.maxGold;
		resBody.changedCreatureCard = ((m_heroCreatureCard != null) ? m_heroCreatureCard.ToPDHeroCreatureCard() : null);
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_CreatureCardLuckyShopFreePickTime(m_myHero.id, m_myHero.creatureCardLuckyShopFreePickTime));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_CreatureCardLuckyShopPickCount(m_myHero.id, m_myHero.creatureCardLuckyShopPickDate, m_myHero.creatureCardLuckyShopFreePickCount, m_myHero.creatureCardLuckyShopPick1TimeCount, m_myHero.creatureCardLuckyShopPick5TimeCount));
		dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateHeroCreatureCard(m_heroCreatureCard));
		dbWork.Schedule();
	}

	private void SaveToLogDB(long lnRewardGold)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardLuckyShopPickLog(logId, m_myHero.id, 1, 0, 0, lnRewardGold, m_currentTime));
			if (m_heroCreatureCard != null)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardLuckyShopPickDetailLog(Guid.NewGuid(), logId, m_heroCreatureCard.card.id));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
