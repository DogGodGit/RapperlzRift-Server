using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class TradeShipAdditionalRewardExpReceiveCommandHandler : InGameCommandHandler<TradeShipAdditionalRewardExpReceiveCommandBody, TradeShipAdditionalRewardExpReceiveResponseBody>
{
	public const short kResult_NotStatusFinished = 101;

	public const short kResult_NotEnoughUnOwnDia = 102;

	private TradeShipInstance m_tradeShipInst;

	private int m_nRewardExpType;

	private int m_nRequiredUnOwnDia;

	private long m_lnAcquisitionExp;

	protected override void HandleInGameCommand()
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		m_tradeShipInst = m_myHero.currentPlace as TradeShipInstance;
		if (m_tradeShipInst == null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!m_tradeShipInst.isFinished)
		{
			throw new CommandHandleException(101, "현재 상태에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_nRewardExpType = m_body.rewardExpType;
		if (!TradeShip.IsDefinedAdditionalRewardExpType(m_nRewardExpType))
		{
			throw new CommandHandleException(1, "추가경험치보상타입이 유효하지 않습니다. m_nRewardExpType = " + m_nRewardExpType);
		}
		if (m_tradeShipInst.ContainsAdditionalRewardReceivedHero(m_myHero.id))
		{
			throw new CommandHandleException(1, "이미 추가보상을 받았습니다.");
		}
		TradeShip tradeShip = m_tradeShipInst.tradeShip;
		int nMultiple = 0;
		if (m_nRewardExpType == 1)
		{
			m_nRequiredUnOwnDia = tradeShip.exp2xRewardRequiredUnOwnDia;
			nMultiple = 2;
		}
		else
		{
			m_nRequiredUnOwnDia = tradeShip.exp3xRewardRequiredUnOwnDia;
			nMultiple = 3;
		}
		if (m_myHero.unOwnDia < m_nRequiredUnOwnDia)
		{
			throw new CommandHandleException(102, "비귀속다이아가 부족합니다.");
		}
		long nReceivedRewardExp = m_tradeShipInst.GetReceivedRewardExp(m_myHero.id);
		if (nReceivedRewardExp <= 0)
		{
			throw new CommandHandleException(1, "받은 보상경험치가 없습니다.");
		}
		m_myHero.account.UseUnOwnDia(m_nRequiredUnOwnDia, currentTime);
		m_tradeShipInst.AddAdditionalRewardReceivedHero(m_myHero.id);
		m_lnAcquisitionExp = nReceivedRewardExp * (nMultiple - 1);
		m_lnAcquisitionExp = (long)Math.Floor((float)m_lnAcquisitionExp * Cache.instance.GetWorldLevelExpFactor(base.hero.level));
		m_myHero.AddExp(m_lnAcquisitionExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		SaveToDB();
		SaveToDB_Log();
		TradeShipAdditionalRewardExpReceiveResponseBody resBody = new TradeShipAdditionalRewardExpReceiveResponseBody();
		resBody.acquiredExp = m_lnAcquisitionExp;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.unOwnDia = m_myHero.unOwnDia;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddTradeShipCompletionMemberAdditionalRewardLog(m_tradeShipInst.instanceId, m_myHero.id, m_nRewardExpType, m_lnAcquisitionExp, m_nRequiredUnOwnDia));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex);
		}
	}
}
