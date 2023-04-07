using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class AnkouTombMoneyBuffActivateCommandHandler : InGameCommandHandler<AnkouTombMoneyBuffActivateCommandBody, AnkouTombMoneyBuffActivateResponseBody>
{
	public const short kResult_NotStatusPlayWaitingOrPlaying = 101;

	public const short kResult_NotEnoughGold = 102;

	public const short kResult_NotEnoughDia = 103;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private int m_nBuffId;

	private int m_nUsedGold;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (!(m_myHero.currentPlace is AnkouTombInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (currentPlace.status != 1 && currentPlace.status != 2)
		{
			throw new CommandHandleException(101, "현재 상태에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_nBuffId = m_body.buffId;
		MoneyBuff moneyBuff = Resource.instance.GetMoneyBuff(m_nBuffId);
		if (moneyBuff == null)
		{
			throw new CommandHandleException(1, "재화버프ID가 유효하지 않습니다. m_nBuffId = " + m_nBuffId);
		}
		int nMoneyAmount = moneyBuff.moneyAmount;
		if (moneyBuff.moneyType == 1)
		{
			if (m_myHero.gold < moneyBuff.moneyAmount)
			{
				throw new CommandHandleException(102, "골드가 부족합니다.");
			}
			m_myHero.UseGold(nMoneyAmount);
			m_nUsedGold = nMoneyAmount;
		}
		else
		{
			if (m_myHero.dia < moneyBuff.moneyAmount)
			{
				throw new CommandHandleException(103, "다이아가 부족합니다.");
			}
			m_myHero.UseDia(nMoneyAmount, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
		}
		m_myHero.SetAnkouTombMoneyBuff(moneyBuff, m_currentTime);
		SaveToDB();
		SaveToDB_Log();
		AnkouTombMoneyBuffActivateResponseBody resBody = new AnkouTombMoneyBuffActivateResponseBody();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.gold = m_myHero.gold;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		if (m_nUsedUnOwnDia > 0)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		}
		if (m_nUsedOwnDia > 0)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		}
		if (m_nUsedGold > 0)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		}
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroMoneyBuffUsedLog(Guid.NewGuid(), m_myHero.id, m_nBuffId, m_nUsedGold, m_nUsedOwnDia, m_nUsedUnOwnDia, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex);
		}
	}
}
