using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class WingStep
{
	private int m_nStep;

	private string m_sNameKey;

	private int m_nEnchantMaterialItemCount;

	private Wing m_rewardWing;

	private Dictionary<int, WingStepPart> m_parts = new Dictionary<int, WingStepPart>();

	private List<WingStepLevel> m_levels = new List<WingStepLevel>();

	public int step => m_nStep;

	public string nameKey => m_sNameKey;

	public int enchantMaterialItemCount => m_nEnchantMaterialItemCount;

	public Wing rewardWing => m_rewardWing;

	public List<WingStepLevel> levels => m_levels;

	public WingStepLevel lastLevel => m_levels.LastOrDefault();

	public bool isLastStep => m_nStep == Resource.instance.lastWingStep.step;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nStep = Convert.ToInt32(dr["step"]);
		if (m_nStep <= 0)
		{
			SFLogUtil.Warn(GetType(), "단계가 유효하지 않습니다. m_nStep = " + m_nStep);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_nEnchantMaterialItemCount = Convert.ToInt32(dr["enchantMaterialItemCount"]);
		if (m_nEnchantMaterialItemCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "강화재료아이템수량이 유효하지 않습니다. m_nStep = " + m_nStep + ", m_nEnchantMaterialItemCount = " + m_nEnchantMaterialItemCount);
		}
		int nRewardWingId = Convert.ToInt32(dr["rewardWingId"]);
		if (nRewardWingId > 0)
		{
			m_rewardWing = Resource.instance.GetWing(nRewardWingId);
			if (m_rewardWing == null)
			{
				SFLogUtil.Warn(GetType(), "존재하지 않는 보상날개입니다. m_nStep = " + m_nStep + ", nRewardWingId = " + nRewardWingId);
			}
		}
	}

	public void AddPart(WingStepPart part)
	{
		if (part == null)
		{
			throw new ArgumentNullException("part");
		}
		if (part.part == null)
		{
			throw new Exception("날계단계파트에 날계파트가 존재하지 않습니다.");
		}
		m_parts.Add(part.partId, part);
		part.step = this;
	}

	public WingStepPart GetPart(int nId)
	{
		if (!m_parts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddLevel(WingStepLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_levels.Add(level);
		level.step = this;
	}

	public WingStepLevel GetLevel(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_levels.Count)
		{
			return null;
		}
		return m_levels[nIndex];
	}
}
