using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WisdomTempleStep
{
	public const int kType_Puzzle = 1;

	public const int kType_Quiz = 2;

	private WisdomTemple m_wisdomTemple;

	private int m_nNo;

	private int m_nType;

	private int m_nStartDelayTime;

	private ItemReward m_itemReward;

	private List<WisdomTempleQuizMonsterPosition> m_quizMonsterPositions = new List<WisdomTempleQuizMonsterPosition>();

	private List<WisdomTempleQuizPoolEntry> m_quizPoolEntries = new List<WisdomTempleQuizPoolEntry>();

	private int m_nQuizTotalPoint;

	public WisdomTemple wisdomTemple => m_wisdomTemple;

	public int no => m_nNo;

	public int type => m_nType;

	public int startDelayTime => m_nStartDelayTime;

	public ItemReward itemReward => m_itemReward;

	public List<WisdomTempleQuizMonsterPosition> quizMonsterPositions => m_quizMonsterPositions;

	public WisdomTempleStep(WisdomTemple wisdomTemple)
	{
		m_wisdomTemple = wisdomTemple;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["stepNo"]);
		m_nType = Convert.ToInt32(dr["type"]);
		if (!IsDefinedType(m_nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nType = " + m_nType);
		}
		m_nStartDelayTime = Convert.ToInt32(dr["startDelayTime"]);
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
		else if (lnItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
		}
	}

	public void AddQuizMonsterPosition(WisdomTempleQuizMonsterPosition quizMonsterPosition)
	{
		if (quizMonsterPosition == null)
		{
			throw new ArgumentNullException("quizMonsterPosition");
		}
		m_quizMonsterPositions.Add(quizMonsterPosition);
	}

	public void AddQuizPoolEntry(WisdomTempleQuizPoolEntry quizPoolEntry)
	{
		if (quizPoolEntry == null)
		{
			throw new ArgumentNullException("quizPoolEntry");
		}
		m_quizPoolEntries.Add(quizPoolEntry);
		m_nQuizTotalPoint += quizPoolEntry.point;
	}

	public WisdomTempleQuizPoolEntry GetQuizPoolEntry(int nQuizNo)
	{
		int nIndex = nQuizNo - 1;
		if (nIndex < 0 || nIndex >= m_quizPoolEntries.Count)
		{
			return null;
		}
		return m_quizPoolEntries[nIndex];
	}

	public WisdomTempleQuizPoolEntry SelectQuizPoolEntry()
	{
		return Util.SelectPickEntry(m_quizPoolEntries, m_nQuizTotalPoint);
	}

	public static bool IsDefinedType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
