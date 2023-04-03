using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TradeShipStep
{
	private TradeShip m_tradeShip;

	private int m_nNo;

	private int m_nTargetMonsterKillCount;

	public TradeShip tradeShip => m_tradeShip;

	public int no => m_nNo;

	public int targetMonsterKillCount => m_nTargetMonsterKillCount;

	public TradeShipStep(TradeShip tradeShip)
	{
		m_tradeShip = tradeShip;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["stepNo"]);
		m_nTargetMonsterKillCount = Convert.ToInt32(dr["targetMonsterKillCount"]);
		if (m_nTargetMonsterKillCount < 0)
		{
			SFLogUtil.Warn(GetType(), "목표몬스터처치수가 유효하지 않습니다. m_nTargetMonsterKillCount = " + m_nTargetMonsterKillCount);
		}
	}
}
