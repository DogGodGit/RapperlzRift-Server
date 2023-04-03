using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class AncientRelicStepWave
{
	private AncientRelicStep m_step;

	private int m_nNo;

	private string m_sGuideTitleKey;

	private string m_sGuideContentKey;

	private List<AncientRelicMonsterArrange> m_arranges = new List<AncientRelicMonsterArrange>();

	public AncientRelic ancientRelic => m_step.ancientRelic;

	public AncientRelicStep step => m_step;

	public int no => m_nNo;

	public string guideTitleKey => m_sGuideTitleKey;

	public string guideContentKey => m_sGuideContentKey;

	public List<AncientRelicMonsterArrange> arranges => m_arranges;

	public AncientRelicStepWave(AncientRelicStep step)
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
		m_sGuideTitleKey = Convert.ToString(dr["guideTitleKey"]);
		m_sGuideContentKey = Convert.ToString(dr["guideContentKey"]);
	}

	public void AddArrange(AncientRelicMonsterArrange arrange)
	{
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arranges.Add(arrange);
	}

	public AncientRelicMonsterArrange GetArrange(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_arranges.Count)
		{
			return null;
		}
		return m_arranges[nIndex];
	}
}
