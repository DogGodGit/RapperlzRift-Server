using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class OsirisRoomMonsterArrange
{
	private OsirisRoomDifficultyWave m_wave;

	private int m_nNo;

	private MonsterArrange m_monsterArrange;

	private GoldReward m_killGoldReward;

	public OsirisRoomDifficultyWave wave => m_wave;

	public int no => m_nNo;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public GoldReward killGoldReward => m_killGoldReward;

	public OsirisRoomMonsterArrange(OsirisRoomDifficultyWave wave)
	{
		m_wave = wave;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		Resource res = Resource.instance;
		m_nNo = Convert.ToInt32(dr["arrangeNo"]);
		long lnMonsterArrange = Convert.ToInt64(dr["monsterArrangeId"]);
		if (lnMonsterArrange > 0)
		{
			m_monsterArrange = res.GetMonsterArrange(lnMonsterArrange);
			if (m_monsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. difficulty = " + wave.difficulty.difficulty + ", waveNo = " + wave.no + ", m_nNo = " + m_nNo + ", lnMonsterArrange = " + lnMonsterArrange);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터배치ID가 유효하지 않습니다. difficulty = " + wave.difficulty.difficulty + ", waveNo = " + wave.no + ", m_nNo = " + m_nNo + ", lnMonsterArrange = " + lnMonsterArrange);
		}
		long lnKillGoldRewardId = Convert.ToInt64(dr["killGoldRewardId"]);
		if (lnKillGoldRewardId > 0)
		{
			m_killGoldReward = res.GetGoldReward(lnKillGoldRewardId);
			if (m_killGoldReward == null)
			{
				SFLogUtil.Warn(GetType(), "처치골드보상이 존재하지 않습니다. difficulty = " + wave.difficulty.difficulty + ", waveNo = " + wave.no + ", m_nNo = " + m_nNo + ", lnKillGoldRewardId = " + lnKillGoldRewardId);
			}
		}
		else if (lnKillGoldRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "처치골드보상ID가 유효하지 않습니다. difficulty = " + wave.difficulty.difficulty + ", waveNo = " + wave.no + ", m_nNo = " + m_nNo + ", lnKillGoldRewardId = " + lnKillGoldRewardId);
		}
	}
}
