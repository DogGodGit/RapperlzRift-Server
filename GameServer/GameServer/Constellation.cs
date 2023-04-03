using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Constellation
{
	private int m_nId;

	private ConstellationRequiredConditionType m_requiredConditonType = ConstellationRequiredConditionType.HeroLevel;

	private int m_nRequiredConditionValue;

	private List<ConstellationStep> m_steps = new List<ConstellationStep>();

	public int id => m_nId;

	public ConstellationRequiredConditionType requiredConditonType => m_requiredConditonType;

	public int requiredConditionValue => m_nRequiredConditionValue;

	public List<ConstellationStep> steps => m_steps;

	public int lastStep => m_steps.Count;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["constellationId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "별자리ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nRequiredConditionType = Convert.ToInt32(dr["requiredConditionType"]);
		if (!Enum.IsDefined(typeof(ConstellationRequiredConditionType), nRequiredConditionType))
		{
			SFLogUtil.Warn(GetType(), "필요조건타입이 유효하지 않습니다. m_nId = " + m_nId + ", nRequiredConditionType = " + nRequiredConditionType);
		}
		m_requiredConditonType = (ConstellationRequiredConditionType)nRequiredConditionType;
		m_nRequiredConditionValue = Convert.ToInt32(dr["requiredConditionValue"]);
	}

	public void AddStep(ConstellationStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_steps.Add(step);
	}

	public ConstellationStep GetStep(int nStep)
	{
		int nIndex = nStep - 1;
		if (nIndex < 0 || nIndex >= m_steps.Count)
		{
			return null;
		}
		return m_steps[nIndex];
	}
}
