using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class CreatureCardComposeCommandHandler : InGameCommandHandler<CreatureCardComposeCommandBody, CreatureCardComposeResponseBody>
{
	public const short kResult_NotEnoughVipLevel = 101;

	public const short kResult_NotEnoughSoulPowder = 102;

	private HeroCreatureCard m_changedHeroCreatureCard;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nCardId = m_body.cardId;
		if (nCardId <= 0)
		{
			throw new CommandHandleException(1, "카드ID가 유효하지 않습니다. nCardId = " + nCardId);
		}
		if (!m_myHero.vipLevel.creatureCardCompositionEnabled)
		{
			throw new CommandHandleException(101, "VIP레벨이 부족합니다.");
		}
		CreatureCard creatureCard = Resource.instance.GetCreatureCard(nCardId);
		if (creatureCard == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 크리처카드 입니다.");
		}
		int nCompositionSoulPowder = creatureCard.grade.compositionSoulPowder;
		if (m_myHero.soulPowder < nCompositionSoulPowder)
		{
			throw new CommandHandleException(102, "영혼가루가 부족합니다.");
		}
		m_changedHeroCreatureCard = m_myHero.IncreaseCreatureCardCount(creatureCard);
		m_myHero.UseSoulPowder(nCompositionSoulPowder);
		SaveToDB();
		SaveToGameLogDB(1, nCompositionSoulPowder);
		CreatureCardComposeResponseBody resBody = new CreatureCardComposeResponseBody();
		resBody.changedCreatureCard = m_changedHeroCreatureCard.ToPDHeroCreatureCard();
		resBody.soulPowder = m_myHero.soulPowder;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SoulPowder(m_myHero.id, m_myHero.soulPowder));
		dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateHeroCreatureCard(m_changedHeroCreatureCard));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nAddedCount, int nUsedSoulPowder)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardComposititonLog(Guid.NewGuid(), m_myHero.id, m_changedHeroCreatureCard.card.id, nAddedCount, nUsedSoulPowder, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
