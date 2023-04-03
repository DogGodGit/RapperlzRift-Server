using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WisdomTempleQuizWrongAnswerPoolEntry : IWisdomTempleQuizAnswerPoolEntry, IPickEntry
{
	private WisdomTempleQuizPoolEntry m_quizPoolEntry;

	private int m_nNo;

	private MonsterArrange m_monsterArrange;

	public WisdomTempleQuizPoolEntry quizPoolEntry => m_quizPoolEntry;

	public int no => m_nNo;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public int point => 1;

	int IPickEntry.point => point;

	public WisdomTempleQuizWrongAnswerPoolEntry(WisdomTempleQuizPoolEntry quizPoolEntry)
	{
		m_quizPoolEntry = quizPoolEntry;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		long lnMonsterArrangeId = Convert.ToInt32(dr["monsterArrangeId"]);
		if (lnMonsterArrangeId > 0)
		{
			m_monsterArrange = Resource.instance.GetMonsterArrange(lnMonsterArrangeId);
			if (m_monsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. stepNo = " + m_quizPoolEntry.step.no + ", quizNo = " + m_quizPoolEntry.no + ", m_nNo = " + m_nNo + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터배치ID가 유효하지 않습니다. stepNo = " + m_quizPoolEntry.step.no + ", quizNo = " + m_quizPoolEntry.no + ", m_nNo = " + m_nNo + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
	}
}
