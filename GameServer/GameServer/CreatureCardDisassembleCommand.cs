using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class CreatureCardDisassembleCommandHandler : InGameCommandHandler<CreatureCardDisassembleCommandBody, CreatureCardDisassembleResponseBody>
{
	public const short kResult_NotEnoughCreauteCard = 101;

	private HeroCreatureCard m_targetHeroCreatureCard;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nCardId = m_body.cardId;
		int nCount = m_body.count;
		if (nCardId <= 0)
		{
			throw new CommandHandleException(1, "카드ID가 유효하지 않습니다. nCardId = " + nCardId);
		}
		if (nCount <= 0)
		{
			throw new CommandHandleException(1, "카드수량이 유효하지 않습니다. nCount = " + nCount);
		}
		m_targetHeroCreatureCard = m_myHero.GetCreatureCard(nCardId);
		if (m_targetHeroCreatureCard == null)
		{
			throw new CommandHandleException(1, "카드가 존재하지 않습니다.");
		}
		if (m_targetHeroCreatureCard.count < nCount)
		{
			throw new CommandHandleException(101, "카드수량이 부족합니다.");
		}
		m_targetHeroCreatureCard.count -= nCount;
		if (m_targetHeroCreatureCard.count <= 0)
		{
			m_myHero.RemoveCreatureCard(m_targetHeroCreatureCard.card.id);
		}
		CreatureCard creatureCard = m_targetHeroCreatureCard.card;
		int nRewardedSoulPowder = creatureCard.grade.disassembleSoulPowder * nCount;
		m_myHero.AddSoulPowder(nRewardedSoulPowder);
		SaveToDB();
		SaveToGameLogDB(nCount, nRewardedSoulPowder);
		CreatureCardDisassembleResponseBody resBody = new CreatureCardDisassembleResponseBody();
		resBody.changedCreatureCard = m_targetHeroCreatureCard.ToPDHeroCreatureCard();
		resBody.soulPowder = m_myHero.soulPowder;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SoulPowder(m_myHero.id, m_myHero.soulPowder));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteHeroCreatureCard(m_targetHeroCreatureCard));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nUseCount, int nAcquiredSoulPowder)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardDisassembleLog(Guid.NewGuid(), m_myHero.id, m_targetHeroCreatureCard.card.id, nUseCount, nAcquiredSoulPowder, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
