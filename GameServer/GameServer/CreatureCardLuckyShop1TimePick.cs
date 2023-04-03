using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CreatureCardLuckyShop1TimePickCommandHandler : InGameCommandHandler<CreatureCardLuckyShop1TimePickCommandBody, CreatureCardLuckyShop1TimePickResponseBody>
{
	public const short kResult_NotEnoughDia = 101;

	public const short kResult_OverflowedPickCount = 102;

	private HeroCreatureCard m_heroCreatureCard;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		CreatureCardLuckyShop creatureCardLuckyShop = Resource.instance.creatureCardLuckyShop;
		int nRequiredDia = creatureCardLuckyShop.pick1TimeDia;
		if (m_myHero.dia < nRequiredDia)
		{
			throw new CommandHandleException(101, "다이아가 부족합니다.");
		}
		m_myHero.RefreshCreatureCardLuckyShopPickCount(currentDate);
		if (m_myHero.creatureCardLuckyShopPick1TimeCount >= m_myHero.vipLevel.luckyShopPickMaxCount)
		{
			throw new CommandHandleException(102, "뽑기횟수가 최대횟수를 넘어갑니다.");
		}
		CreatureCardLuckyShopNormalPoolEntry normalPickEntry = creatureCardLuckyShop.SelectNormalEnrty();
		long lnRewardGold = creatureCardLuckyShop.pick1TimeGoldRewardValue;
		m_myHero.UseDia(nRequiredDia, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
		if (normalPickEntry != null)
		{
			m_heroCreatureCard = m_myHero.IncreaseCreatureCardCount(normalPickEntry.creatureCard);
		}
		m_myHero.AddGold(lnRewardGold);
		m_myHero.creatureCardLuckyShopPick1TimeCount++;
		SaveToDB();
		SaveToLogDB(lnRewardGold);
		CreatureCardLuckyShop1TimePickResponseBody resBody = new CreatureCardLuckyShop1TimePickResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.pick1TimeCount = m_myHero.creatureCardLuckyShopPick1TimeCount;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		resBody.gold = m_myHero.gold;
		resBody.maxGold = m_myHero.maxGold;
		resBody.changedCreatureCard = ((m_heroCreatureCard != null) ? m_heroCreatureCard.ToPDHeroCreatureCard() : null);
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
		dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateHeroCreatureCard(m_heroCreatureCard));
		dbWork.Schedule();
	}

	private void SaveToLogDB(long lnRewardGold)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardLuckyShopPickLog(logId, m_myHero.id, 2, m_nUsedOwnDia, m_nUsedUnOwnDia, lnRewardGold, m_currentTime));
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
