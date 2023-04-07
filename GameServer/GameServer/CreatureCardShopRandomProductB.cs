using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class CreatureCardShopRandomProductBuyCommandHandler : InGameCommandHandler<CreatureCardShopRandomProductBuyCommandBody, CreatureCardShopRandomProductBuyResponseBody>
{
	public const short kResult_NotExistProduct = 101;

	public const short kResult_AlreadyBoughtProduct = 102;

	public const short kResult_NotEnoughSoulPowder = 103;

	private HeroCreatureCardShopRandomProduct m_targetHeroRandomProduct;

	private HeroCreatureCard m_changedHeroCreatureCard;

	private DateTimeOffset m_currendTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currendTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nProductId = m_body.productId;
		if (nProductId <= 0)
		{
			throw new CommandHandleException(1, "상품ID가 유효하지 않습니다.");
		}
		m_targetHeroRandomProduct = m_myHero.GetCreatureCardShopRandomProduct(nProductId);
		if (m_targetHeroRandomProduct == null)
		{
			throw new CommandHandleException(101, "존재하지 않는 랜덤상품입니다. nProductId = " + nProductId);
		}
		if (m_targetHeroRandomProduct.purchased)
		{
			throw new CommandHandleException(102, "이미 구매한 랜덤상품입니다. nProductId = " + nProductId);
		}
		CreatureCardShopRandomProduct randomProduct = m_targetHeroRandomProduct.product;
		CreatureCard creatureCard = randomProduct.creatureCard;
		int nSaleSoulPowder = creatureCard.grade.saleSoulPowder;
		if (m_myHero.soulPowder < nSaleSoulPowder)
		{
			throw new CommandHandleException(103, "영혼가루가 부족합니다.");
		}
		m_changedHeroCreatureCard = m_myHero.IncreaseCreatureCardCount(creatureCard);
		m_myHero.UseSoulPowder(nSaleSoulPowder);
		m_targetHeroRandomProduct.purchased = true;
		SaveToDB();
		SaveToGameLogDB(nSaleSoulPowder);
		CreatureCardShopRandomProductBuyResponseBody resBody = new CreatureCardShopRandomProductBuyResponseBody();
		resBody.soulPowder = m_myHero.soulPowder;
		resBody.changedCreatureCard = m_changedHeroCreatureCard.ToPDHeroCreatureCard();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SoulPowder(m_myHero.id, m_myHero.soulPowder));
		dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateHeroCreatureCard(m_changedHeroCreatureCard));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroCreatureCardShopRandomProduct(m_targetHeroRandomProduct.hero.id, m_targetHeroRandomProduct.product.id, m_targetHeroRandomProduct.purchased));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nUsedSoulPowder)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_UpdateHeroCreatureCardShopRandomProductLog_Purchase(m_myHero.creatureCardShopId, m_targetHeroRandomProduct.product.id, nUsedSoulPowder, m_currendTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
