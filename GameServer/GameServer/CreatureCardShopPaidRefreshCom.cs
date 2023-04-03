using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CreatureCardShopPaidRefreshCommandHandler : InGameCommandHandler<CreatureCardShopPaidRefreshCommandBody, CreatureCardShopPaidRefreshResponseBody>
{
	public const short kResult_DailyRefreshCountOverflowed = 101;

	public const short kResult_NotEnoughDia = 102;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		m_myHero.RefreshDailyCreatureCardShopPaidRefreshCount(currentDate);
		DateValuePair<int> dailyCreatureCardShopPaidRefreshCount = m_myHero.dailyCreatureCardShopPaidRefreshCount;
		if (dailyCreatureCardShopPaidRefreshCount.value >= m_myHero.vipLevel.creatureCardShopPaidRefreshMaxCount)
		{
			throw new CommandHandleException(101, "일일횟수가 최대횟수를 넘어갑니다.");
		}
		int nPaidRefreshDia = Resource.instance.creatureCardShopPaidRefreshDia;
		if (m_myHero.dia < nPaidRefreshDia)
		{
			throw new CommandHandleException(102, "다이아가 부족합니다.");
		}
		m_myHero.UseDia(nPaidRefreshDia, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
		m_myHero.RefreshCreatureCardShop();
		dailyCreatureCardShopPaidRefreshCount.value++;
		SaveToDB();
		SaveToGameLogDB();
		CreatureCardShopPaidRefreshResponseBody resBody = new CreatureCardShopPaidRefreshResponseBody();
		resBody.date = (DateTime)dailyCreatureCardShopPaidRefreshCount.date;
		resBody.dailyPaidRefreshCount = dailyCreatureCardShopPaidRefreshCount.value;
		resBody.randomProducts = HeroCreatureCardShopRandomProduct.ToPDHeroCreatureCardShopRandomProducts(m_myHero.creatureCardShopRandomProducts.Values).ToArray();
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_CreatureCardShopPaidRefresh(m_myHero.id, m_myHero.dailyCreatureCardShopPaidRefreshCount.date, m_myHero.dailyCreatureCardShopPaidRefreshCount.value));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_CreatureCardShopId(m_myHero.id, m_myHero.creatureCardShopId));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroCreatureCardshopFixedProductBuy(m_myHero.id));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroCreatureCardShopRandomProducts(m_myHero.id));
		foreach (HeroCreatureCardShopRandomProduct product in m_myHero.creatureCardShopRandomProducts.Values)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroCreatureCardShopRandomProduct(product.hero.id, product.product.id));
		}
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardShopLog(m_myHero.creatureCardShopId, m_myHero.id, 1, m_nUsedOwnDia, m_nUsedUnOwnDia, m_currentTime));
			foreach (HeroCreatureCardShopRandomProduct product in m_myHero.creatureCardShopRandomProducts.Values)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardShopRandomProductLog(m_myHero.creatureCardShopId, product.product.id, product.product.creatureCard.id, m_currentTime));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
