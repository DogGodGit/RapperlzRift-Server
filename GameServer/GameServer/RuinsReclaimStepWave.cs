using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimStepWave
{
	public const int kTargetType_AllMonster = 1;

	public const int kTargetType_TargetMonster = 2;

	private RuinsReclaimStep m_step;

	private int m_nNo;

	private int m_nTargetType;

	private int m_nTargetArrangeKey;

	private RuinsReclaimStepWaveSkill m_skill;

	private List<RuinsReclaimMonsterArrange> m_monsterArranges = new List<RuinsReclaimMonsterArrange>();

	public RuinsReclaimStep step => m_step;

	public int no => m_nNo;

	public int targetType => m_nTargetType;

	public int TargetArrangeKey => m_nTargetArrangeKey;

	public RuinsReclaimStepWaveSkill skill => m_skill;

	public List<RuinsReclaimMonsterArrange> monsterArrages => m_monsterArranges;

	public RuinsReclaimStepWave(RuinsReclaimStep step)
	{
		m_step = step;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["waveNo"]);
		m_nTargetType = Convert.ToInt32(dr["targetType"]);
		if (!IsDefinedTargetType(m_nTargetType))
		{
			SFLogUtil.Warn(GetType(), "목표타입이 유효하지 않습니다. stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", m_nTargetType = " + m_nTargetType);
		}
		m_nTargetArrangeKey = Convert.ToInt32(dr["targetArrangeKey"]);
		if (m_nTargetArrangeKey < 0)
		{
			SFLogUtil.Warn(GetType(), "목표배치키가 유효하지 않습니다. stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", m_nTargetArrangeKey = " + m_nTargetArrangeKey);
		}
	}

	public void SetSkill(RuinsReclaimStepWaveSkill skill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_skill = skill;
	}

	public void AddMonsterArrange(RuinsReclaimMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange);
	}

	public static bool IsDefinedTargetType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
