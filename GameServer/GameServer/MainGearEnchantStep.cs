using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class MainGearEnchantStep
{
	private int m_nStep;

	private int m_nNextEnchantMaterialItemId;

	private int m_nNextEnchantPenaltyPreventItemId;

	private List<MainGearEnchantLevel> m_levels = new List<MainGearEnchantLevel>();

	public int step => m_nStep;

	public int nextEnchantMaterialItemId => m_nNextEnchantMaterialItemId;

	public int nextEnchantPenaltyPreventItemId => m_nNextEnchantPenaltyPreventItemId;

	public MainGearEnchantLevel minEnchantLevel => m_levels.FirstOrDefault();

	public MainGearEnchantLevel maxEnchantLevel => m_levels.LastOrDefault();

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nStep = Convert.ToInt32(dr["step"]);
		m_nNextEnchantMaterialItemId = Convert.ToInt32(dr["nextEnchantMaterialItemId"]);
		if (m_nNextEnchantMaterialItemId <= 0)
		{
			SFLogUtil.Warn(GetType(), "다음강화재료아이템 ID가 유효하지 않습니다. m_nNextEnchantMaterialItemId = " + m_nNextEnchantMaterialItemId);
		}
		m_nNextEnchantPenaltyPreventItemId = Convert.ToInt32(dr["nextEnchantPenaltyPreventItemId"]);
		if (m_nNextEnchantPenaltyPreventItemId <= 0)
		{
			SFLogUtil.Warn(GetType(), "다음강화패널티방지아이템 ID가 유효하지 않습니다. m_nNextEnchantPenaltyPreventItemId = " + m_nNextEnchantPenaltyPreventItemId);
		}
	}

	public void AddLevel(MainGearEnchantLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		if (level.step != null)
		{
			throw new Exception("이미 메인장비강화단계에 추가된 메인장비강화레벨입니다.");
		}
		m_levels.Add(level);
		level.step = this;
		if (level.penaltyPreventEnabled && m_nNextEnchantPenaltyPreventItemId < 0)
		{
			SFLogUtil.Warn(GetType(), "레벨의 패널티 아이템 사용여부가 유효하지 않습니다. step = " + m_nStep + ", level = " + level.enchantLevel);
		}
	}
}
