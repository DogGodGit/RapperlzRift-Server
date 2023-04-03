using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GameServer;

public class HeroRetrievalLog
{
	private Guid m_logId = Guid.Empty;

	private Guid m_heroId = Guid.Empty;

	private int m_nRetrievalId;

	private int m_nCount;

	private int m_nLevel;

	private int m_nVipLevel;

	private int m_nType;

	private long m_nUsedGold;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	private Guid m_detailLogId = Guid.Empty;

	private long m_lnReawrdExp;

	private int m_nRewardItemId;

	private bool m_bRewardItemOwned;

	private int m_nRewardItemCount;

	public HeroRetrievalLog(Guid logId, Guid heroId, int nRetrievalId, int nCount, int nLevel, int nVipLevel, int nType, long nUsedGold, int nUsedOwnDia, int nUsedUnOwnDia, DateTimeOffset regTime, Guid detailLogId, long lnReawrdExp, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		m_logId = logId;
		m_heroId = heroId;
		m_nRetrievalId = nRetrievalId;
		m_nCount = nCount;
		m_nLevel = nLevel;
		m_nVipLevel = nVipLevel;
		m_nType = nType;
		m_nUsedGold = nUsedGold;
		m_nUsedOwnDia = nUsedOwnDia;
		m_nUsedUnOwnDia = nUsedUnOwnDia;
		m_regTime = regTime;
		m_detailLogId = detailLogId;
		m_lnReawrdExp = lnReawrdExp;
		m_nRewardItemId = nRewardItemId;
		m_bRewardItemOwned = bRewardItemOwned;
		m_nRewardItemCount = nRewardItemCount;
	}

	public List<SqlCommand> ToSqlcommands()
	{
		List<SqlCommand> sqlCommands = new List<SqlCommand>();
		sqlCommands.Add(GameLogDac.CSC_AddHeroRetrievalLogs(m_logId, m_heroId, m_nRetrievalId, m_nCount, m_nLevel, m_nVipLevel, m_nType, m_nUsedGold, m_nUsedOwnDia, m_nUsedUnOwnDia, m_regTime));
		sqlCommands.Add(GameLogDac.CSC_AddHeroRetrievalDetailLog(m_detailLogId, m_logId, m_lnReawrdExp, m_nRetrievalId, m_bRewardItemOwned, m_nRewardItemCount));
		return sqlCommands;
	}
}
