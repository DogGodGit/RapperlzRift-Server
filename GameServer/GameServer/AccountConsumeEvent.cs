using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ClientCommon;

namespace GameServer;

public class AccountConsumeEvent
{
	private Account m_account;

	private int m_nEventId;

	private int m_nAccDia;

	private HashSet<int> m_rewardedMissions = new HashSet<int>();

	public Account account => m_account;

	public int eventId => m_nEventId;

	public int accDia => m_nAccDia;

	public HashSet<int> rewardedMissions => m_rewardedMissions;

	public AccountConsumeEvent(Account account)
		: this(account, 0)
	{
	}

	public AccountConsumeEvent(Account account, int nEventId)
	{
		if (account == null)
		{
			throw new ArgumentNullException("account");
		}
		m_account = account;
		m_nEventId = nEventId;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nEventId = Convert.ToInt32(dr["eventId"]);
		m_nAccDia = Convert.ToInt32(dr["accDia"]);
	}

	public void AddDia(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		m_nAccDia += nAmount;
	}

	public void AddRewardedMission(int nMissionNo)
	{
		m_rewardedMissions.Add(nMissionNo);
	}

	public bool IsRewardedMission(int nMissionNo)
	{
		return m_rewardedMissions.Contains(nMissionNo);
	}

	public PDAccountConsumeEvent ToPDAccountConsumeEvent()
	{
		PDAccountConsumeEvent inst = new PDAccountConsumeEvent();
		inst.eventId = m_nEventId;
		inst.accDia = m_nAccDia;
		inst.rewardedMissions = m_rewardedMissions.ToArray();
		return inst;
	}
}
