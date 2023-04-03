using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ClientCommon;

namespace GameServer;

public class AccountChargeEvent
{
	private Account m_account;

	private int m_nEventId;

	private int m_nAccUnOwnDia;

	private HashSet<int> m_rewardedMissions = new HashSet<int>();

	public Account account => m_account;

	public int eventId => m_nEventId;

	public int accUnOwnDia => m_nAccUnOwnDia;

	public HashSet<int> rewardedMissions => m_rewardedMissions;

	public AccountChargeEvent(Account account)
		: this(account, 0)
	{
	}

	public AccountChargeEvent(Account account, int nEventId)
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
		m_nAccUnOwnDia = Convert.ToInt32(dr["accUnOwnDia"]);
	}

	public void AddUnOwnDia(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		m_nAccUnOwnDia += nAmount;
	}

	public void AddRewardedMission(int nMissionNo)
	{
		m_rewardedMissions.Add(nMissionNo);
	}

	public bool IsRewardedMission(int nMissionNo)
	{
		return m_rewardedMissions.Contains(nMissionNo);
	}

	public PDAccountChargeEvent ToPDAccountChargeEvent()
	{
		PDAccountChargeEvent inst = new PDAccountChargeEvent();
		inst.eventId = m_nEventId;
		inst.accUnOwnDia = m_nAccUnOwnDia;
		inst.rewardedMissions = m_rewardedMissions.ToArray();
		return inst;
	}
}
